using UnityEngine;

namespace DexHigh.UI
{
    /// <summary>
    /// Animated ground telegraph for Sky Dive Slam.
    /// Features rotating outer calibrated rings, an inward-contracting countdown ring,
    /// and a pulsing danger core that flares white-hot before the dragon lands.
    /// </summary>
    public class DiveTargetIndicator : MonoBehaviour
    {
        [Header("Duration & Scale")]
        [SerializeField] private float _expectedDuration = 1.85f;
        [SerializeField] private float _scale = 0.7f;

        [Header("Layers")]
        [SerializeField] private SpriteRenderer _outerRing;
        [SerializeField] private SpriteRenderer _midChevrons;
        [SerializeField] private SpriteRenderer _contractingRing;
        [SerializeField] private SpriteRenderer _focalCore;

        [Header("Colors")]
        [SerializeField] private Color _startColor = new Color(1f, 0.55f, 0.1f, 0.85f);
        [SerializeField] private Color _apexColor = new Color(1f, 0.15f, 0.05f, 1f);
        [SerializeField] private Color _flashColor = new Color(1f, 0.95f, 0.8f, 1f);

        private float _elapsed = 0f;

        private void Awake()
        {
            var reticleShader = Shader.Find("DexHigh/CombatReticle");
            if (reticleShader == null) reticleShader = Shader.Find("DexHigh/CombatReticleAdditive");
            if (reticleShader == null) reticleShader = Shader.Find("Sprites/Default");

            foreach (var sr in GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (sr.sharedMaterial == null || sr.sharedMaterial.shader.name.Contains("2D"))
                {
                    if (reticleShader != null)
                    {
                        var mat = new Material(reticleShader);
                        mat.SetFloat("_SrcBlend", 5f); // SrcAlpha
                        mat.SetFloat("_DstBlend", 1f); // One (Additive)
                        mat.SetFloat("_Intensity", 1.8f);
                        mat.renderQueue = 3200;
                        sr.sharedMaterial = mat;
                    }
                }
                sr.sortingOrder = 25;
            }
        }

        private void Start()
        {
            Vector3 pos = transform.position;
            pos.y = 0.12f;
            transform.position = pos;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            transform.localScale = Vector3.one * _scale;
            ApplyScaleAndColor(0f);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(_elapsed / _expectedDuration);

            // 1. Independent layer rotations
            if (_outerRing != null)
            {
                _outerRing.transform.Rotate(0f, 0f, -25f * Time.deltaTime, Space.Self);
            }
            if (_midChevrons != null)
            {
                _midChevrons.transform.Rotate(0f, 0f, 35f * Time.deltaTime, Space.Self);
            }

            // 2. Contracting countdown ring: smoothly shrinks from full radius down to core
            if (_contractingRing != null)
            {
                float contractScale = Mathf.Lerp(1.05f, 0.12f, progress);
                _contractingRing.transform.localScale = new Vector3(contractScale, contractScale, 1f);
            }

            // 3. Focal core pulses faster as dive approaches
            if (_focalCore != null)
            {
                float freq = Mathf.Lerp(3f, 14f, progress);
                float pulse = 1f + Mathf.Sin(_elapsed * freq) * 0.15f * (0.5f + progress * 0.5f);
                _focalCore.transform.localScale = new Vector3(pulse, pulse, 1f);
            }

            ApplyScaleAndColor(progress);
        }

        private void ApplyScaleAndColor(float progress)
        {
            Color curColor = (progress > 0.88f) 
                ? Color.Lerp(_apexColor, _flashColor, (progress - 0.88f) / 0.12f)
                : Color.Lerp(_startColor, _apexColor, progress);

            if (_outerRing != null)
            {
                Color c = curColor;
                c.a = Mathf.Lerp(0.7f, 0.95f, progress);
                _outerRing.color = c;
            }

            if (_midChevrons != null)
            {
                Color c = curColor;
                c.a = Mathf.Lerp(0.8f, 1.0f, progress);
                _midChevrons.color = c;
            }

            if (_contractingRing != null)
            {
                Color c = curColor;
                c.a = 0.95f;
                _contractingRing.color = c;
            }

            if (_focalCore != null)
            {
                _focalCore.color = curColor;
            }
        }
    }
}
