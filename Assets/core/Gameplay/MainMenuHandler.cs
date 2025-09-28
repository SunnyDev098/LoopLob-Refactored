using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuHandler : MonoBehaviour
{
    public Button PlayButton;
    public Button OptionsButton;
    

    private void Awake()
    {
        if (PlayButton != null) PlayButton.onClick.AddListener(OnPlayClicked);
        if (OptionsButton != null) OptionsButton.onClick.AddListener(OnOptionClicked);
     
    }

    private void OnDestroy()
    {
        if (PlayButton != null) PlayButton.onClick.RemoveListener(OnPlayClicked);
        if (OptionsButton != null) OptionsButton.onClick.RemoveListener(OnOptionClicked);
        
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene("CoreGame", LoadSceneMode.Single);
    }

    private void OnOptionClicked()
    {
        Debug.Log("Pause button clicked!");
    }

    
}
