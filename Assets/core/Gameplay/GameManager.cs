namespace Core
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI References")]
      
        [SerializeField] private GameObject powerUpStuff;
        [SerializeField] public AudioSource audioSource;
        public GameObject Ball;
        public GameObject Camera;
        public GameObject TopBar;

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
        public float SfxVolume { get; private set; } = 1f;
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

        private void Awake()
        {
           

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }


            
            LoadSettings(); 
        }
     
       
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

        private void LoadSettings()
        {
            SfxVolume = PlayerPrefs.GetFloat("sfx_volume", 1f);
        }

      //  #region Game Flow
        public void EndGame(int finalScore)
        {
            if (IsGameOver) return;

            IsGameOver = true;
            SetGamePaused(true);
            Debug.Log($"Game Over! Final score: {finalScore}");
        }
        private void OnEnable()
        {
            EventBus.OnGameOver += HandleGameOver;
            SceneManager.sceneLoaded += OnSceneLoaded;

        }

        private void OnDisable()
        {
            EventBus.OnGameOver -= HandleGameOver;
            SceneManager.sceneLoaded -= OnSceneLoaded;

        }
        private void HandleGameOver()
        {
            EndGame(GetCurrentScore()); // Or whatever finalScore source you have
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Ball = GameObject.FindGameObjectWithTag("Ball");
            Camera = GameObject.FindGameObjectWithTag("MainCamera");
            TopBar = GameObject.FindGameObjectWithTag("TopBar");
        }
        public void ResetGame()
        {
            IsGameOver = false;
            TotalPoints = 0;
        }
        public void ResetGameAndScene()
        {
            IsGameOver = false;
            TotalPoints = 0;
            currentScore = 0;
            coinNumber = 0;
            gameRunning = true;
            isShieldActive = false;
            isLaserActive = false;
            isMagnetActive = false;

            EventBus.RaiseScoreChanged(0);
            EventBus.RaiseCoinCollected(0);
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
        //  #region Scores & Currency
        public void AddCoin(int amount) => coinNumber += amount;
        public int GetCoinCount() => coinNumber;
        public void SetBestScore(int score) => bestScore = score;
        public int GetBestScore() => bestScore;
        public void SetCurrentScore(int score) => currentScore = score;
        public int GetCurrentScore() => currentScore;

       // #region Volumes
        public void SetMusicVolume(float volume) => musicVolume = volume;
        public float GetMusicVolume() => musicVolume;
        public void SetSfxVolume(float volume) => sfxVolume = volume;
        public float GetSfxVolume() => sfxVolume;
        public void SetSfxVolume(int volume) => sfxVolume = volume;

     //   #region Power-Ups
        public bool IsShieldActive() => isShieldActive;
        public void SetShieldActive(bool active) => isShieldActive = active;
        public bool IsLaserActive() => isLaserActive;
        public void SetLaserActive(bool active) => isLaserActive = active;
        public bool IsMagnetActive() => isMagnetActive;
        public void SetMagnetActive(bool active) => isMagnetActive = active;
        public bool NoDeath => noDeath;
        public void SetNoDeath(bool active) => noDeath = active;

     //   #region Ball State
        public bool IsBallAttached() => ballAttached;
        public void SetBallAttached(bool attached) => ballAttached = attached;



      //  #region Game State

        public bool IsGamePaused() => !gameRunning;
        public void SetGamePaused(bool paused) => gameRunning = paused;

     //   #region Camera
        public void SetCameraTarget(GameObject target) => camTarget = target;
        public GameObject GetCameraTarget() => camTarget;
    }
}
