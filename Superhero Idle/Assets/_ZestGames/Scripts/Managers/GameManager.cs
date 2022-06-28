using UnityEngine;
using DG.Tweening;
using SuperheroIdle;

namespace ZestGames
{
    public class GameManager : MonoBehaviour
    {
        public static Enums.GameState GameState { get; private set; }
        public static Enums.GameEnd GameEnd { get; private set; }

        [Header("-- REFERENCES --")]
        [SerializeField] private ObjectPooler objectPooler;
        private UiManager _uiManager;
        private LevelManager _levelManager;
        private SettingsManager _settingsManager;
        private DataManager _dataManager;
        private CrimeManager _crimeManager;

        private void Init()
        {
            Application.targetFrameRate = 240;
            // TODO: Check for DOTween capacity requirement.
            //DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity();

            objectPooler.Init(this);

            GameState = Enums.GameState.WaitingToStart;
            GameEnd = Enums.GameEnd.None;
            
            #region MANAGER INITIALIZATIONS
            _levelManager = GetComponent<LevelManager>();
            _levelManager.Init(this);
            _dataManager = GetComponent<DataManager>();
            _dataManager.Init(this);
            _settingsManager = GetComponent<SettingsManager>();
            _settingsManager.Init(this);
            _uiManager = GetComponent<UiManager>();
            _uiManager.Init(this);
            _crimeManager = GetComponent<CrimeManager>();
            _crimeManager.Init(this);
            #endregion

            UiEvents.OnUpdateCollectableText?.Invoke(DataManager.TotalMoney);
            UiEvents.OnUpdateLevelText?.Invoke(LevelHandler.Level);
        }

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameEnd += HandleGameEnd;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameEnd -= HandleGameEnd;

            DOTween.KillAll();
        }

        private void HandleGameStart()
        {
            GameState = Enums.GameState.Started;
        }

        private void HandleGameEnd(Enums.GameEnd gameEnd)
        {
            GameEnd = gameEnd;

            if (gameEnd == Enums.GameEnd.Success)
                GameEvents.OnLevelSuccess?.Invoke();
            else if (gameEnd == Enums.GameEnd.Fail)
                GameEvents.OnLevelFail?.Invoke();
        }
    }
}
