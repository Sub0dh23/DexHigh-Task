using DexHigh.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DexHigh.UI
{
    public class AbilitySlotUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _socketBorderImage;
        [SerializeField] private Image _cooldownRadialOverlay;
        [SerializeField] private TextMeshProUGUI _cooldownText;
        [SerializeField] private TextMeshProUGUI _hotkeyText;
        [SerializeField] private TextMeshProUGUI _abilityNameText;

        [Header("Socket Styling")]
        [SerializeField] private Sprite _readySocketSprite;
        [SerializeField] private Sprite _cooldownSocketSprite;

        public void Initialize(AbilityData data, string hotkey)
        {
            if (data == null) return;

            if (_iconImage != null && data.Icon != null)
            {
                _iconImage.sprite = data.Icon;
            }

            if (_hotkeyText != null)
            {
                _hotkeyText.text = hotkey;
            }

            if (_abilityNameText != null)
            {
                _abilityNameText.text = data.AbilityName;
            }

            SetCooldown(0f, data.Cooldown);
        }

        public void SetCooldown(float remaining, float total)
        {
            if (remaining > 0.05f)
            {
                if (_cooldownRadialOverlay != null)
                {
                    _cooldownRadialOverlay.fillAmount = remaining / total;
                    _cooldownRadialOverlay.gameObject.SetActive(true);
                }

                if (_cooldownText != null)
                {
                    _cooldownText.text = remaining > 1f ? remaining.ToString("F0") : remaining.ToString("F1");
                    _cooldownText.gameObject.SetActive(true);
                }

                if (_socketBorderImage != null && _cooldownSocketSprite != null)
                {
                    _socketBorderImage.sprite = _cooldownSocketSprite;
                }

                if (_iconImage != null)
                {
                    _iconImage.color = new Color(0.5f, 0.5f, 0.5f, 0.85f);
                }
            }
            else
            {
                if (_cooldownRadialOverlay != null)
                {
                    _cooldownRadialOverlay.fillAmount = 0f;
                    _cooldownRadialOverlay.gameObject.SetActive(false);
                }

                if (_cooldownText != null)
                {
                    _cooldownText.gameObject.SetActive(false);
                }

                if (_socketBorderImage != null && _readySocketSprite != null)
                {
                    _socketBorderImage.sprite = _readySocketSprite;
                }

                if (_iconImage != null)
                {
                    _iconImage.color = Color.white;
                }
            }
        }
    }
}
