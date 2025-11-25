using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuHandler : MonoBehaviour
{
    public Button PlayButton;
    public Button OptionsButton;
    public Button ScoreboardButton;
    public Button Tutorial;

    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject scoreboardPanel;
    public GameObject TutorialPanel;

    public GameObject sceneDecoration;
    public PanelDecider panelDecider;

    private void Awake()
    {
        if (PlayButton != null) PlayButton.onClick.AddListener(OnPlayClicked);
        if (OptionsButton != null) OptionsButton.onClick.AddListener(OnOptionClicked);
        if (ScoreboardButton != null) ScoreboardButton.onClick.AddListener(OnScoreboardClicked);
        if (Tutorial != null) Tutorial.onClick.AddListener(ShowTutorial);


    }

    private void OnDestroy()
    {
        if (PlayButton != null) PlayButton.onClick.RemoveListener(OnPlayClicked);
        if (OptionsButton != null) OptionsButton.onClick.RemoveListener(OnOptionClicked);
        if (ScoreboardButton != null) ScoreboardButton.onClick.RemoveListener(OnScoreboardClicked);
        if (Tutorial != null) Tutorial.onClick.RemoveListener(ShowTutorial);
        
    }

    private void OnEnable()
    {
        if (DataHandler.Instance.GoScoreboard)
        {
            OnScoreboardClicked();

        }



    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("CoreGame", LoadSceneMode.Single);
    }

    private void OnOptionClicked()
    {
        ShowOptions();
        sceneDecoration.SetActive(false);
    }

    private void OnScoreboardClicked()
    {
        if (PlayerPrefs.HasKey("PlayerUserName"))
        {
            ShowScoreboard();
            sceneDecoration.SetActive(false);
        }
        else
        {
            panelDecider.ShowAuth();
            DataHandler.Instance.GoScoreboard = true;

        }


    }


    public void ShowMainMenu()
    {
        OptionSelectionService.SelectOption(transform.parent.gameObject, "MainMenuPanel");
        sceneDecoration.SetActive(true);
    }
    public void ShowOptions()  => OptionSelectionService.SelectOption(transform.parent.gameObject, "OptionsPanel");
    public void ShowScoreboard()  => OptionSelectionService.SelectOption(transform.parent.gameObject, "ScoreBoardPanel");
    public void ShowTutorial()
    {

        OptionSelectionService.SelectOption(transform.parent.gameObject, "TutorialPanel");
        sceneDecoration.SetActive(false);

    }



}
