namespace Core
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private Button backToMenuBtn;
        [SerializeField] private Button sendMessageBtn;
        [SerializeField] private Button retryBtn;
        [SerializeField] private GameObject powerUpStuff;

        private Dictionary<string, string> globalUserMessageDic = new();
        private bool ghostMessagesCollected;
        private bool gameRunning;
        private float pauseMomentTimeScale = 1f;

        private bool gameStarted;
        private bool inPowerUps;
        private bool isLaserActive;
        private bool isShieldActive;
        private bool isMagnetActive;

        private int bestScore;
        private int currentScore;
        private int coinNumber;
        private float musicVolume;
        private float sfxVolume;

        private GameObject camTarget;
        private bool noDeath;
        private int currentGameLevel;
        private bool ballAttached;

        private int vfxVolume;

        public bool IsGameOver { get; private set; }
        public int TotalPoints { get; private set; }


        // extras
        private float leftBarX;
        private float rightBarX;
        private int nextDangerZoneHeight;
        private bool checkForDangerZone;


        // Public accessors
        public float LeftBarX { get => leftBarX; set => leftBarX = value; }
        public float RightBarX { get => rightBarX; set => rightBarX = value; }
        public int NextDangerZoneHeight { get => nextDangerZoneHeight; set => nextDangerZoneHeight = value; }
        public bool CheckForDangerZone { get => checkForDangerZone; set => checkForDangerZone = value; }

        #region Singleton
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            IsGameOver = false;
            TotalPoints = 0;
        }
        #endregion

        private void Start()
        {
            gameRunning = true;
            isShieldActive = false;
            isLaserActive = true;
            isMagnetActive = false;
            noDeath = false;
            gameStarted = true;
            currentGameLevel = 0;

            Application.runInBackground = true;
            Application.targetFrameRate = 60;
        }

     

        #region Game Flow
        public void EndGame(int finalScore)
        {
            IsGameOver = true;
            TotalPoints = finalScore;

            retryBtn.gameObject.SetActive(true);
            backToMenuBtn.gameObject.SetActive(true);
            sendMessageBtn.gameObject.SetActive(true);
        }

        public void ResetGame()
        {
            IsGameOver = false;
            TotalPoints = 0;
        }
        #endregion

        #region Scores & Currency
        public void AddCoin(int amount) => coinNumber += amount;
        public int GetCoinCount() => coinNumber;
        public void SetBestScore(int score) => bestScore = score;
        public int GetBestScore() => bestScore;
        public void SetCurrentScore(int score) => currentScore = score;
        public int GetCurrentScore() => currentScore;
        #endregion

        #region Volumes
        public void SetMusicVolume(float volume) => musicVolume = volume;
        public float GetMusicVolume() => musicVolume;
        public void SetSfxVolume(float volume) => sfxVolume = volume;
        public float GetSfxVolume() => sfxVolume;
        public void SetVfxVolume(int volume) => vfxVolume = volume;
        public int GetVfxVolume() => vfxVolume;
        #endregion

        #region Power-Ups
        public bool IsShieldActive() => isShieldActive;
        public void SetShieldActive(bool active) => isShieldActive = active;
        public bool IsLaserActive() => isLaserActive;
        public void SetLaserActive(bool active) => isLaserActive = active;
        public bool IsMagnetActive() => isMagnetActive;
        public void SetMagnetActive(bool active) => isMagnetActive = active;
        public bool NoDeath => noDeath;
        public void SetNoDeath(bool active) => noDeath = active;
        #endregion

        #region Ball State
        public bool IsBallAttached() => ballAttached;
        public void SetBallAttached(bool attached) => ballAttached = attached;


        #endregion

        #region Game State

        public bool IsGamePaused() => !gameRunning;
        public void SetGamePaused(bool paused) => gameRunning = paused;
        #endregion

        #region Camera
        public void SetCameraTarget(GameObject target) => camTarget = target;
        public GameObject GetCameraTarget() => camTarget;
        #endregion
    }
}
