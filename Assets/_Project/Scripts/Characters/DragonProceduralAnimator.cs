using DexHigh.Combat;
using UnityEngine;

namespace DexHigh.Characters
{
    /// <summary>
    /// Coordinates high-level character state (movement speed, combat abilities, hit reactions, death)
    /// with the Unity Animator controller driven by Blender-designed animation clips.
    /// </summary>
    public class DragonProceduralAnimator : MonoBehaviour
    {
        [Header("Animator Reference")]
        [SerializeField] private Animator _animator;

        private DragonMotor _motor;
        private DragonCombat _combat;
        private DragonHealth _health;

        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int TakeHitTrigger = Animator.StringToHash("TakeHit");
        private static readonly int DieTrigger = Animator.StringToHash("Die");
        private static readonly int IsDeadParam = Animator.StringToHash("IsDead");
        private static readonly int FireBreathTrigger = Animator.StringToHash("FireBreath");
        private static readonly int TailWhipTrigger = Animator.StringToHash("TailWhip");
        private static readonly int DiveBombTrigger = Animator.StringToHash("DiveBomb");

        private void Awake()
        {
            _motor = GetComponent<DragonMotor>();
            _combat = GetComponent<DragonCombat>();
            _health = GetComponent<DragonHealth>();

            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
        }

        private void Start()
        {
            if (_health != null)
            {
                _health.OnDamaged += HandleDamaged;
                _health.OnDied += HandleDied;
                _health.OnRevived += HandleRevived;
            }

            if (_combat != null)
            {
                _combat.OnAbilityCastStarted += HandleAbilityCastStarted;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDamaged -= HandleDamaged;
                _health.OnDied -= HandleDied;
                _health.OnRevived -= HandleRevived;
            }

            if (_combat != null)
            {
                _combat.OnAbilityCastStarted -= HandleAbilityCastStarted;
            }
        }

        private void Update()
        {
            if (_animator == null) return;

            // Drive forward/idle blend via velocity magnitude
            if (_motor != null && (_health == null || _health.IsAlive))
            {
                float currentSpeed = _motor.Velocity.magnitude;
                float normalizedSpeed = currentSpeed / Mathf.Max(1f, _motor.MoveSpeed);
                _animator.SetFloat(SpeedParam, normalizedSpeed);
            }
            else
            {
                _animator.SetFloat(SpeedParam, 0f);
            }
        }

        private void HandleAbilityCastStarted(int slotIndex, AbilityData ability)
        {
            if (_animator == null || ability == null) return;

            switch (ability.AbilityType)
            {
                case AbilityType.FireBreath:
                    _animator.SetTrigger(FireBreathTrigger);
                    break;
                case AbilityType.TailWhip:
                    _animator.SetTrigger(TailWhipTrigger);
                    break;
                case AbilityType.FlyDive:
                    _animator.SetTrigger(DiveBombTrigger);
                    break;
            }
        }

        private void HandleDamaged(DamageInfo damageInfo)
        {
            if (_animator != null && _health != null && _health.IsAlive)
            {
                _animator.SetTrigger(TakeHitTrigger);
            }
        }

        private void HandleDied(DamageInfo killerInfo)
        {
            if (_animator != null)
            {
                _animator.SetBool(IsDeadParam, true);
                _animator.SetTrigger(DieTrigger);
            }
        }

        private void HandleRevived()
        {
            if (_animator != null)
            {
                _animator.SetBool(IsDeadParam, false);
                _animator.Play("Fly_Idle", 0, 0f);
            }
        }
    }
}
