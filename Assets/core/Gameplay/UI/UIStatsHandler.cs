using TMPro;
using UnityEngine;

public class UIStatsHandler : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinsText;

    private void OnEnable()
    {
        // Subscribe to the event
        Core.EventBus.OnScoreChanged += HandleScoreChanged;

    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        Core.EventBus.OnScoreChanged -= HandleScoreChanged;

    }

    private void HandleScoreChanged(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString();
        }
    }

   
}
