using UnityEngine;

namespace DexHigh.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;

        [Header("SFX Library")]
        [SerializeField] private AudioClip _fireBreathClip;
        [SerializeField] private AudioClip _tailWhipClip;
        [SerializeField] private AudioClip _flyTakeoffClip;
        [SerializeField] private AudioClip _flyImpactClip;
        [SerializeField] private AudioClip _hitClip;
        [SerializeField] private AudioClip _victoryClip;
        [SerializeField] private AudioClip _defeatClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlaySfx(AudioClip clip, float volume = 1f, float pitchVariation = 0.1f)
        {
            if (clip == null || _sfxSource == null) return;

            _sfxSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            _sfxSource.PlayOneShot(clip, volume);
        }

        public void PlaySfxAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        public void PlayFireBreath() => PlaySfx(_fireBreathClip, 0.9f);
        public void PlayTailWhip() => PlaySfx(_tailWhipClip, 0.9f);
        public void PlayFlyTakeoff() => PlaySfx(_flyTakeoffClip, 0.8f);
        public void PlayFlyImpact() => PlaySfx(_flyImpactClip, 1f);
        public void PlayHit() => PlaySfx(_hitClip, 0.7f, 0.15f);
        public void PlayVictory() => PlaySfx(_victoryClip, 1f, 0f);
        public void PlayDefeat() => PlaySfx(_defeatClip, 1f, 0f);
    }
}
