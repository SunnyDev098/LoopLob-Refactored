using UnityEngine;
using UnityEngine.UI;
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


    private void OnRetryClicked() => Debug.Log("Retry button clicked.");
    private void OnMenuClicked() => Debug.Log("Menu button clicked.");
    private void OnSendMessageClicked() => Debug.Log("Send Message button clicked.");
}
