using DexHigh.Characters;
using DexHigh.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DexHigh.UI
{
    public class CombatHUD : MonoBehaviour
    {
        [Header("Player HUD (Top Left)")]
        [SerializeField] private DragonHealth _playerHealth;
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private Image _playerHealthFill;
        [SerializeField] private Image _playerHealthEase;
        [SerializeField] private TextMeshProUGUI _playerHealthValText;
        [SerializeField] private Image[] _playerHealthBlocks = new Image[8];
        [SerializeField] private Sprite _playerBlockActiveSprite;
        [SerializeField] private Sprite _playerBlockGhostSprite;
        [SerializeField] private Sprite _playerBlockDepletedSprite;

        [Header("AI HUD (Top Right)")]
        [SerializeField] private DragonHealth _aiHealth;
        [SerializeField] private TextMeshProUGUI _aiNameText;
        [SerializeField] private Image _aiHealthFill;
        [SerializeField] private Image _aiHealthEase;
        [SerializeField] private TextMeshProUGUI _aiHealthValText;
        [SerializeField] private Image[] _aiHealthBlocks = new Image[8];
        [SerializeField] private Sprite _aiBlockActiveSprite;
        [SerializeField] private Sprite _aiBlockGhostSprite;
        [SerializeField] private Sprite _aiBlockDepletedSprite;

        [Header("Player Abilities Bar")]
        [SerializeField] private DragonCombat _playerCombat;
        [SerializeField] private AbilitySlotUI[] _abilitySlots = new AbilitySlotUI[3];

        [Header("Feedback / Spawners")]
        [SerializeField] private GameObject _damageNumberPrefab;
        [SerializeField] private GameObject _hitSparksPrefab;

        [Header("Winner Screen")]
        [SerializeField] private GameObject _winnerScreenPanel;
        [SerializeField] private TextMeshProUGUI _winnerTitleText;
        [SerializeField] private TextMeshProUGUI _winnerSubtitleText;
        [SerializeField] private Image _winnerDragonIcon;
        [SerializeField] private TextMeshProUGUI _winnerOpponentNote;
        [SerializeField] private TextMeshProUGUI _statTimeText;
        [SerializeField] private TextMeshProUGUI _statDamageText;
        [SerializeField] private TextMeshProUGUI _statHealthText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _leaveButton;

        [Header("Settings")]
        [SerializeField] private float _easeSpeed = 2.5f;
        [SerializeField] private float _ghostDecayDelay = 0.45f;

        private Coroutine _playerGhostRoutine;
        private Coroutine _aiGhostRoutine;
        private int _playerLastActiveCount = 8;
        private int _aiLastActiveCount = 8;
        private float _battleStartTime;
        private float _totalPlayerDamageDealt;

        private void Start()
        {
            InitializeHUD();
        }

        public void InitializeHUD()
        {
            _battleStartTime = Time.time;
            _totalPlayerDamageDealt = 0f;

            if (_winnerScreenPanel != null)
            {
                _winnerScreenPanel.SetActive(false);
            }

            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveAllListeners();
                _restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (_leaveButton != null)
            {
                _leaveButton.onClick.RemoveAllListeners();
                _leaveButton.onClick.AddListener(OnLeaveClicked);
            }

            HookPlayer();
            HookAI();
        }

        private void HookPlayer()
        {
            if (_playerHealth == null)
            {
                var player = FindFirstObjectByType<PlayerDragonController>();
                if (player != null)
                {
                    _playerHealth = player.GetComponent<DragonHealth>();
                    _playerCombat = player.GetComponent<DragonCombat>();
                }
            }

            if (_playerHealth != null)
            {
                if (_playerNameText != null) _playerNameText.text = _playerHealth.CharacterName;
                _playerHealth.OnHealthChanged += UpdatePlayerHealthUI;
                _playerHealth.OnDamaged += SpawnDamageNumber;
                UpdatePlayerHealthUI(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            }

            if (_playerCombat != null)
            {
                _playerCombat.OnCooldownUpdated += UpdateAbilityCooldown;

                string[] hotkeys = { "Q", "V", "E" };
                for (int i = 0; i < _abilitySlots.Length; i++)
                {
                    if (i < _playerCombat.Abilities.Length && _playerCombat.Abilities[i] != null && _abilitySlots[i] != null)
                    {
                        _abilitySlots[i].Initialize(_playerCombat.Abilities[i], hotkeys[i]);
                    }
                }
            }
        }

        private void HookAI()
        {
            if (_aiHealth == null)
            {
                var ai = FindFirstObjectByType<AI.AIDragonController>();
                if (ai != null)
                {
                    _aiHealth = ai.GetComponent<DragonHealth>();
                }
            }

            if (_aiHealth != null)
            {
                if (_aiNameText != null) _aiNameText.text = _aiHealth.CharacterName;
                _aiHealth.OnHealthChanged += UpdateAIHealthUI;
                _aiHealth.OnDamaged += (DamageInfo info) =>
                {
                    _totalPlayerDamageDealt += info.Amount;
                    SpawnDamageNumber(info);
                };
                UpdateAIHealthUI(_aiHealth.CurrentHealth, _aiHealth.MaxHealth);
            }
        }

        private void Update()
        {
            UpdateEaseBars();
        }

        private void UpdateEaseBars()
        {
            if (_playerHealthEase != null && _playerHealthFill != null)
            {
                if (_playerHealthEase.fillAmount > _playerHealthFill.fillAmount)
                {
                    _playerHealthEase.fillAmount = Mathf.MoveTowards(_playerHealthEase.fillAmount, _playerHealthFill.fillAmount, _easeSpeed * Time.deltaTime);
                }
                else
                {
                    _playerHealthEase.fillAmount = _playerHealthFill.fillAmount;
                }
            }

            if (_aiHealthEase != null && _aiHealthFill != null)
            {
                if (_aiHealthEase.fillAmount > _aiHealthFill.fillAmount)
                {
                    _aiHealthEase.fillAmount = Mathf.MoveTowards(_aiHealthEase.fillAmount, _aiHealthFill.fillAmount, _easeSpeed * Time.deltaTime);
                }
                else
                {
                    _aiHealthEase.fillAmount = _aiHealthFill.fillAmount;
                }
            }
        }

        private void UpdatePlayerHealthUI(float current, float max)
        {
            if (_playerHealthFill != null && max > 0f)
            {
                _playerHealthFill.fillAmount = Mathf.Clamp01(current / max);
            }
            if (_playerHealthValText != null)
            {
                _playerHealthValText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
            UpdateSegmentedBlocks(
                _playerHealthBlocks,
                _playerBlockActiveSprite,
                _playerBlockGhostSprite,
                _playerBlockDepletedSprite,
                current,
                max,
                ref _playerLastActiveCount,
                ref _playerGhostRoutine
            );
        }

        private void UpdateAIHealthUI(float current, float max)
        {
            if (_aiHealthFill != null && max > 0f)
            {
                _aiHealthFill.fillAmount = Mathf.Clamp01(current / max);
            }
            if (_aiHealthValText != null)
            {
                _aiHealthValText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
            UpdateSegmentedBlocks(
                _aiHealthBlocks,
                _aiBlockActiveSprite,
                _aiBlockGhostSprite,
                _aiBlockDepletedSprite,
                current,
                max,
                ref _aiLastActiveCount,
                ref _aiGhostRoutine
            );
        }

        private void UpdateSegmentedBlocks(
            Image[] blocks,
            Sprite activeSprite,
            Sprite ghostSprite,
            Sprite depletedSprite,
            float current,
            float max,
            ref int lastActiveCount,
            ref Coroutine ghostRoutine)
        {
            if (blocks == null || blocks.Length == 0 || max <= 0f) return;

            float ratio = Mathf.Clamp01(current / max);
            int newActiveCount = Mathf.CeilToInt(ratio * blocks.Length);

            if (ghostRoutine != null)
            {
                StopCoroutine(ghostRoutine);
                ghostRoutine = null;
            }

            int prevCount = lastActiveCount;
            lastActiveCount = newActiveCount;

            for (int i = 0; i < newActiveCount; i++)
            {
                if (blocks[i] != null && activeSprite != null)
                {
                    blocks[i].sprite = activeSprite;
                    blocks[i].color = Color.white;
                }
            }

            if (newActiveCount < prevCount)
            {
                for (int i = newActiveCount; i < prevCount; i++)
                {
                    if (blocks[i] != null && ghostSprite != null)
                    {
                        blocks[i].sprite = ghostSprite;
                        blocks[i].color = Color.white;
                    }
                }
                for (int i = prevCount; i < blocks.Length; i++)
                {
                    if (blocks[i] != null && depletedSprite != null)
                    {
                        blocks[i].sprite = depletedSprite;
                        blocks[i].color = Color.white;
                    }
                }
                ghostRoutine = StartCoroutine(GhostDecayRoutine(blocks, depletedSprite, newActiveCount, prevCount));
            }
            else
            {
                for (int i = newActiveCount; i < blocks.Length; i++)
                {
                    if (blocks[i] != null && depletedSprite != null)
                    {
                        blocks[i].sprite = depletedSprite;
                        blocks[i].color = Color.white;
                    }
                }
            }
        }

        private System.Collections.IEnumerator GhostDecayRoutine(Image[] blocks, Sprite depletedSprite, int startIndex, int endIndex)
        {
            yield return new WaitForSeconds(_ghostDecayDelay);
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i >= 0 && i < blocks.Length && blocks[i] != null && depletedSprite != null)
                {
                    blocks[i].sprite = depletedSprite;
                }
            }
        }

        private void UpdateAbilityCooldown(int slotIndex, float remaining, float total)
        {
            if (slotIndex >= 0 && slotIndex < _abilitySlots.Length && _abilitySlots[slotIndex] != null)
            {
                _abilitySlots[slotIndex].SetCooldown(remaining, total);
            }
        }

        private void SpawnDamageNumber(DamageInfo info)
        {
            if (_damageNumberPrefab != null)
            {
                Vector3 spawnPos = info.HitPoint + Vector3.up * 1.5f;
                var popupObj = Instantiate(_damageNumberPrefab, spawnPos, Quaternion.identity);
                if (popupObj.TryGetComponent<DamageNumberPopup>(out var popup))
                {
                    popup.Setup(info);
                }
            }

            // Directional Hit Sparks (for direct physical/fire/impact hits, not DoT ticks)
            if (_hitSparksPrefab != null && info.Type != DamageType.Burn)
            {
                Vector3 sparkDir = info.HitDirection.sqrMagnitude > 0.001f ? info.HitDirection : Vector3.up;
                Instantiate(_hitSparksPrefab, info.HitPoint, Quaternion.LookRotation(sparkDir));
            }

            // Screen shake on heavy impacts and critical focal hits
            if (info.IsCritical || info.Type == DamageType.Impact)
            {
                var cam = FindFirstObjectByType<DexHigh.CombatCamera.DynamicCombatCamera>();
                if (cam != null)
                {
                    float mag = info.IsCritical ? 0.35f : 0.22f;
                    cam.TriggerScreenShake(mag, 0.25f);
                }
            }
        }

        public void ShowWinnerScreen(string winnerName, bool isPlayerWinner)
        {
            if (_winnerScreenPanel != null)
            {
                _winnerScreenPanel.SetActive(true);

                if (_winnerTitleText != null)
                {
                    _winnerTitleText.text = isPlayerWinner ? "VICTORY" : "DEFEAT";
                    _winnerTitleText.color = isPlayerWinner ? new Color(1f, 0.85f, 0.25f) : new Color(0.9f, 0.25f, 0.25f);
                }

                if (_winnerSubtitleText != null)
                {
                    _winnerSubtitleText.text = isPlayerWinner ? "INFERNO WYRM ASCENDANT" : "FROST WYRM CLAIMS SUPREMACY";
                }

                if (_winnerOpponentNote != null)
                {
                    _winnerOpponentNote.text = isPlayerWinner ? "Frost Wyrm Vanquished in Arena Combat" : "Inferno Wyrm Vanquished in Combat";
                }

                if (_statTimeText != null)
                {
                    float elapsed = Mathf.Max(0f, Time.time - _battleStartTime);
                    int minutes = Mathf.FloorToInt(elapsed / 60f);
                    int seconds = Mathf.FloorToInt(elapsed % 60f);
                    _statTimeText.text = $"{minutes:00}:{seconds:00}";
                }

                if (_statDamageText != null)
                {
                    _statDamageText.text = Mathf.RoundToInt(_totalPlayerDamageDealt).ToString("N0");
                }

                if (_statHealthText != null)
                {
                    float hpPercent = (_playerHealth != null && _playerHealth.MaxHealth > 0)
                        ? Mathf.Clamp01(_playerHealth.CurrentHealth / _playerHealth.MaxHealth) * 100f
                        : 0f;
                    _statHealthText.text = $"{Mathf.RoundToInt(hpPercent)}%";
                }
            }
        }

        private void OnRestartClicked()
        {
            Core.BattleGameManager.Instance?.RestartBattle();
        }

        private void OnLeaveClicked()
        {
            Core.BattleGameManager.Instance?.RestartBattle();
        }
    }
}
