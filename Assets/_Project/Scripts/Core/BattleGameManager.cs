using System.Collections;
using DexHigh.Characters;
using DexHigh.Combat;
using DexHigh.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DexHigh.Core
{
    public enum BattleState
    {
        Warmup,
        Battle,
        Ended
    }

    public class BattleGameManager : MonoBehaviour
    {
        public static BattleGameManager Instance { get; private set; }

        [Header("Combatants")]
        [SerializeField] private DragonHealth _playerDragon;
        [SerializeField] private DragonHealth _aiDragon;

        [Header("Spawn Points")]
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _aiSpawnPoint;

        [Header("UI & Camera")]
        [SerializeField] private CombatHUD _combatHUD;
        [SerializeField] private CombatCamera.DynamicCombatCamera _combatCamera;

        [Header("Battle Settings")]
        [SerializeField] private float _warmupDuration = 1f;

        private BattleState _currentState = BattleState.Warmup;

        public BattleState CurrentState => _currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            FindReferencesIfNull();
            RegisterEvents();
            StartCoroutine(StartBattleRoutine());
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void FindReferencesIfNull()
        {
            if (_playerDragon == null)
            {
                var player = FindFirstObjectByType<PlayerDragonController>();
                if (player != null) _playerDragon = player.GetComponent<DragonHealth>();
            }

            if (_aiDragon == null)
            {
                var ai = FindFirstObjectByType<AI.AIDragonController>();
                if (ai != null) _aiDragon = ai.GetComponent<DragonHealth>();
            }

            if (_combatHUD == null)
            {
                _combatHUD = FindFirstObjectByType<CombatHUD>();
            }

            if (_combatCamera == null)
            {
                _combatCamera = FindFirstObjectByType<CombatCamera.DynamicCombatCamera>();
            }
        }

        private void RegisterEvents()
        {
            if (_playerDragon != null)
            {
                _playerDragon.OnDied += HandlePlayerDeath;
                if (_playerDragon.TryGetComponent<DragonCombat>(out var playerCombat))
                {
                    playerCombat.OnAbilityImpact += HandleAbilityImpact;
                }
            }

            if (_aiDragon != null)
            {
                _aiDragon.OnDied += HandleAIDeath;
                if (_aiDragon.TryGetComponent<DragonCombat>(out var aiCombat))
                {
                    aiCombat.OnAbilityImpact += HandleAbilityImpact;
                }
            }
        }

        private void UnregisterEvents()
        {
            if (_playerDragon != null)
            {
                _playerDragon.OnDied -= HandlePlayerDeath;
                if (_playerDragon.TryGetComponent<DragonCombat>(out var playerCombat))
                {
                    playerCombat.OnAbilityImpact -= HandleAbilityImpact;
                }
            }

            if (_aiDragon != null)
            {
                _aiDragon.OnDied -= HandleAIDeath;
                if (_aiDragon.TryGetComponent<DragonCombat>(out var aiCombat))
                {
                    aiCombat.OnAbilityImpact -= HandleAbilityImpact;
                }
            }
        }

        private IEnumerator StartBattleRoutine()
        {
            _currentState = BattleState.Warmup;
            yield return new WaitForSeconds(_warmupDuration);
            _currentState = BattleState.Battle;
        }

        private void HandlePlayerDeath(DamageInfo killer)
        {
            if (_currentState == BattleState.Ended) return;
            _currentState = BattleState.Ended;

            string winnerName = _aiDragon != null ? _aiDragon.CharacterName : "AI Dragon";
            if (_combatHUD != null)
            {
                _combatHUD.ShowWinnerScreen(winnerName, isPlayerWinner: false);
            }
        }

        private void HandleAIDeath(DamageInfo killer)
        {
            if (_currentState == BattleState.Ended) return;
            _currentState = BattleState.Ended;

            string winnerName = _playerDragon != null ? _playerDragon.CharacterName : "Player Dragon";
            if (_combatHUD != null)
            {
                _combatHUD.ShowWinnerScreen(winnerName, isPlayerWinner: true);
            }
        }

        private void HandleAbilityImpact(AbilityData ability, Vector3 position, float radius)
        {
            if (ability.AbilityType == AbilityType.FlyDive && _combatCamera != null)
            {
                _combatCamera.TriggerScreenShake(0.5f, 0.35f);
            }
        }

        public void RestartBattle()
        {
            // Option 1: Reload active scene for complete clean state
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
