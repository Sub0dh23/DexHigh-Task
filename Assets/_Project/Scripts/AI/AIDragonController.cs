using System.Collections;
using DexHigh.Characters;
using DexHigh.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace DexHigh.AI
{
    public enum AIState
    {
        Idle,
        Circling,
        Chase,
        Attack,
        Dead
    }

    [RequireComponent(typeof(DragonMotor))]
    [RequireComponent(typeof(DragonCombat))]
    [RequireComponent(typeof(DragonHealth))]
    public class AIDragonController : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField] private Transform _targetPlayer;
        [SerializeField] private float _decisionInterval = 0.25f;

        [Header("Tactical Ranges")]
        [SerializeField] private float _meleeRange = 3.5f;
        [SerializeField] private float _midRange = 8.5f;
        [SerializeField] private float _maxCombatRange = 14f;

        [Header("Circling / Repositioning")]
        [SerializeField] private float _circleRadius = 5.5f;
        [SerializeField] private float _circleSpeed = 2.5f;

        private DragonMotor _motor;
        private DragonCombat _combat;
        private DragonHealth _health;
        private NavMeshAgent _navAgent;

        private AIState _currentState = AIState.Idle;
        private float _decisionTimer = 2.0f;
        private float _circleAngle;
        private int _circleDirection = 1;

        public AIState CurrentState => _currentState;

        private void Awake()
        {
            _motor = GetComponent<DragonMotor>();
            _combat = GetComponent<DragonCombat>();
            _health = GetComponent<DragonHealth>();
            _navAgent = GetComponent<NavMeshAgent>();

            if (_navAgent != null)
            {
                // Disable automatic nav agent movement so DragonMotor handles translation & physics cleanly
                _navAgent.updatePosition = false;
                _navAgent.updateRotation = false;
            }
        }

        private void Start()
        {
            if (_targetPlayer == null)
            {
                var player = FindFirstObjectByType<PlayerDragonController>();
                if (player != null) _targetPlayer = player.transform;
            }

            if (_health != null)
            {
                _health.OnDied += _ => SetState(AIState.Dead);
                _health.OnRevived += () => SetState(AIState.Idle);
            }

            _circleDirection = Random.value > 0.5f ? 1 : -1;
        }

        public void SetTarget(Transform target)
        {
            _targetPlayer = target;
        }

        private void Update()
        {
            if (_currentState == AIState.Dead || (_health != null && !_health.IsAlive)) return;

            if (_targetPlayer == null)
            {
                var player = FindFirstObjectByType<PlayerDragonController>();
                if (player != null) _targetPlayer = player.transform;
                if (_targetPlayer == null) return;
            }

            // Wait for warmup to complete before attacking
            if (DexHigh.Core.BattleGameManager.Instance != null &&
                DexHigh.Core.BattleGameManager.Instance.CurrentState != DexHigh.Core.BattleState.Battle)
            {
                _motor.SetLookTarget(_targetPlayer.position);
                _motor.SetMoveInput(Vector3.zero);
                return;
            }

            if (_combat.IsCasting)
            {
                // While casting, maintain target focus
                _motor.SetLookTarget(_targetPlayer.position);
                return;
            }

            _decisionTimer -= Time.deltaTime;
            if (_decisionTimer <= 0f)
            {
                _decisionTimer = _decisionInterval;
                EvaluateStateAndAbilities();
            }

            ExecuteCurrentState();
        }

        private void EvaluateStateAndAbilities()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _targetPlayer.position);

            // Ability Slot 0: FireBreath, Slot 1: TailWhip, Slot 2: FlyDive
            bool canTail = _combat.CanCast(1);
            bool canFire = _combat.CanCast(0);
            bool canFly = _combat.CanCast(2);

            // 1. Close Range (Melee)
            if (distanceToPlayer <= _meleeRange)
            {
                if (canTail)
                {
                    SetState(AIState.Attack);
                    _combat.TryCastAbility(1, _targetPlayer.position);
                    return;
                }
                if (canFire)
                {
                    SetState(AIState.Attack);
                    _combat.TryCastAbility(0, _targetPlayer.position);
                    return;
                }
            }
            // 2. Mid Range
            else if (distanceToPlayer <= _midRange)
            {
                if (canFire)
                {
                    SetState(AIState.Attack);
                    _combat.TryCastAbility(0, _targetPlayer.position);
                    return;
                }
                if (canFly && Random.value > 0.4f)
                {
                    SetState(AIState.Attack);
                    _combat.TryCastAbility(2, _targetPlayer.position);
                    return;
                }
            }
            // 3. Far Range
            else if (distanceToPlayer <= _maxCombatRange)
            {
                if (canFly)
                {
                    SetState(AIState.Attack);
                    _combat.TryCastAbility(2, _targetPlayer.position);
                    return;
                }
            }

            // If attacks not triggered, choose between Chase and Circling
            if (distanceToPlayer > _midRange)
            {
                SetState(AIState.Chase);
            }
            else
            {
                SetState(AIState.Circling);
            }
        }

        private void ExecuteCurrentState()
        {
            Vector3 targetPos = _targetPlayer.position;
            _motor.SetLookTarget(targetPos);

            switch (_currentState)
            {
                case AIState.Chase:
                    MoveTowards(targetPos);
                    break;

                case AIState.Circling:
                    ExecuteCircling(targetPos);
                    break;

                case AIState.Idle:
                    _motor.SetMoveInput(Vector3.zero);
                    break;

                case AIState.Attack:
                    _motor.SetMoveInput(Vector3.zero);
                    break;
            }
        }

        private void ExecuteCircling(Vector3 playerCenter)
        {
            _circleAngle += _circleDirection * _circleSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Cos(_circleAngle), 0f, Mathf.Sin(_circleAngle)) * _circleRadius;
            Vector3 circleDestination = playerCenter + offset;

            MoveTowards(circleDestination);
        }

        private void MoveTowards(Vector3 destination)
        {
            if (_navAgent != null && _navAgent.isOnNavMesh)
            {
                _navAgent.transform.position = transform.position;
                _navAgent.SetDestination(destination);

                if (_navAgent.hasPath && _navAgent.path.corners.Length > 1)
                {
                    Vector3 nextCorner = _navAgent.path.corners[1];
                    Vector3 dir = (nextCorner - transform.position);
                    dir.y = 0f;
                    _motor.SetMoveInput(dir.normalized);
                    return;
                }
            }

            // Fallback direct movement
            Vector3 directDir = (destination - transform.position);
            directDir.y = 0f;
            _motor.SetMoveInput(directDir.normalized);
        }

        public void SetState(AIState newState)
        {
            _currentState = newState;
        }
    }
}
