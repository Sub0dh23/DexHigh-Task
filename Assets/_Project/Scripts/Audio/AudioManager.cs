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
        [SerializeField] private AudioClip _bgmClip;

        public AudioClip FireBreathClip { get => _fireBreathClip; set => _fireBreathClip = value; }
        public AudioClip TailWhipClip { get => _tailWhipClip; set => _tailWhipClip = value; }
        public AudioClip FlyImpactClip { get => _flyImpactClip; set => _flyImpactClip = value; }
        public AudioClip VictoryClip { get => _victoryClip; set => _victoryClip = value; }
        public AudioClip DefeatClip { get => _defeatClip; set => _defeatClip = value; }
        public AudioClip BgmClip { get => _bgmClip; set => _bgmClip = value; }

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

            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.volume = 0.45f;
            }
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (_fireBreathClip == null)
                _fireBreathClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/472688__silverillusionist__fire-burst.wav");
            if (_tailWhipClip == null)
                _tailWhipClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/93100__cgeffex__whip-crack-01.wav");
            if (_flyImpactClip == null)
                _flyImpactClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/dragon-studio-hard-heavy-impact-515256.mp3");
            if (_victoryClip == null)
                _victoryClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Victory.mp3");
            if (_defeatClip == null)
                _defeatClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Loss.mp3");
            if (_bgmClip == null)
                _bgmClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Fight-Background.mp3");
#endif

            if (_bgmClip != null && _musicSource != null && !_musicSource.isPlaying)
            {
                _musicSource.clip = _bgmClip;
                _musicSource.Play();
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

        public void PlayFireBreath()
        {
            if (_fireBreathClip == null)
            {
#if UNITY_EDITOR
                _fireBreathClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/472688__silverillusionist__fire-burst.wav");
#endif
            }
            PlaySfx(_fireBreathClip, 0.9f);
        }

        public void PlayTailWhip()
        {
            if (_tailWhipClip == null)
            {
#if UNITY_EDITOR
                _tailWhipClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/93100__cgeffex__whip-crack-01.wav");
#endif
            }
            PlaySfx(_tailWhipClip, 0.95f);
        }

        public void PlayFlyTakeoff() => PlaySfx(_flyTakeoffClip, 0.8f);

        public void PlayFlyImpact()
        {
            if (_flyImpactClip == null)
            {
#if UNITY_EDITOR
                _flyImpactClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/dragon-studio-hard-heavy-impact-515256.mp3");
#endif
            }
            PlaySfx(_flyImpactClip, 1f);
        }

        public void PlayHit() => PlaySfx(_hitClip, 0.7f, 0.15f);

        public void PlayVictory()
        {
            if (_victoryClip == null)
            {
#if UNITY_EDITOR
                _victoryClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Victory.mp3");
#endif
            }
            if (_musicSource != null && _musicSource.isPlaying)
            {
                _musicSource.Stop();
            }
            PlaySfx(_victoryClip, 1f, 0f);
        }

        public void PlayDefeat()
        {
            if (_defeatClip == null)
            {
#if UNITY_EDITOR
                _defeatClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Loss.mp3");
#endif
            }
            if (_musicSource != null && _musicSource.isPlaying)
            {
                _musicSource.Stop();
            }
            PlaySfx(_defeatClip, 1f, 0f);
        }
    }
}
