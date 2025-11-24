using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles tutorial page navigation.
/// Attach to a parent UI object that holds all tutorial pages as children.
/// </summary>
public class TutorialUIManager : MonoBehaviour
{
    [Header("Page Containers (ordered)")]
    [SerializeField] private List<GameObject> pages = new List<GameObject>();

    [Header("Navigation Buttons")]
    [SerializeField] private Button inGameTutorial;


    [SerializeField] private Button menuButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private MainMenuHandler mainMenuHandler;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pageFlip;



    public GameObject SceneDecoration;
    private int currentPage = 0;

    private void Awake()
    {
        // Wire up listeners
        if (menuButton) menuButton.onClick.AddListener(OnMainMenuClick);
        if (nextButton) nextButton.onClick.AddListener(OnNextClicked);
        if (backButton) backButton.onClick.AddListener(OnBackClicked);
        if (inGameTutorial) inGameTutorial.onClick.AddListener(OnInGameTutorialClicked);

        ShowPage(0); 

       // MusicPlayer.StopMusic();

    }
    private void Start()
    {
        SceneDecoration.SetActive(false);
    }
    private void ShowPage(int pageIndex)
    {
        // Bounds safety
        if (pageIndex < 0 || pageIndex >= pages.Count)
        {
            Debug.LogWarning("Invalid page index: " + pageIndex);
            return;
        }

        currentPage = pageIndex;

        // Activate only the current page
        for (int i = 0; i < pages.Count; i++)
            pages[i].SetActive(i == currentPage);

        // Update buttons availability based on page
        bool isFirst = currentPage == 0;
        bool isLast = currentPage == pages.Count - 1;

        if (nextButton) nextButton.gameObject.SetActive(!isLast);
        if (backButton) backButton.gameObject.SetActive(!isFirst);

        if (menuButton) menuButton.gameObject.SetActive(true);
    }

    private void OnNextClicked()
    {
        int nextIndex = currentPage + 1;
        if (nextIndex < pages.Count)
            ShowPage(nextIndex);
        audioSource.PlayOneShot(pageFlip);
    }

    private void OnBackClicked()
    {
        int prevIndex = currentPage - 1;
        if (prevIndex >= 0)
            ShowPage(prevIndex);
        audioSource.PlayOneShot(pageFlip);


    }

    private async void OnInGameTutorialClicked()
    {
        DataHandler.Instance.firstTime = true;

        mainMenuHandler.ShowMainMenu();
        SceneDecoration.SetActive(true);


        MusicPlayer.ResumeMusic();
        SceneManager.LoadScene("CoreGame", LoadSceneMode.Single);


    }
    private void OnMainMenuClick()
    {
        mainMenuHandler.ShowMainMenu();
        SceneDecoration.SetActive(true);


        MusicPlayer.ResumeMusic();
    }



    }
