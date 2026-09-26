using DexHigh.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace DexHigh.UI
{
    public class WorldHealthBar : MonoBehaviour
    {
        [Header("Target & Components")]
        [SerializeField] private DragonHealth _health;
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private Image _healthEaseImage;
        [SerializeField] private float _easeSpeed = 3f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 3.5f, 0f);

        private UnityEngine.Camera _mainCamera;
        private Transform _targetTransform;

        private void Awake()
        {
            if (_health == null) _health = GetComponentInParent<DragonHealth>();
            _mainCamera = UnityEngine.Camera.main;
            if (_health != null) _targetTransform = _health.transform;
        }

        private void Start()
        {
            if (_health != null)
            {
                _health.OnHealthChanged += HandleHealthChanged;
                _health.OnDied += _ => gameObject.SetActive(false);
                _health.OnRevived += () => gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnHealthChanged -= HandleHealthChanged;
            }
        }

        private void LateUpdate()
        {
            if (_mainCamera == null) _mainCamera = UnityEngine.Camera.main;

            if (_targetTransform != null)
            {
                transform.position = _targetTransform.position + _offset;
            }

            if (_mainCamera != null)
            {
                transform.rotation = _mainCamera.transform.rotation;
            }

            if (_healthEaseImage != null && _healthFillImage != null)
            {
                if (_healthEaseImage.fillAmount > _healthFillImage.fillAmount)
                {
                    _healthEaseImage.fillAmount = Mathf.MoveTowards(_healthEaseImage.fillAmount, _healthFillImage.fillAmount, _easeSpeed * Time.deltaTime);
                }
                else
                {
                    _healthEaseImage.fillAmount = _healthFillImage.fillAmount;
                }
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            if (_healthFillImage != null && max > 0f)
            {
                _healthFillImage.fillAmount = Mathf.Clamp01(current / max);
            }
        }
    }
}
