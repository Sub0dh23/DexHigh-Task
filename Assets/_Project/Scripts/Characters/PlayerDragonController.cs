using DexHigh.Combat;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace DexHigh.Characters
{
    [RequireComponent(typeof(DragonMotor))]
    [RequireComponent(typeof(DragonCombat))]
    [RequireComponent(typeof(DragonHealth))]
    public class PlayerDragonController : MonoBehaviour
    {
        [Header("Camera & Aiming")]
        [SerializeField] private UnityEngine.Camera _mainCamera;
        [SerializeField] private LayerMask _groundLayer = ~0;

        private DragonMotor _motor;
        private DragonCombat _combat;
        private DragonHealth _health;
        private Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);

        public Vector3 CurrentAimPoint { get; private set; }

        private void Awake()
        {
            _motor = GetComponent<DragonMotor>();
            _combat = GetComponent<DragonCombat>();
            _health = GetComponent<DragonHealth>();

            if (_mainCamera == null)
            {
                _mainCamera = UnityEngine.Camera.main;
            }
            CurrentAimPoint = transform.position + transform.forward * 10f;
        }

        private void Update()
        {
            if (_health != null && !_health.IsAlive) return;

            if (DexHigh.Core.BattleGameManager.Instance != null &&
                DexHigh.Core.BattleGameManager.Instance.CurrentState != DexHigh.Core.BattleState.Battle)
            {
                _motor.SetMoveInput(Vector3.zero);
                return;
            }

            HandleMovementInput();
            HandleAiming();
            HandleAbilitiesInput();
        }

        private void HandleMovementInput()
        {
            Vector3 moveInput = Vector3.zero;

#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                float h = 0f;
                float v = 0f;

                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;

                moveInput = new Vector3(h, 0f, v).normalized;
            }
#else
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveInput = new Vector3(h, 0f, v).normalized;
#endif

            _motor.SetMoveInput(moveInput);
        }

        private void HandleAiming()
        {
            if (_mainCamera == null)
            {
                _mainCamera = UnityEngine.Camera.main;
                if (_mainCamera == null) return;
            }

            Vector2 mouseScreenPos = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            if (mouse != null)
            {
                mouseScreenPos = mouse.position.ReadValue();
            }
#else
            mouseScreenPos = Input.mousePosition;
#endif

            if (mouseScreenPos.sqrMagnitude < 0.01f) return;

            Ray ray = _mainCamera.ScreenPointToRay(mouseScreenPos);

            // Ground combat plane at arena floor height
            Plane aimPlane = new Plane(Vector3.up, Vector3.zero);
            if (aimPlane.Raycast(ray, out float enter) && enter > 0f)
            {
                Vector3 worldHit = ray.GetPoint(enter);
                // Clamp aim strictly inside circular arena floor
                Vector2 horiz = new Vector2(worldHit.x, worldHit.z);
                float maxAimRadius = 18.2f;
                if (horiz.sqrMagnitude > maxAimRadius * maxAimRadius)
                {
                    horiz = horiz.normalized * maxAimRadius;
                    worldHit.x = horiz.x;
                    worldHit.z = horiz.y;
                }
                worldHit.y = 0f;
                CurrentAimPoint = worldHit;

                Vector3 toHit = worldHit - transform.position;
                toHit.y = 0f;

                if (toHit.sqrMagnitude >= 0.5f * 0.5f)
                {
                    _motor.SetLookTarget(CurrentAimPoint);
                }
            }
        }

        private void HandleAbilitiesInput()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                // Q: Ability 0 (Fire Breath)
                if (kb.qKey.wasPressedThisFrame || kb.digit1Key.wasPressedThisFrame)
                {
                    _combat.TryCastAbility(0, CurrentAimPoint);
                }
                // V: Ability 1 (Tail Whip)
                if (kb.vKey.wasPressedThisFrame || kb.digit2Key.wasPressedThisFrame)
                {
                    _combat.TryCastAbility(1, CurrentAimPoint);
                }
                // E: Ability 2 (Sky Dive Slam)
                if (kb.eKey.wasPressedThisFrame || kb.digit3Key.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
                {
                    _combat.TryCastAbility(2, CurrentAimPoint);
                }
            }
#else
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                _combat.TryCastAbility(0, CurrentAimPoint);
            }
            if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                _combat.TryCastAbility(1, CurrentAimPoint);
            }
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Space))
            {
                _combat.TryCastAbility(2, CurrentAimPoint);
            }
#endif
        }
    }
}
