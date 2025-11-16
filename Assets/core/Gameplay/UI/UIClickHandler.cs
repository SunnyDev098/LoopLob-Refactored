using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Core;
using System.Threading.Tasks;
using TMPro;
public class UIButtonClickManager : MonoBehaviour
{
    public GhostMessageSubmitter ghostMessageSubmitter;
    public Button retryButton;
    public Button menuButton;
    public Button sendMessageButton;
    public Button SubmitMessageButton;
    public TMP_InputField tMP_InputField;
    // public Button ;



    public GameObject deathScore;
    public GameObject newBest;

    public void Start()
    {
        retryButton.onClick.AddListener(OnRetryClicked);
        menuButton.onClick.AddListener(OnMenuClicked);
        sendMessageButton.onClick.AddListener(OnSendMessageClicked);
        SubmitMessageButton.onClick.AddListener(OnSubmitessageClicked);
        EventBus.OnGameOver += HandleGameOverUI;

    }

    private void OnDisable()
    {
        Core.EventBus.OnGameOver -= HandleGameOverUI;

    }
    public async void HandleGameOverUI()
    {
        retryButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
       // sendMessageButton.gameObject.SetActive(true);
        deathScore.gameObject.SetActive(true);
        deathScore.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(GameManager.Instance.GetCurrentScore().ToString());
        await Task.Delay(1000);
        if (GameManager.Instance.isBestScore)
        {
            newBest.SetActive(true);
        }
        

    }
    private void OnRetryClicked()
    {
        GameManager.Instance.ResetGameAndScene();
        SceneManager.LoadScene("CoreGame");

    }

    private void OnMenuClicked()
    {
        GameManager.Instance.ResetGameAndScene();

        SceneManager.LoadScene("InitialScene");
    }
    private void OnSendMessageClicked() => sendMessageButton.gameObject.transform.GetChild(1).gameObject.SetActive(true);
    private async void OnSubmitessageClicked()
    {
        Debug.Log("woow");
        ghostMessageSubmitter.SubmitDeathMessageAsync(GameManager.Instance.GetCurrentScore(), tMP_InputField.text.Trim());
        await Task.Delay(500);
        sendMessageButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        sendMessageButton.gameObject.SetActive(false);
    }
}
