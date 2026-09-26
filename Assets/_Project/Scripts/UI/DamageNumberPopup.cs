using System.Collections;
using DexHigh.Combat;
using TMPro;
using UnityEngine;

namespace DexHigh.UI
{
    public class DamageNumberPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _textMesh;
        [SerializeField] private float _floatSpeed = 2.2f;
        [SerializeField] private float _lifeTime = 0.9f;

        [Header("Damage Type Colors")]
        [SerializeField] private Color _fireColor = new Color(1f, 0.45f, 0.05f);
        [SerializeField] private Color _physicalColor = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private Color _impactColor = new Color(1f, 0.2f, 0.35f);
        [SerializeField] private Color _burnColor = new Color(1f, 0.6f, 0.1f);
        [SerializeField] private Color _critColor = new Color(1f, 0.15f, 0.15f);

        private UnityEngine.Camera _mainCamera;
        private bool _isCritical;
        private bool _isBurn;

        public void Setup(DamageInfo info)
        {
            if (_textMesh == null) _textMesh = GetComponent<TextMeshPro>();
            _mainCamera = UnityEngine.Camera.main;

            _isCritical = info.IsCritical;
            _isBurn = info.Type == DamageType.Burn;

            if (_textMesh != null)
            {
                int rounded = Mathf.Max(1, Mathf.RoundToInt(info.Amount));

                if (_isBurn)
                {
                    _textMesh.text = $"<size=80%>{rounded}</size>";
                    _textMesh.color = _burnColor;
                }
                else if (_isCritical)
                {
                    _textMesh.text = $"<size=125%><b>{rounded}</b></size>\n<size=65%><color=#FFE066>CRIT</color></size>";
                    _textMesh.color = _critColor;
                }
                else
                {
                    _textMesh.text = rounded.ToString();
                    switch (info.Type)
                    {
                        case DamageType.Fire:
                            _textMesh.color = _fireColor;
                            break;
                        case DamageType.Physical:
                            _textMesh.color = _physicalColor;
                            break;
                        case DamageType.Impact:
                            _textMesh.color = _impactColor;
                            break;
                        default:
                            _textMesh.color = _physicalColor;
                            break;
                    }
                }
            }

            StartCoroutine(AnimateRoutine());
        }

        private void LateUpdate()
        {
            if (_mainCamera == null) _mainCamera = UnityEngine.Camera.main;
            if (_mainCamera != null)
            {
                transform.rotation = _mainCamera.transform.rotation;
            }
        }

        private IEnumerator AnimateRoutine()
        {
            float elapsed = 0f;
            Vector3 startPos = transform.position + Random.insideUnitSphere * 0.2f;
            Vector3 initialScale = transform.localScale;

            // Lateral pop direction
            float horizontalDrift = Random.Range(-0.8f, 0.8f);
            float duration = _isBurn ? 0.7f : (_isCritical ? 1.1f : _lifeTime);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Parabolic trajectory (upwards + slight lateral drift)
                float upOffset = _floatSpeed * elapsed;
                float sideOffset = horizontalDrift * Mathf.Sin(t * Mathf.PI * 0.5f);
                transform.position = startPos + new Vector3(sideOffset, upOffset, 0f);

                // Punchy scale curve: pop up quickly, settle, then fade
                float scaleMult;
                if (_isCritical)
                {
                    // Overshoot punch
                    scaleMult = (t < 0.2f) ? Mathf.Lerp(0.5f, 1.4f, t / 0.2f) : Mathf.Lerp(1.4f, 1.0f, (t - 0.2f) / 0.8f);
                }
                else if (_isBurn)
                {
                    scaleMult = Mathf.Lerp(0.7f, 0.9f, t);
                }
                else
                {
                    scaleMult = (t < 0.25f) ? Mathf.Lerp(0.6f, 1.15f, t / 0.25f) : Mathf.Lerp(1.15f, 0.9f, (t - 0.25f) / 0.75f);
                }

                transform.localScale = initialScale * scaleMult;

                // Fade out towards end
                if (_textMesh != null)
                {
                    Color c = _textMesh.color;
                    c.a = (t > 0.65f) ? Mathf.Lerp(1f, 0f, (t - 0.65f) / 0.35f) : 1f;
                    _textMesh.color = c;
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
