using System;
using System.Collections;
using System.Collections.Generic;
using DexHigh.Combat;
using UnityEngine;

namespace DexHigh.Characters
{
    public class DragonHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float _maxHealth = 200f;
        [SerializeField] private string _characterName = "Dragon";
        [SerializeField] private bool _isPlayer = false;

        [Header("Hit Flash Settings")]
        [SerializeField] private Renderer[] _meshRenderers;
        [SerializeField] private Color _flashColor = new Color(1f, 0.3f, 0.3f, 1f);
        [SerializeField] private float _flashDuration = 0.15f;

        private float _currentHealth = 200f;
        private bool _isInvulnerable;
        private bool _isDead;
        private Coroutine _flashCoroutine;
        private readonly List<Material> _instancedMaterials = new List<Material>();
        private readonly List<Color> _originalBaseColors = new List<Color>();

        public event Action<float, float> OnHealthChanged; // current, max
        public event Action<DamageInfo> OnDamaged;
        public event Action<DamageInfo> OnDied;
        public event Action OnRevived;
        public event Action<bool> OnBurnStateChanged;

        private Coroutine _burnCoroutine;
        private bool _isBurning;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public string CharacterName => _characterName;
        public bool IsPlayer => _isPlayer;
        public bool IsAlive => !_isDead && _currentHealth > 0f;
        public bool IsInvulnerable => _isInvulnerable;
        public bool IsBurning => _isBurning;
        public Transform Transform => transform;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private void Awake()
        {
            _isDead = false;
            _currentHealth = _maxHealth > 0f ? _maxHealth : 200f;
            CacheMaterials();
        }

        private void CacheMaterials()
        {
            if (_meshRenderers == null || _meshRenderers.Length == 0)
            {
                _meshRenderers = GetComponentsInChildren<Renderer>();
            }

            foreach (var renderer in _meshRenderers)
            {
                if (renderer == null) continue;
                foreach (var mat in renderer.materials)
                {
                    _instancedMaterials.Add(mat);
                    if (mat.HasProperty(BaseColorId))
                    {
                        _originalBaseColors.Add(mat.GetColor(BaseColorId));
                    }
                    else if (mat.HasProperty(ColorId))
                    {
                        _originalBaseColors.Add(mat.GetColor(ColorId));
                    }
                    else
                    {
                        _originalBaseColors.Add(Color.white);
                    }
                }
            }
        }

        public void SetInvulnerable(bool invulnerable)
        {
            _isInvulnerable = invulnerable;
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (_isDead || _isInvulnerable || damageInfo.Amount <= 0f) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damageInfo.Amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnDamaged?.Invoke(damageInfo);

            TriggerHitFlash();

            if (_currentHealth <= 0f && !_isDead)
            {
                Die(damageInfo);
            }
        }

        private void Die(DamageInfo killerInfo)
        {
            _isDead = true;
            StopBurn();
            OnDied?.Invoke(killerInfo);
        }

        public void ApplyBurn(float damagePerTick, float duration, float interval, GameObject attacker)
        {
            if (_isDead || _isInvulnerable) return;

            if (_burnCoroutine != null)
            {
                StopCoroutine(_burnCoroutine);
            }
            _burnCoroutine = StartCoroutine(BurnRoutine(damagePerTick, duration, interval, attacker));
        }

        public void StopBurn()
        {
            if (_burnCoroutine != null)
            {
                StopCoroutine(_burnCoroutine);
                _burnCoroutine = null;
            }

            if (_isBurning)
            {
                _isBurning = false;
                OnBurnStateChanged?.Invoke(false);
            }
        }

        private IEnumerator BurnRoutine(float damagePerTick, float duration, float interval, GameObject attacker)
        {
            _isBurning = true;
            OnBurnStateChanged?.Invoke(true);

            float elapsed = 0f;
            WaitForSeconds wait = new WaitForSeconds(interval);

            while (elapsed < duration && !_isDead && !_isInvulnerable)
            {
                yield return wait;
                elapsed += interval;

                DamageInfo burnDamage = new DamageInfo(
                    amount: damagePerTick,
                    abilitySource: AbilityType.FireBreath,
                    attacker: attacker,
                    hitPoint: transform.position + Vector3.up * 1.2f,
                    hitDirection: Vector3.up,
                    knockbackForce: 0f,
                    damageType: DamageType.Burn
                );

                TakeDamage(burnDamage);
            }

            _isBurning = false;
            OnBurnStateChanged?.Invoke(false);
            _burnCoroutine = null;
        }

        public void Heal(float amount)
        {
            if (_isDead || amount <= 0f) return;

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void ResetHealth()
        {
            _isDead = false;
            _isInvulnerable = false;
            StopBurn();
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnRevived?.Invoke();
            ResetMaterialColors();
        }

        private void TriggerHitFlash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }
            _flashCoroutine = StartCoroutine(HitFlashRoutine());
        }

        private IEnumerator HitFlashRoutine()
        {
            for (int i = 0; i < _instancedMaterials.Count; i++)
            {
                var mat = _instancedMaterials[i];
                if (mat == null) continue;

                if (mat.HasProperty(BaseColorId))
                    mat.SetColor(BaseColorId, _flashColor);
                else if (mat.HasProperty(ColorId))
                    mat.SetColor(ColorId, _flashColor);

                if (mat.HasProperty(EmissionColorId))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor(EmissionColorId, _flashColor * 1.5f);
                }
            }

            yield return new WaitForSeconds(_flashDuration);

            ResetMaterialColors();
            _flashCoroutine = null;
        }

        private void ResetMaterialColors()
        {
            for (int i = 0; i < _instancedMaterials.Count; i++)
            {
                var mat = _instancedMaterials[i];
                if (mat == null) continue;

                var orig = (i < _originalBaseColors.Count) ? _originalBaseColors[i] : Color.white;
                if (mat.HasProperty(BaseColorId))
                    mat.SetColor(BaseColorId, orig);
                else if (mat.HasProperty(ColorId))
                    mat.SetColor(ColorId, orig);

                if (mat.HasProperty(EmissionColorId))
                {
                    mat.SetColor(EmissionColorId, Color.black);
                }
            }
        }
    }
}
