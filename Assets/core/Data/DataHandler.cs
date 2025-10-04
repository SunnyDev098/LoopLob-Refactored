using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DataHandler : MonoBehaviour
{
    public static DataHandler Instance { get; private set; }

    private const string COIN_PREF_KEY = "PlayerCoinsAmount";
    private const string BEST_SCORE_PREF_KEY = "PlayerBestScore";
    private const string USER_NAME_PREF_KEY = "PlayerUserName";

    private int bestscore;
    private int totalcoins;
    private string username;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persist across scene changes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // prevent duplicate singletons
            return;
        }
    }

    private void Start()
    {
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
    public void SetUserName(string name)
    {
        username = name;
        PlayerPrefs.SetString(USER_NAME_PREF_KEY, username);
        PlayerPrefs.Save();
    }

    public string GetUserName()
    {
        return username;
    }
}
