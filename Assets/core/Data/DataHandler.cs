using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DataHandler : MonoBehaviour
{
    public static DataHandler Instance { get; private set; }

    private const string COIN_PREF_KEY = "PlayerCoinsAmount";
    private const string BEST_SCORE_PREF_KEY = "PlayerBestScore";
    private const string USER_NAME_PREF_KEY = "PlayerUserName";
    private const string BACK_GROUND_INDEX_KEY = "BackGroundIndex";
    private const string FIRST_TIME = "FirstTime";

    private int bestscore;
    private int totalcoins;
    private string username;
    public int backGroundIndex;
    public bool firstTime;

    private bool isLeaderBoardReady;


    public bool GoScoreboard = false;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // prevent duplicate singletons
            return;
        }
    }

    private void Start()
    {
       // PlayerPrefs.DeleteAll();

        DataInitiator();
        Application.targetFrameRate = 60;
    }

    private void DataInitiator()
    {
        // Coins
        if (PlayerPrefs.HasKey(COIN_PREF_KEY))
        {
            totalcoins = PlayerPrefs.GetInt(COIN_PREF_KEY);
        }
        else
        {
            totalcoins = 0;
            PlayerPrefs.SetInt(COIN_PREF_KEY, totalcoins);
        }

        // Best Score
        if (PlayerPrefs.HasKey(BEST_SCORE_PREF_KEY))
        {
            bestscore = PlayerPrefs.GetInt(BEST_SCORE_PREF_KEY);
        }
        else
        {
            bestscore = 0;
            PlayerPrefs.SetInt(BEST_SCORE_PREF_KEY, bestscore);
        }

        // Username
        if (PlayerPrefs.HasKey(USER_NAME_PREF_KEY))
        {
            username = PlayerPrefs.GetString(USER_NAME_PREF_KEY);
        }
        else
        {
            username = null;
        }


        if (PlayerPrefs.HasKey(BACK_GROUND_INDEX_KEY))
        {

            backGroundIndex = PlayerPrefs.GetInt(BACK_GROUND_INDEX_KEY);
            Debug.Log(backGroundIndex);
        }
        else
        {
            backGroundIndex = 0;
            Debug.Log(backGroundIndex);

        }

        if (PlayerPrefs.HasKey(FIRST_TIME))
        {
            firstTime = false;
        }
        else
        {
            firstTime = true;

        }


        PlayerPrefs.Save();

    }

    // ----------------- coins -----------------------
    public void SaveTotalCoins(int coin)
    {
        totalcoins = coin;
        PlayerPrefs.SetInt(COIN_PREF_KEY, totalcoins);
        PlayerPrefs.Save();
    }

    public int GetTotalCoins()
    {
        return totalcoins;
    }

    // ----------------- score -----------------------
    public void SaveBestScore(int score)
    {
        bestscore = score;
        PlayerPrefs.SetInt(BEST_SCORE_PREF_KEY, bestscore);
        PlayerPrefs.Save();
    }

    public int GetBestScore()
    {
        return bestscore;
    }

    // ----------------- userName -----------------------

    public string GetUserName()
    {
        return username;


    }

    public void SetUserName(string name)
    {
        username = name;
        PlayerPrefs.SetString(USER_NAME_PREF_KEY, username);
        PlayerPrefs.Save();
    }

    // ----------------- backGround -----------------------

    public int GetBackGroundIndex()
    {
        return backGroundIndex;


    }

    public void SetBackGroundIndex(int index)
    {
        backGroundIndex = index;
        PlayerPrefs.SetInt(BACK_GROUND_INDEX_KEY, index);
        PlayerPrefs.Save();
    }

    public void SetLeaderBoard(bool isReady)
    {
        isLeaderBoardReady = isReady;
    }

    public bool GetLeaderBoard()
    {
        return isLeaderBoardReady;
    }
}
