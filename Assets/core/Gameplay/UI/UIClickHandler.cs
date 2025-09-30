using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Core;
public class UIButtonClickManager : MonoBehaviour
{
    [SerializeField] public Button retryButton;
    [SerializeField] public Button menuButton;
    [SerializeField] public Button sendMessageButton;

    public void Start()
    {
        retryButton.onClick.AddListener(OnRetryClicked);
        menuButton.onClick.AddListener(OnMenuClicked);
        sendMessageButton.onClick.AddListener(OnSendMessageClicked);
    }
    private void OnEnable()
    {
        Core.EventBus.OnGameOver += HandleGameOverUI;

    }

    private void OnDisable()
    {
        Core.EventBus.OnGameOver -= HandleGameOverUI;

    }
    private void HandleGameOverUI()
    {
        retryButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
      //  sendMessageButton.gameObject.SetActive(true);
     

    }
    private void OnRetryClicked() => SceneManager.LoadScene("CoreGame");    
    private void OnMenuClicked(){
        SceneManager.LoadScene("InitialScene");
        GameManager.Instance.ResetGame();
    } 
    private void OnSendMessageClicked() => Debug.Log("Send Message button clicked.");
}
