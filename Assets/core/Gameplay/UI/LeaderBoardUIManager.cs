using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderBoardGo;
    [SerializeField] private GameObject loadingGo;
    [SerializeField] private GameObject netAndUserName;
    [SerializeField] private Button BackButton;
    [SerializeField] private MainMenuHandler mainMenuHandler;



    private FirebaseFirestore db;

    // UI References (Inspector)
    [Header("Top 3")]
    public TextMeshProUGUI first_name;
    public TextMeshProUGUI first_score;

    public TextMeshProUGUI second_name;
    public TextMeshProUGUI second_score;

    public TextMeshProUGUI third_name;
    public TextMeshProUGUI third_score;

    public TextMeshProUGUI fourth_name;
    public TextMeshProUGUI fourth_score;

    public TextMeshProUGUI fivth_name;
    public TextMeshProUGUI fivth_score;

    public TextMeshProUGUI sixth_name;
    public TextMeshProUGUI sixth_score;


    public TextMeshProUGUI seventh_name;
    public TextMeshProUGUI seventh_score;

    public TextMeshProUGUI eighth_name;
    public TextMeshProUGUI eighth_score;

    public TextMeshProUGUI ninth_name;
    public TextMeshProUGUI ninth_score;

    [Header("Current Player")]
    public TextMeshProUGUI user_name;
    public TextMeshProUGUI user_score;
    public TextMeshProUGUI user_ranking;

    private void Awake()
    {
        BackButton.onClick.AddListener(OnBackButtonClick);

    }
    private async void Start()
    {
          
      
    }
    private async void OnEnable()
    {

      


        loadingGo.SetActive(true);
        leaderBoardGo.SetActive(false);
        netAndUserName.SetActive(false);

        await Task.Delay(200);

        Debug.Log("data reading");
        db = FirebaseFirestore.DefaultInstance;
        await SubmitAndGetTopInfo();

    }

    /// <summary>
    /// Updates current player's best score in Firestore, then retrieves:
    /// - Number of players with better score
    /// - Top 3 players
    /// Updates UI or logs error.
    /// </summary>
    public async Task SubmitAndGetTopInfo()
    {
        try
        {
            string username = GetUsername();
            int bestScore = GetBestScore();

            await UpdateScoreInFirestore(username, bestScore);

            int topperCount = await CountPlayersWithHigherScore(bestScore);
            var (topNames, topScores) = await GetTopThreePlayers();

            UpdateLeaderboardUI(topperCount, topNames, topScores);
            DataHandler.Instance.SetLeaderBoard(true);
            leaderBoardGo.SetActive(true);
            loadingGo.SetActive(false);

        }
        catch (System.Exception ex)
        {
            HandleError(ex);
            DataHandler.Instance.SetLeaderBoard(false);
            leaderBoardGo.SetActive(false);
            loadingGo.SetActive(false);
            netAndUserName.SetActive(true);

        }
    }

    private string GetUsername()
    {
        string username =DataHandler.Instance.GetUserName();
        if (string.IsNullOrWhiteSpace(username))
            throw new System.Exception("Username not set in PlayerPrefs.");
        return username.ToLowerInvariant();
    }

    private int GetBestScore()
    {
       
        return DataHandler.Instance.GetBestScore();
    }

    private async Task UpdateScoreInFirestore(string username, int score)
    {
        var userDoc = db.Collection("usernames").Document(username);
        var data = new Dictionary<string, object> { { "best_score", score } };
        await userDoc.SetAsync(data, SetOptions.MergeAll);
    }

    private async Task<int> CountPlayersWithHigherScore(int score)
    {
        var snap = await db.Collection("usernames")
                           .WhereGreaterThan("best_score", score)
                           .GetSnapshotAsync();
        return snap.Count;
    }

    private async Task<(List<string> names, List<int> scores)> GetTopThreePlayers()
    {
        var snap = await db.Collection("usernames")
                           .OrderByDescending("best_score")
                           .Limit(9)
                           .GetSnapshotAsync();

        var names = new List<string>(9);
        var scores = new List<int>(9);

        foreach (var doc in snap.Documents)
        {
            names.Add(doc.Id);
            scores.Add(doc.ContainsField("best_score") ? doc.GetValue<int>("best_score") : 0);
        }

        return (names, scores);
    }

    private void UpdateLeaderboardUI(int topperCount, List<string> topNames, List<int> topScores)
    {
        if (topNames.Count >= 9)
        {
            first_name.text = topNames[0];
            first_score.text = topScores[0].ToString();

            second_name.text = topNames[1];
            second_score.text = topScores[1].ToString();

            third_name.text = topNames[2];
            third_score.text = topScores[2].ToString();



            fourth_name.text = topNames[3];
            fourth_score.text = topScores[3].ToString();

            fivth_name.text = topNames[4];
            fivth_score.text = topScores[4].ToString();

            sixth_name.text = topNames[5];
            sixth_score.text = topScores[5].ToString();

            seventh_name.text = topNames[6];
            seventh_score.text = topScores[6].ToString();

            eighth_name.text = topNames[7];
            eighth_score.text = topScores[7].ToString();

            ninth_name.text = topNames[8];
            ninth_score.text = topScores[8].ToString();

        }

        user_name.text = GetUsername();
        user_score.text = GetBestScore().ToString();
        user_ranking.text = (topperCount + 1).ToString();
    }

    private void HandleError(System.Exception ex)
    {
        Debug.LogError($"Leaderboard error: {ex.Message}");
    }

    private void OnBackButtonClick()
    {

        DataHandler.Instance.GoScoreboard = false;
        mainMenuHandler.ShowMainMenu();

    }

}
