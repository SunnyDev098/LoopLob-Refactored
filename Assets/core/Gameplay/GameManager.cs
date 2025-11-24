namespace Core
{
    using Environment;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }


       // public bool InTutorial;
        [Header("Config References")]
        public ProceduralSettings proceduralSettings;

        [Header("UI References")]
        public GameObject AttachedPlanet;
        public bool IsDebugMode;
        public bool ballCanMove;
        public int lastbally;

        [SerializeField] private GameObject powerUpStuff;
        [SerializeField] public AudioSource audioSource;
        public GameObject Ball;
        public GameObject Camera;
        public GameObject TopBar;
        public GameObject Shield;
        public GameObject Magnet;
        public float lastMissile;
       
        public TextMeshProUGUI CoinTxt;

        private Dictionary<string, string> globalUserMessageDic = new();
        private bool ghostMessagesCollected;
        private bool gameRunning;
        private float pauseMomentTimeScale = 1f;

        public bool gameStarted;
        private bool inPowerUps;
        private bool isLaserActive;
        public bool isShieldActive;
        public bool isMagnetActive;

        public bool isBestScore ;
        private int currentScore;
        public int coinNumber;

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

        public  float difficulty;

        private void Awake()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "CoreGame")
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);

            }
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

            IsGameOver = false;
        }


        private void Start()
        {
            gameRunning = true;
            isShieldActive = false;
            isLaserActive = true;
            isMagnetActive = false;
            noDeath = false;
            currentGameLevel = 0;
            isBestScore = false;
            Application.runInBackground = true;
            Application.targetFrameRate = 60;

         
        }
        private void Update()
        {
        }
       

      //  #region Game Flow
        public void EndGame(int finalScore)
        {
            if (IsGameOver) return;

            SetGamePaused(true);
            Debug.Log($"Game Over! Final score: {finalScore}");
            ballCanMove = false;
            Ball.transform.GetChild(2).gameObject.SetActive(false);
            if (finalScore > DataHandler.Instance.GetBestScore())
            {
                isBestScore = true;
                DataHandler.Instance.SaveBestScore(finalScore);


            }

            DataHandler.Instance.SaveTotalCoins(coinNumber);




        }
        private void OnEnable()
        {

            SceneManager.sceneLoaded += OnSceneLoaded;
            EventBus.OnGameOver += HandleGameOver;

        }

        private void OnDisable()
        {
            EventBus.OnGameOver -= HandleGameOver;
            SceneManager.sceneLoaded -= OnSceneLoaded;

        }
        private void HandleGameOver()
        {
            EndGame(GetCurrentScore());
            DataHandler.Instance.SaveTotalCoins(coinNumber);
            Ball.GetComponent<CircleCollider2D>().enabled = false;
        }
        private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            try
            {
                await Task.Delay(200);

                Ball = GameObject.FindGameObjectWithTag("Ball");
                Camera = GameObject.FindGameObjectWithTag("MainCamera");
                TopBar = GameObject.FindGameObjectWithTag("TopBar");

                Shield = Ball.transform.GetChild(2).gameObject;
                Magnet = Ball.transform.GetChild(3).gameObject;

                CoinTxt = GameObject.FindGameObjectWithTag("CoinTxt").GetComponent<TextMeshProUGUI>();
                coinNumber = DataHandler.Instance.GetTotalCoins();
                CoinTxt.text = coinNumber.ToString();
            }
            catch (Exception ex)
            {
              //  Debug.LogError($"[OnSceneLoaded] Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void ResetGame()
        {
            IsGameOver = false;
            TotalPoints = 0;
        }
        public void ResetGameAndScene()
        {
            difficulty = 0;
            gameStarted = false;
            IsGameOver = false;
            TotalPoints = 0;
            currentScore = 0;
            gameRunning = true;
            isShieldActive = false;
            isLaserActive = false;
            isMagnetActive = false;
            isBestScore = false;

            SetGamePaused(false);
            EventBus.RaiseScoreChanged(0);
            lastbally = 0;
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
        //  #region Scores & Currency
        public void AddCoin(int amount)
        {
            coinNumber += amount;
            CoinTxt.text = coinNumber.ToString();
        }
        public void SetCurrentScore(int score) => currentScore = score;
        public int GetCurrentScore() => currentScore;
        public int increaseCurrentScore(int plusScore) => currentScore = currentScore+ plusScore;

        public void DeActiveSheildCall()
        {
            Shield.SetActive(false);
            Invoke("DeActiveSheild", 2);
        }

        public void DeActiveSheild()
        {
            isShieldActive = false ;
        }

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
        public void SetGamePaused(bool paused) => gameRunning = !paused;

     //   #region Camera
        public void SetCameraTarget(GameObject target) => camTarget = target;
        public GameObject GetCameraTarget() => camTarget;
    }
}
