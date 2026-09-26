using UnityEngine;

namespace DexHigh.Combat
{
    [CreateAssetMenu(fileName = "NewAbilityData", menuName = "DexHigh/Combat/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        [Header("General Info")]
        [SerializeField] private AbilityType _abilityType;
        [SerializeField] private string _abilityName = "Ability";
        [TextArea(2, 4)]
        [SerializeField] private string _description = "Ability description";
        [SerializeField] private Sprite _icon;
        [SerializeField] private KeyCode _defaultHotkey = KeyCode.Q;

        [Header("Stats")]
        [SerializeField] private float _baseDamage = 25f;
        [SerializeField] private float _cooldown = 4f;
        [SerializeField] private float _castDuration = 1.2f;
        [SerializeField] private float _effectiveRange = 8f;
        [SerializeField] private float _impactRadius = 3.5f;
        [SerializeField] private float _outerImpactRadius = 8.0f;
        [SerializeField] private float _minRange = 0f;
        [SerializeField] private float _knockbackForce = 5f;

        [Header("Animation & Visuals")]
        [SerializeField] private string _animationTrigger = "Attack";
        [SerializeField] private GameObject _vfxPrefab;
        [SerializeField] private GameObject _impactVfxPrefab;
        [SerializeField] private GameObject _targetIndicatorPrefab;

        [Header("Audio")]
        [SerializeField] private AudioClip _castAudioClip;
        [SerializeField] private AudioClip _impactAudioClip;

        public AbilityType AbilityType => _abilityType;
        public string AbilityName => _abilityName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public KeyCode DefaultHotkey => _defaultHotkey;
        public float BaseDamage => _baseDamage;
        public float Cooldown => _cooldown;
        public float CastDuration => _castDuration;
        public float EffectiveRange => _effectiveRange;
        public float ImpactRadius => _impactRadius > 0f ? _impactRadius : 3.5f;
        public float OuterImpactRadius => _outerImpactRadius > 0f ? _outerImpactRadius : 8.0f;
        public float MinRange => _minRange;
        public float KnockbackForce => _knockbackForce;
        public string AnimationTrigger => _animationTrigger;
        public GameObject VfxPrefab => _vfxPrefab;
        public GameObject ImpactVfxPrefab => _impactVfxPrefab;
        public GameObject TargetIndicatorPrefab => _targetIndicatorPrefab;
        public AudioClip CastAudioClip => _castAudioClip;
        public AudioClip ImpactAudioClip => _impactAudioClip;
    }
}
