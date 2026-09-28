using DexHigh.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DexHigh.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private static MainMenuUI _instance;
        public static MainMenuUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<MainMenuUI>(FindObjectsInactive.Include);
                }
                return _instance;
            }
        }

        [Header("UI References")]
        [SerializeField] private GameObject _menuRoot;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            if (_menuRoot == null)
            {
                _menuRoot = gameObject;
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            if (_playButton != null)
            {
                _playButton.onClick.RemoveAllListeners();
                _playButton.onClick.AddListener(OnPlayClicked);
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.RemoveAllListeners();
                _exitButton.onClick.AddListener(OnExitClicked);
            }
        }

        public void ShowMenu()
        {
            if (_menuRoot != null)
            {
                _menuRoot.SetActive(true);
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
        }

        public void HideMenu()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }

            if (_menuRoot != null)
            {
                _menuRoot.SetActive(false);
            }
        }

        private void OnPlayClicked()
        {
            HideMenu();

            if (BattleGameManager.Instance != null)
            {
                BattleGameManager.Instance.StartGameFromMenu();
            }
        }

        private void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
