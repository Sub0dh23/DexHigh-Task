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
        MainMenu,
        Warmup,
        Battle,
        Ended
    }

    public class BattleGameManager : MonoBehaviour
    {
        private static BattleGameManager _instance;
        public static BattleGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<BattleGameManager>(FindObjectsInactive.Include);
                }
                return _instance;
            }
        }
        public static bool StartInBattleOnLoad = false;

        [Header("Combatants")]
        [SerializeField] private DragonHealth _playerDragon;
        [SerializeField] private DragonHealth _aiDragon;

        [Header("Spawn Points")]
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _aiSpawnPoint;

        [Header("UI & Camera")]
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private CombatHUD _combatHUD;
        [SerializeField] private CombatCamera.DynamicCombatCamera _combatCamera;

        [Header("Battle Settings")]
        [SerializeField] private float _warmupDuration = 1f;

        private BattleState _currentState = BattleState.MainMenu;
        private Vector3 _playerDefaultPosition = new Vector3(-9f, 0f, 0f);
        private Quaternion _playerDefaultRotation = Quaternion.Euler(0f, 90f, 0f);
        private Vector3 _aiDefaultPosition = new Vector3(9f, 0f, 0f);
        private Quaternion _aiDefaultRotation = Quaternion.Euler(0f, 270f, 0f);

        public BattleState CurrentState => _currentState;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            CacheInitialSpawns();
        }

        private void Start()
        {
            FindReferencesIfNull();
            RegisterEvents();

            if (StartInBattleOnLoad)
            {
                StartInBattleOnLoad = false;
                if (_mainMenuUI != null) _mainMenuUI.HideMenu();
                if (_combatHUD != null) _combatHUD.gameObject.SetActive(true);
                StartCoroutine(StartBattleRoutine());
            }
            else
            {
                _currentState = BattleState.MainMenu;
                if (_combatHUD != null) _combatHUD.gameObject.SetActive(false);
                if (_mainMenuUI != null) _mainMenuUI.ShowMenu();
            }
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

            if (_mainMenuUI == null)
            {
                _mainMenuUI = FindFirstObjectByType<MainMenuUI>(FindObjectsInactive.Include);
            }

            CacheInitialSpawns();
        }

        private void CacheInitialSpawns()
        {
            if (_playerSpawnPoint != null)
            {
                _playerDefaultPosition = _playerSpawnPoint.position;
                _playerDefaultRotation = _playerSpawnPoint.rotation;
            }
            else if (_playerDragon != null)
            {
                _playerDefaultPosition = _playerDragon.transform.position;
                _playerDefaultRotation = _playerDragon.transform.rotation;
            }

            if (_aiSpawnPoint != null)
            {
                _aiDefaultPosition = _aiSpawnPoint.position;
                _aiDefaultRotation = _aiSpawnPoint.rotation;
            }
            else if (_aiDragon != null)
            {
                _aiDefaultPosition = _aiDragon.transform.position;
                _aiDefaultRotation = _aiDragon.transform.rotation;
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

        public void StartGameFromMenu()
        {
            ResetCombatants();

            if (_mainMenuUI != null)
            {
                _mainMenuUI.HideMenu();
            }

            if (_combatHUD != null)
            {
                _combatHUD.gameObject.SetActive(true);
                _combatHUD.InitializeHUD();
            }

            StartCoroutine(StartBattleRoutine());
        }

        public void ReturnToMainMenu()
        {
            StopAllCoroutines();
            _currentState = BattleState.MainMenu;

            if (_combatHUD != null)
            {
                _combatHUD.HideWinnerScreen();
                _combatHUD.gameObject.SetActive(false);
            }

            ResetCombatants();

            if (_mainMenuUI != null)
            {
                _mainMenuUI.ShowMenu();
            }
        }

        public void ResetCombatants()
        {
            if (_playerDragon != null)
            {
                _playerDragon.ResetHealth();

                Vector3 targetPos = _playerSpawnPoint != null ? _playerSpawnPoint.position : _playerDefaultPosition;
                Quaternion targetRot = _playerSpawnPoint != null ? _playerSpawnPoint.rotation : _playerDefaultRotation;

                if (_playerDragon.TryGetComponent<DragonMotor>(out var motorP))
                {
                    motorP.Teleport(targetPos, targetRot);
                }
                else
                {
                    if (_playerDragon.TryGetComponent<CharacterController>(out var ccP)) ccP.enabled = false;
                    _playerDragon.transform.position = targetPos;
                    _playerDragon.transform.rotation = targetRot;
                    if (ccP != null) ccP.enabled = true;
                }

                if (_playerDragon.TryGetComponent<DragonCombat>(out var combatP))
                {
                    combatP.ResetCombat();
                }
            }

            if (_aiDragon != null)
            {
                _aiDragon.ResetHealth();

                Vector3 targetPos = _aiSpawnPoint != null ? _aiSpawnPoint.position : _aiDefaultPosition;
                Quaternion targetRot = _aiSpawnPoint != null ? _aiSpawnPoint.rotation : _aiDefaultRotation;

                if (_aiDragon.TryGetComponent<DragonMotor>(out var motorAI))
                {
                    motorAI.Teleport(targetPos, targetRot);
                }
                else
                {
                    if (_aiDragon.TryGetComponent<CharacterController>(out var ccAI)) ccAI.enabled = false;
                    _aiDragon.transform.position = targetPos;
                    _aiDragon.transform.rotation = targetRot;
                    if (ccAI != null) ccAI.enabled = true;
                }

                if (_aiDragon.TryGetComponent<DragonCombat>(out var combatAI))
                {
                    combatAI.ResetCombat();
                }

                if (_aiDragon.TryGetComponent<AI.AIDragonController>(out var aiCtrl))
                {
                    aiCtrl.SetState(AI.AIState.Idle);
                }
            }
        }

        public void RestartBattle()
        {
            StartInBattleOnLoad = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
