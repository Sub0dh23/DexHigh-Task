using UnityEngine;
using DexHigh.Characters;
using DexHigh.Combat;

namespace DexHigh.UI
{
    /// <summary>
    /// Circular impact cursor (Tactical Caliper) that follows mouse ground aim
    /// and dynamically communicates attack concentration, target lock, and impact pulses.
    /// </summary>
    public class CircularImpactCursor : MonoBehaviour
    {
        [Header("Targeting & Height")]
        [SerializeField] private PlayerDragonController _playerController;
        [SerializeField] private float _groundYOffset = 0.06f;
        [SerializeField] private float _smoothFollowSpeed = 28f;
        [SerializeField] private float _defaultScale = 1.2f;

        [Header("Visual Components (Layered Reticle)")]
        [SerializeField] private SpriteRenderer _outerTicksRenderer;
        [SerializeField] private SpriteRenderer _midChevronsRenderer;
        [SerializeField] private SpriteRenderer _innerConcentrationRenderer;
        [SerializeField] private SpriteRenderer _centerFocalRenderer;

        [Header("Rotation & Breathing Dynamics")]
        [SerializeField] private float _outerRotationSpeed = -12f;
        [SerializeField] private float _midRotationSpeed = 16f;
        [SerializeField] private float _breathingFrequency = 2.4f;
        [SerializeField] private float _breathingAmplitude = 0.05f;

        [Header("Attack Concentration Feedback")]
        [SerializeField] private float _concentrationLockRadius = 1.1f;
        [SerializeField] private Color _neutralColor = new Color(1.0f, 0.55f, 0.12f, 0.9f);
        [SerializeField] private Color _lockedColor = new Color(1.0f, 0.18f, 0.08f, 1.0f);
        [SerializeField] private Color _chargingColor = new Color(1.0f, 0.85f, 0.25f, 1.0f);

        [Header("Target Detection")]
        [SerializeField] private LayerMask _targetLayers = ~0;

        private float _pulseTimer = 0f;
        private float _impactPunch = 0f;
        private bool _isTargetLocked = false;
        private bool _isPinned = false;
        private Vector3 _pinnedPosition;
        private Vector3 _currentPosition;
        private DragonCombat _playerCombat;

        private void Awake()
        {
            if (_playerController == null)
            {
#if UNITY_2023_1_OR_NEWER
                _playerController = FindFirstObjectByType<PlayerDragonController>();
#else
                _playerController = FindObjectOfType<PlayerDragonController>();
#endif
            }

            if (_playerController != null)
            {
                _playerCombat = _playerController.GetComponent<DragonCombat>();
            }

            _currentPosition = transform.position;
        }

        private void OnEnable()
        {
            if (_playerCombat != null)
            {
                _playerCombat.OnAbilityCastStarted += HandleAbilityCast;
                _playerCombat.OnAbilityImpact += HandleAbilityImpact;
            }
        }

        private void OnDisable()
        {
            if (_playerCombat != null)
            {
                _playerCombat.OnAbilityCastStarted -= HandleAbilityCast;
                _playerCombat.OnAbilityImpact -= HandleAbilityImpact;
            }
        }

        private void Start()
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            transform.localScale = Vector3.one * _defaultScale;
        }

        private void Update()
        {
            UpdatePosition();
            UpdateLayersRotationAndBreathing();
            UpdateConcentrationState();
        }

        public void PinToPosition(Vector3 pinPos)
        {
            _isPinned = true;
            _pinnedPosition = pinPos;
            _pinnedPosition.y = _groundYOffset;
            _currentPosition = _pinnedPosition;
            transform.position = _currentPosition;
            TriggerImpactPunch(0.5f);
        }

        public void ReleasePin()
        {
            _isPinned = false;
        }

        private void UpdatePosition()
        {
            if (_isPinned)
            {
                transform.position = _pinnedPosition;
                return;
            }

            Vector3 targetAim = transform.position;
            if (_playerController != null)
            {
                targetAim = _playerController.CurrentAimPoint;
            }

            targetAim.y = _groundYOffset;

            // Snappy interpolation towards mouse aim
            if (_smoothFollowSpeed > 0f)
            {
                _currentPosition = Vector3.Lerp(_currentPosition, targetAim, Time.deltaTime * _smoothFollowSpeed);
            }
            else
            {
                _currentPosition = targetAim;
            }

            transform.position = _currentPosition;
        }

        private void UpdateLayersRotationAndBreathing()
        {
            // 1. Independent Layer Rotations
            if (_outerTicksRenderer != null)
            {
                _outerTicksRenderer.transform.Rotate(0f, 0f, _outerRotationSpeed * Time.deltaTime, Space.Self);
            }

            if (_midChevronsRenderer != null)
            {
                _midChevronsRenderer.transform.Rotate(0f, 0f, _midRotationSpeed * Time.deltaTime, Space.Self);
            }

            // 2. Inner Concentration Teeth Breathing
            _pulseTimer += Time.deltaTime * _breathingFrequency;
            float breatheScale = 1f + Mathf.Sin(_pulseTimer) * _breathingAmplitude;

            // 3. Impact Punch Decay
            if (_impactPunch > 0.001f)
            {
                _impactPunch = Mathf.MoveTowards(_impactPunch, 0f, Time.deltaTime * 3.5f);
            }

            float finalInnerScale = breatheScale + _impactPunch;

            if (_innerConcentrationRenderer != null)
            {
                _innerConcentrationRenderer.transform.localScale = new Vector3(finalInnerScale, finalInnerScale, 1f);
            }

            // Center focal core subtle twitch/scale
            if (_centerFocalRenderer != null)
            {
                float focalScale = 1f + (_isTargetLocked ? 0.25f : 0f) + (_impactPunch * 0.5f);
                _centerFocalRenderer.transform.localScale = new Vector3(focalScale, focalScale, 1f);
            }
        }

        private void UpdateConcentrationState()
        {
            // Check for enemies within concentration focal zone
            _isTargetLocked = false;
            Collider[] hits = Physics.OverlapSphere(transform.position, _concentrationLockRadius, _targetLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit.transform.root == (_playerController != null ? _playerController.transform.root : null))
                    continue; // Skip self

                var health = hit.GetComponentInParent<DexHigh.Characters.DragonHealth>();
                if (health != null && health.IsAlive)
                {
                    _isTargetLocked = true;
                    break;
                }
            }

            // Interpolate colors based on concentration status
            Color targetCol = _isTargetLocked ? _lockedColor : _neutralColor;
            if (_impactPunch > 0.1f)
            {
                targetCol = Color.Lerp(targetCol, _chargingColor, _impactPunch);
            }

            ApplyColor(targetCol);
        }

        private void ApplyColor(Color col)
        {
            if (_innerConcentrationRenderer != null)
            {
                _innerConcentrationRenderer.color = col;
            }

            if (_midChevronsRenderer != null)
            {
                Color midCol = col;
                midCol.a = _isTargetLocked ? 1.0f : 0.85f;
                _midChevronsRenderer.color = midCol;
            }

            if (_outerTicksRenderer != null)
            {
                Color outerCol = col;
                outerCol.a = _isTargetLocked ? 0.9f : 0.65f;
                _outerTicksRenderer.color = outerCol;
            }

            if (_centerFocalRenderer != null)
            {
                Color centerCol = _isTargetLocked ? Color.white : col;
                _centerFocalRenderer.color = centerCol;
            }
        }

        public void TriggerImpactPunch(float intensity = 0.35f)
        {
            _impactPunch = intensity;
        }

        private void HandleAbilityCast(int slotIndex, AbilityData ability)
        {
            // Rapid concentration compression and flare
            TriggerImpactPunch(0.45f);
        }

        private void HandleAbilityImpact(AbilityData ability, Vector3 position, float radius)
        {
            // Impact confirmed on arena ground / target
            TriggerImpactPunch(0.6f);
        }
    }
}
