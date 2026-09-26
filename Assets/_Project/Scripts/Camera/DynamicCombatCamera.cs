using UnityEngine;

namespace DexHigh.CombatCamera
{
    public class DynamicCombatCamera : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _aiTransform;

        [Header("Camera Framing")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 14f, -10f);
        [SerializeField] private float _minDistance = 8f;
        [SerializeField] private float _maxDistance = 22f;
        [SerializeField] private float _minHeight = 10f;
        [SerializeField] private float _maxHeight = 18f;
        [SerializeField] private float _smoothTime = 0.2f;

        [Header("Screen Shake")]
        [SerializeField] private float _shakeDamping = 5f;

        private Vector3 _currentVelocity;
        private Vector3 _shakeOffset;
        private float _shakeMagnitude;

        private void Start()
        {
            FindTargetsIfNull();
        }

        public void FindTargetsIfNull()
        {
            if (_playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) _playerTransform = player.transform;
            }

            if (_aiTransform == null)
            {
                var ai = GameObject.FindWithTag("Enemy");
                if (ai != null) _aiTransform = ai.transform;
            }
        }

        public void SetTargets(Transform player, Transform ai)
        {
            _playerTransform = player;
            _aiTransform = ai;
        }

        public void TriggerScreenShake(float magnitude = 0.35f, float duration = 0.25f)
        {
            _shakeMagnitude = magnitude;
        }

        private void LateUpdate()
        {
            if (_playerTransform == null && _aiTransform == null)
            {
                FindTargetsIfNull();
                if (_playerTransform == null && _aiTransform == null) return;
            }

            Vector3 midpoint;
            float targetsDistance = 0f;

            if (_playerTransform != null && _aiTransform != null)
            {
                midpoint = (_playerTransform.position + _aiTransform.position) * 0.5f;
                targetsDistance = Vector3.Distance(_playerTransform.position, _aiTransform.position);
            }
            else if (_playerTransform != null)
            {
                midpoint = _playerTransform.position;
            }
            else
            {
                midpoint = _aiTransform.position;
            }

            // Calculate height and depth distance scaling based on targets separation
            float distFactor = Mathf.InverseLerp(0f, 20f, targetsDistance);
            float desiredHeight = Mathf.Lerp(_minHeight, _maxHeight, distFactor);
            float desiredDepth = Mathf.Lerp(_minDistance, _maxDistance, distFactor);

            Vector3 targetPosition = midpoint + new Vector3(_offset.x, desiredHeight, -desiredDepth);

            // Screen shake
            if (_shakeMagnitude > 0.01f)
            {
                _shakeOffset = Random.insideUnitSphere * _shakeMagnitude;
                _shakeOffset.z = 0f;
                _shakeMagnitude = Mathf.MoveTowards(_shakeMagnitude, 0f, _shakeDamping * Time.deltaTime);
            }
            else
            {
                _shakeOffset = Vector3.zero;
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + _shakeOffset, ref _currentVelocity, _smoothTime);
            transform.LookAt(midpoint + Vector3.up * 1.5f);
        }
    }
}
