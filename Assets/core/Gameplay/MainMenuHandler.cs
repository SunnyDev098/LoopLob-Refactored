using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuHandler : MonoBehaviour
{
    public Button PlayButton;
    public Button OptionsButton;
    public Button Scoreboard;

    public GameObject mainMenuPanel;
    public GameObject scoreboardPanel;
    public GameObject optionsPanel;
    public GameObject sceneDecoration;

    private void Awake()
    {
        if (PlayButton != null) PlayButton.onClick.AddListener(OnPlayClicked);
        if (OptionsButton != null) OptionsButton.onClick.AddListener(OnOptionClicked);
        if (Scoreboard != null) Scoreboard.onClick.AddListener(OnOptionClicked);


    }

    private void OnDestroy()
    {
        if (PlayButton != null) PlayButton.onClick.RemoveListener(OnPlayClicked);
        if (OptionsButton != null) OptionsButton.onClick.RemoveListener(OnOptionClicked);
        if (Scoreboard != null) Scoreboard.onClick.RemoveListener(OnScoreboardClicked);
        
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("CoreGame", LoadSceneMode.Single);
    }

    private void OnOptionClicked()
    {
        Debug.Log("options button clicked!");
        ShowOptions();
    }

    private void OnScoreboardClicked()
    {
        Debug.Log("scpreboard button clicked!");
        ShowScoreboard();
    }


    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        sceneDecoration.SetActive(true);
        scoreboardPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void ShowScoreboard()
    {
        mainMenuPanel.SetActive(false);
        scoreboardPanel.SetActive(true);
        sceneDecoration.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        scoreboardPanel.SetActive(false);
        sceneDecoration.SetActive(false);
        optionsPanel.SetActive(true);
    }
}
