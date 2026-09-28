using System;
using System.Collections;
using System.Collections.Generic;
using DexHigh.Characters;
using UnityEngine;

namespace DexHigh.Combat
{
    public class DragonCombat : MonoBehaviour
    {
        [Header("Ability Loadout")]
        [SerializeField] private AbilityData[] _abilities = new AbilityData[3];

        [Header("Attack Offsets & Transforms")]
        [SerializeField] private Transform _mouthTransform;
        [SerializeField] private Transform _tailTransform;
        [SerializeField] private Transform _groundIndicatorAnchor;

        [Header("Components")]
        [SerializeField] private DragonMotor _motor;
        [SerializeField] private DragonHealth _health;
        [SerializeField] private Animator _animator;

        private float[] _cooldownTimers = new float[3];
        private bool _isCasting;
        private Coroutine _activeCastCoroutine;

        public event Action<int, AbilityData> OnAbilityCastStarted;
        public event Action<int, float, float> OnCooldownUpdated; // slot, remaining, total
        public event Action<AbilityData, Vector3, float> OnAbilityImpact; // ability, position, radius

        public AbilityData[] Abilities => _abilities;
        public bool IsCasting => _isCasting;

        private static readonly int AnimSpeed = Animator.StringToHash("Speed");
        private static readonly int AnimAttackTrigger = Animator.StringToHash("Attack");
        private static readonly int AnimTakeoffTrigger = Animator.StringToHash("Takeoff");
        private static readonly int AnimLandTrigger = Animator.StringToHash("Land");
        private static readonly int AnimDieTrigger = Animator.StringToHash("Die");

        private void Awake()
        {
            if (_motor == null) _motor = GetComponent<DragonMotor>();
            if (_health == null) _health = GetComponent<DragonHealth>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            if (_health != null)
            {
                _health.OnDied += HandleDeath;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDeath;
            }
        }

        private void Update()
        {
            UpdateCooldowns();
            UpdateAnimator();
        }

        private void UpdateCooldowns()
        {
            for (int i = 0; i < _cooldownTimers.Length; i++)
            {
                if (_cooldownTimers[i] > 0f)
                {
                    _cooldownTimers[i] -= Time.deltaTime;
                    if (_cooldownTimers[i] < 0f) _cooldownTimers[i] = 0f;

                    if (_abilities != null && i < _abilities.Length && _abilities[i] != null)
                    {
                        OnCooldownUpdated?.Invoke(i, _cooldownTimers[i], _abilities[i].Cooldown);
                    }
                }
            }
        }

        private void UpdateAnimator()
        {
            if (_animator != null && _motor != null)
            {
                float speedPercent = _motor.Velocity.magnitude / Mathf.Max(0.1f, _motor.MoveSpeed);
                _animator.SetFloat(AnimSpeed, speedPercent);
            }
        }

        public bool CanCast(int slotIndex)
        {
            if (_health != null && !_health.IsAlive) return false;
            if (_isCasting) return false;
            if (slotIndex < 0 || slotIndex >= _abilities.Length) return false;
            if (_abilities[slotIndex] == null) return false;
            return _cooldownTimers[slotIndex] <= 0f;
        }

        public float GetCooldownRemaining(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _cooldownTimers.Length) return 0f;
            return _cooldownTimers[slotIndex];
        }

        public float GetCooldownNormalized(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _abilities.Length || _abilities[slotIndex] == null) return 0f;
            float total = _abilities[slotIndex].Cooldown;
            if (total <= 0f) return 0f;
            return Mathf.Clamp01(_cooldownTimers[slotIndex] / total);
        }

        public bool TryCastAbility(int slotIndex, Vector3 targetPoint)
        {
            if (!CanCast(slotIndex)) return false;

            AbilityData ability = _abilities[slotIndex];
            _cooldownTimers[slotIndex] = ability.Cooldown;
            OnCooldownUpdated?.Invoke(slotIndex, ability.Cooldown, ability.Cooldown);
            OnAbilityCastStarted?.Invoke(slotIndex, ability);

            switch (ability.AbilityType)
            {
                case AbilityType.FireBreath:
                    _activeCastCoroutine = StartCoroutine(CastFireBreathRoutine(ability, targetPoint));
                    break;
                case AbilityType.TailWhip:
                    _activeCastCoroutine = StartCoroutine(CastTailWhipRoutine(ability, targetPoint));
                    break;
                case AbilityType.FlyDive:
                    _activeCastCoroutine = StartCoroutine(CastFlyDiveRoutine(ability, targetPoint));
                    break;
            }

            return true;
        }

        private IEnumerator CastFireBreathRoutine(AbilityData ability, Vector3 targetPoint)
        {
            _isCasting = true;
            _motor.SnapToLookTarget(targetPoint);
            _motor.SetSpeedMultiplier(0.3f); // Slow movement during breath

            TriggerAnimation(ability.AnimationTrigger);

            Transform spawnPoint = _mouthTransform != null ? _mouthTransform : transform;
            GameObject vfx = null;
            if (ability.VfxPrefab != null)
            {
                // Align VFX flat with dragon forward vector, immune to 35-degree jaw bone tilt
                Quaternion flatRot = Quaternion.LookRotation(transform.forward, Vector3.up);
                vfx = Instantiate(ability.VfxPrefab, spawnPoint.position, flatRot, transform);
            }

            // Play Fire Breath SFX
            PlayAbilitySound(ability.CastAudioClip, AbilitySoundType.FireBreath);

            float duration = ability.CastDuration;
            float tickInterval = 0.2f;
            float elapsed = 0f;
            float damagePerTick = ability.BaseDamage / Mathf.Max(1f, (duration / tickInterval));

            while (elapsed < duration)
            {
                elapsed += tickInterval;

                if (vfx != null)
                {
                    vfx.transform.position = spawnPoint.position;
                    vfx.transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
                }

                ApplyConeDamage(spawnPoint.position, transform.forward, ability.EffectiveRange, 45f, damagePerTick, AbilityType.FireBreath, ability.KnockbackForce * 0.2f);
                yield return new WaitForSeconds(tickInterval);
            }

            if (vfx != null)
            {
                Destroy(vfx, 0.5f);
            }

            _motor.SetSpeedMultiplier(1f);
            _isCasting = false;
            _activeCastCoroutine = null;
        }

        private IEnumerator CastTailWhipRoutine(AbilityData ability, Vector3 targetPoint)
        {
            _isCasting = true;
            _motor.SnapToLookTarget(targetPoint);
            _motor.SetMovementLocked(true);

            TriggerAnimation(ability.AnimationTrigger);

            // Windup delay
            yield return new WaitForSeconds(0.25f);

            // Calculate origin: use tail transform if assigned, else position behind dragon center
            Vector3 originPos = _tailTransform != null ? _tailTransform.position : (transform.position - transform.forward * 1.5f);
            originPos.y = transform.position.y;

            if (ability.VfxPrefab != null)
            {
                Instantiate(ability.VfxPrefab, originPos, transform.rotation);
            }

            // Play Tail Whip SFX
            PlayAbilitySound(ability.CastAudioClip, AbilitySoundType.TailWhip);

            // Sweep melee area (around and behind/flank)
            ApplyAreaDamage(originPos, ability.EffectiveRange, ability.BaseDamage, AbilityType.TailWhip, ability.KnockbackForce, transform.forward);

            yield return new WaitForSeconds(Mathf.Max(0.1f, ability.CastDuration - 0.25f));

            _motor.SetMovementLocked(false);
            _isCasting = false;
            _activeCastCoroutine = null;
        }

        private IEnumerator CastFlyDiveRoutine(AbilityData ability, Vector3 targetPoint)
        {
            _isCasting = true;
            _motor.SnapToLookTarget(targetPoint);
            _motor.SetMovementLocked(true);
            _health.SetInvulnerable(true);

            TriggerAnimation("DiveBomb");

            // Lock the slam target coordinates immediately at initiation so subsequent mouse movement does not drift the target
            Vector3 aimOffset = targetPoint - transform.position;
            aimOffset.y = 0f;
            float maxRange = ability.EffectiveRange > 0f ? ability.EffectiveRange : 22f;
            Vector3 slamPosition = transform.position + Vector3.ClampMagnitude(aimOffset, maxRange);
            slamPosition.y = 0f;

            // Clamp slam target safely inside circular arena radius
            Vector2 slamHoriz = new Vector2(slamPosition.x, slamPosition.z);
            float maxSlamRadius = 18.5f;
            if (slamHoriz.sqrMagnitude > maxSlamRadius * maxSlamRadius)
            {
                slamHoriz = slamHoriz.normalized * maxSlamRadius;
                slamPosition = new Vector3(slamHoriz.x, 0f, slamHoriz.y);
            }

            // Visually pin the combat cursor to the exact target coordinates throughout the dive (Player only)
            DexHigh.UI.CircularImpactCursor impactCursor = null;
            if (GetComponent<DexHigh.Characters.PlayerDragonController>() != null)
            {
                impactCursor = FindFirstObjectByType<DexHigh.UI.CircularImpactCursor>();
                if (impactCursor != null)
                {
                    impactCursor.PinToPosition(slamPosition);
                }
            }

            // Spawn target indicator firmly at the locked impact coordinates
            GameObject indicator = null;
            if (ability.TargetIndicatorPrefab != null)
            {
                indicator = Instantiate(ability.TargetIndicatorPrefab, slamPosition, Quaternion.identity);
            }

            // 1. Takeoff & Ascend (Target indicator remains firmly pinned at initial aim mark)
            float takeoffHeight = 7f;
            _motor.SetFlightAltitude(takeoffHeight);
            float takeoffElapsed = 0f;
            float takeoffDuration = 0.8f;

            while (takeoffElapsed < takeoffDuration)
            {
                takeoffElapsed += Time.deltaTime;
                yield return null;
            }

            // 2. Move overhead toward slam position
            float hoverTime = 0.7f;
            float hoverElapsed = 0f;
            Vector3 startPos = transform.position;
            Vector3 airTargetPos = new Vector3(slamPosition.x, takeoffHeight, slamPosition.z);

            while (hoverElapsed < hoverTime)
            {
                hoverElapsed += Time.deltaTime;
                float t = hoverElapsed / hoverTime;
                transform.position = Vector3.Lerp(startPos, airTargetPos, t);
                _motor.SetLookTarget(slamPosition);
                yield return null;
            }

            // 3. Dive slam descent
            float diveDuration = 0.35f;
            float diveElapsed = 0f;
            Vector3 diveStartPos = transform.position;

            while (diveElapsed < diveDuration)
            {
                diveElapsed += Time.deltaTime;
                float t = diveElapsed / diveDuration;
                transform.position = Vector3.Lerp(diveStartPos, slamPosition, t * t); // Accelerating descent
                yield return null;
            }

            var cc = _motor.CharacterController;
            if (cc != null) cc.enabled = false;
            transform.position = new Vector3(slamPosition.x, 0.2f, slamPosition.z);
            if (cc != null) cc.enabled = true;

            _health.SetInvulnerable(false);

            if (indicator != null)
            {
                Destroy(indicator, 0.2f);
            }

            // 4. Impact shockwave & visual weight
            if (ability.ImpactVfxPrefab != null)
            {
                Instantiate(ability.ImpactVfxPrefab, slamPosition, Quaternion.identity);
            }

            // Play Heavy Impact SFX on dive slam
            PlayAbilitySound(ability.ImpactAudioClip, AbilitySoundType.FlyImpact);

            // Heavy screen shake on impact
            var cam = FindFirstObjectByType<DexHigh.CombatCamera.DynamicCombatCamera>();
            if (cam != null)
            {
                cam.TriggerScreenShake(0.5f, 0.35f);
            }

            // Procedural impact landing squash on dragon body
            StartCoroutine(ImpactLandingSquashRoutine());

            float innerRadius = ability.ImpactRadius;
            float outerRadius = ability.OuterImpactRadius;
            ApplyRadialDamage(slamPosition, innerRadius, outerRadius, ability.BaseDamage, AbilityType.FlyDive, ability.KnockbackForce);
            OnAbilityImpact?.Invoke(ability, slamPosition, outerRadius);

            yield return new WaitForSeconds(0.4f);

            // Re-ascend back to hovering flight altitude
            _motor.RestoreBaseFlightAltitude();

            // Release pinned combat cursor back to live mouse tracking once dive is fully completed
            if (impactCursor != null)
            {
                impactCursor.ReleasePin();
            }

            _motor.SetMovementLocked(false);
            _isCasting = false;
            _activeCastCoroutine = null;
        }

        private IEnumerator ImpactLandingSquashRoutine()
        {
            Vector3 origScale = transform.localScale;
            Vector3 squashScale = new Vector3(origScale.x * 1.25f, origScale.y * 0.7f, origScale.z * 1.25f);

            // Rapid squash compression on impact
            float elapsed = 0f;
            float squashDuration = 0.08f;
            while (elapsed < squashDuration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(origScale, squashScale, elapsed / squashDuration);
                yield return null;
            }

            // Elastic rebound back to normal scale
            elapsed = 0f;
            float reboundDuration = 0.22f;
            while (elapsed < reboundDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / reboundDuration);
                transform.localScale = Vector3.Lerp(squashScale, origScale, t);
                yield return null;
            }

            transform.localScale = origScale;
        }

        private void ApplyConeDamage(Vector3 origin, Vector3 forward, float range, float halfAngle, float damage, AbilityType abilityType, float knockback)
        {
            Collider[] hits = Physics.OverlapSphere(origin, range);
            foreach (var col in hits)
            {
                if (col.gameObject == gameObject) continue;

                if (col.TryGetComponent<IDamageable>(out var damageable) && damageable.IsAlive)
                {
                    Vector3 toTarget = (col.transform.position - origin);
                    toTarget.y = 0f;
                    float angle = Vector3.Angle(forward, toTarget);

                    if (angle <= halfAngle)
                    {
                        Vector3 hitDir = toTarget.normalized;
                        // Central sweetspot (inner 40% cone) deals critical damage
                        bool isFocal = angle <= halfAngle * 0.4f;
                        float finalDamage = isFocal ? damage * 1.4f : damage;

                        DamageInfo info = new DamageInfo(
                            amount: finalDamage,
                            abilitySource: abilityType,
                            attacker: gameObject,
                            hitPoint: col.transform.position,
                            hitDirection: hitDir,
                            knockbackForce: isFocal ? knockback * 1.3f : knockback,
                            damageType: DamageType.Fire,
                            isCritical: isFocal,
                            isFocalHit: isFocal
                        );

                        damageable.TakeDamage(info);

                        // Apply Burn DoT
                        if (col.TryGetComponent<DragonHealth>(out var targetHealth))
                        {
                            targetHealth.ApplyBurn(damagePerTick: 3.5f, duration: 2.0f, interval: 0.5f, attacker: gameObject);
                        }

                        if (col.TryGetComponent<DragonMotor>(out var targetMotor))
                        {
                            targetMotor.ApplyKnockback(hitDir, info.KnockbackForce);
                        }
                    }
                }
            }
        }

        private void ApplyAreaDamage(Vector3 center, float radius, float damage, AbilityType abilityType, float knockback, Vector3 defaultDir)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius);
            foreach (var col in hits)
            {
                if (col.gameObject == gameObject) continue;

                if (col.TryGetComponent<IDamageable>(out var damageable) && damageable.IsAlive)
                {
                    Vector3 offset = (col.transform.position - center);
                    offset.y = 0f;
                    Vector3 hitDir = offset.normalized;
                    if (hitDir.sqrMagnitude < 0.01f) hitDir = defaultDir;

                    float dist = offset.magnitude;
                    bool isFocal = dist <= radius * 0.5f;
                    float finalDamage = isFocal ? damage * 1.35f : damage;

                    DamageInfo info = new DamageInfo(
                        amount: finalDamage,
                        abilitySource: abilityType,
                        attacker: gameObject,
                        hitPoint: col.transform.position,
                        hitDirection: hitDir,
                        knockbackForce: isFocal ? knockback * 1.25f : knockback,
                        damageType: DamageType.Physical,
                        isCritical: isFocal,
                        isFocalHit: isFocal
                    );

                    damageable.TakeDamage(info);

                    if (col.TryGetComponent<DragonMotor>(out var targetMotor))
                    {
                        targetMotor.ApplyKnockback(hitDir, info.KnockbackForce);
                    }
                }
            }
        }

        private void ApplyRadialDamage(Vector3 center, float innerRadius, float outerRadius, float damage, AbilityType abilityType, float knockback)
        {
            Collider[] hits = Physics.OverlapSphere(center, outerRadius);
            foreach (var col in hits)
            {
                if (col.gameObject == gameObject) continue;

                if (col.TryGetComponent<IDamageable>(out var damageable) && damageable.IsAlive)
                {
                    Vector3 offset = (col.transform.position - center);
                    offset.y = 0f;
                    float dist = offset.magnitude;

                    // Outside 8m: strictly 0 damage
                    if (dist > outerRadius) continue;

                    Vector3 hitDir = offset.normalized;
                    if (hitDir.sqrMagnitude < 0.01f) hitDir = Vector3.forward;

                    float finalDamage;
                    bool isEpicenter;
                    float currentKnockback;

                    if (dist <= innerRadius)
                    {
                        // Inside 3.5m: Full epicenter critical damage (100% to 125%) + heavy knockback
                        float innerT = dist / Mathf.Max(0.1f, innerRadius);
                        float epicenterMult = Mathf.Lerp(1.25f, 1.0f, innerT);
                        finalDamage = damage * epicenterMult;
                        isEpicenter = true;
                        currentKnockback = knockback * 1.3f;
                    }
                    else
                    {
                        // Outside 3.5m to 8m: smoothly decreasing amount of damage (75% tapering down to 15%)
                        float falloffT = Mathf.Clamp01((dist - innerRadius) / Mathf.Max(0.1f, outerRadius - innerRadius));
                        float falloffMult = Mathf.Lerp(0.75f, 0.15f, falloffT);
                        finalDamage = damage * falloffMult;
                        isEpicenter = false;
                        currentKnockback = Mathf.Lerp(knockback * 0.7f, knockback * 0.2f, falloffT);
                    }

                    DamageInfo info = new DamageInfo(
                        amount: finalDamage,
                        abilitySource: abilityType,
                        attacker: gameObject,
                        hitPoint: col.transform.position,
                        hitDirection: hitDir,
                        knockbackForce: currentKnockback,
                        damageType: DamageType.Impact,
                        isCritical: isEpicenter,
                        isFocalHit: isEpicenter
                    );

                    damageable.TakeDamage(info);

                    if (col.TryGetComponent<DragonMotor>(out var targetMotor))
                    {
                        targetMotor.ApplyKnockback(hitDir, info.KnockbackForce);
                    }
                }
            }
        }

        private void TriggerAnimation(string triggerName)
        {
            if (_animator != null && !string.IsNullOrEmpty(triggerName))
            {
                _animator.SetTrigger(triggerName);
            }
        }

        private void HandleDeath(DamageInfo killer)
        {
            if (_activeCastCoroutine != null)
            {
                StopCoroutine(_activeCastCoroutine);
                _activeCastCoroutine = null;
            }
            _isCasting = false;
            _motor.ResetMotor();
            _motor.SetMovementLocked(true);
            _motor.SetRotationLocked(true);
            TriggerAnimation("Die");
        }

        public void ResetCombat()
        {
            if (_activeCastCoroutine != null)
            {
                StopCoroutine(_activeCastCoroutine);
                _activeCastCoroutine = null;
            }
            _isCasting = false;
            for (int i = 0; i < _cooldownTimers.Length; i++)
            {
                _cooldownTimers[i] = 0f;
                if (_abilities != null && i < _abilities.Length && _abilities[i] != null)
                {
                    OnCooldownUpdated?.Invoke(i, 0f, _abilities[i].Cooldown);
                }
            }
        }

        private void PlayAbilitySound(AudioClip clip, AbilitySoundType type)
        {
            if (DexHigh.Audio.AudioManager.Instance != null)
            {
                if (clip != null)
                {
                    DexHigh.Audio.AudioManager.Instance.PlaySfx(clip, 0.95f);
                }
                else
                {
                    switch (type)
                    {
                        case AbilitySoundType.FireBreath:
                            DexHigh.Audio.AudioManager.Instance.PlayFireBreath();
                            break;
                        case AbilitySoundType.TailWhip:
                            DexHigh.Audio.AudioManager.Instance.PlayTailWhip();
                            break;
                        case AbilitySoundType.FlyImpact:
                            DexHigh.Audio.AudioManager.Instance.PlayFlyImpact();
                            break;
                    }
                }
            }
            else if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position, 0.95f);
            }
        }

        private enum AbilitySoundType
        {
            FireBreath,
            TailWhip,
            FlyImpact
        }
    }
}
