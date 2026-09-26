using System;
using UnityEngine;

namespace DexHigh.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class DragonMotor : MonoBehaviour
    {
        [Header("Movement Stats")]
        [SerializeField] private float _moveSpeed = 6.5f;
        [SerializeField] private float _rotationSpeed = 12f;
        [SerializeField] private float _gravity = 20f;

        [Header("Knockback Settings")]
        [SerializeField] private float _knockbackDamping = 6f;

        [Header("Flight Settings")]
        [SerializeField] private float _baseFlightAltitude = 1.8f;
        [SerializeField] private float _flightAscendSpeed = 8f;

        private CharacterController _characterController;
        private Vector3 _moveDirection;
        private Vector3 _lookDirection;
        private Vector3 _knockbackVelocity;
        private float _verticalVelocity;
        private float _targetFlightAltitude;
        private float _currentFlightAltitude;
        private float _speedMultiplier = 1f;
        private bool _isMovementLocked;
        private bool _isRotationLocked;

        public CharacterController CharacterController => _characterController;
        public float MoveSpeed => _moveSpeed * _speedMultiplier;
        public bool IsMoving => _moveDirection.sqrMagnitude > 0.01f;
        public Vector3 Velocity => _characterController != null ? _characterController.velocity : Vector3.zero;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _lookDirection = transform.forward;
            _targetFlightAltitude = _baseFlightAltitude;
            _currentFlightAltitude = _baseFlightAltitude;
        }

        public void SetMoveInput(Vector3 direction)
        {
            if (_isMovementLocked)
            {
                _moveDirection = Vector3.zero;
                return;
            }

            direction.y = 0f;
            _moveDirection = Vector3.ClampMagnitude(direction, 1f);
        }

        public void SetLookDirection(Vector3 direction)
        {
            if (_isRotationLocked) return;

            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                _lookDirection = direction.normalized;
            }
        }

        public void SetLookTarget(Vector3 targetPosition)
        {
            Vector3 dir = targetPosition - transform.position;
            SetLookDirection(dir);
        }

        public void SnapToLookTarget(Vector3 targetPosition)
        {
            if (_isRotationLocked) return;
            Vector3 dir = targetPosition - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                _lookDirection = dir.normalized;
                transform.rotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
            }
        }

        public void SetMovementLocked(bool locked)
        {
            _isMovementLocked = locked;
            if (locked) _moveDirection = Vector3.zero;
        }

        public void SetRotationLocked(bool locked)
        {
            _isRotationLocked = locked;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = Mathf.Max(0f, multiplier);
        }

        public void SetFlightAltitude(float altitude)
        {
            _targetFlightAltitude = altitude;
        }

        public void ApplyKnockback(Vector3 direction, float force)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                _knockbackVelocity += direction.normalized * force;
            }
        }

        public void ResetMotor()
        {
            _moveDirection = Vector3.zero;
            _knockbackVelocity = Vector3.zero;
            _verticalVelocity = 0f;
            _targetFlightAltitude = 0f;
            _currentFlightAltitude = 0f;
            _speedMultiplier = 1f;
            _isMovementLocked = false;
            _isRotationLocked = false;
        }

        private void Update()
        {
            UpdateRotation();
            UpdateMovement();
        }

        private void UpdateRotation()
        {
            if (_isRotationLocked || _lookDirection.sqrMagnitude <= 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private void UpdateMovement()
        {
            if (_characterController == null || !_characterController.enabled) return;

            Vector3 finalMove = _moveDirection * (_moveSpeed * _speedMultiplier);

            // Apply knockback decay
            if (_knockbackVelocity.sqrMagnitude > 0.01f)
            {
                finalMove += _knockbackVelocity;
                _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, _knockbackDamping * Time.deltaTime);
            }

            // Altitude adjustment for flight
            _currentFlightAltitude = Mathf.MoveTowards(_currentFlightAltitude, _targetFlightAltitude, _flightAscendSpeed * Time.deltaTime);

            if (_targetFlightAltitude > 0.01f || _currentFlightAltitude > 0.01f)
            {
                _verticalVelocity = (_targetFlightAltitude - _currentFlightAltitude) * _flightAscendSpeed;
            }
            else
            {
                if (_characterController.isGrounded)
                {
                    _verticalVelocity = -1f;
                }
                else
                {
                    _verticalVelocity -= _gravity * Time.deltaTime;
                }
            }

            finalMove.y = _verticalVelocity;
            _characterController.Move(finalMove * Time.deltaTime);
        }
    }
}
