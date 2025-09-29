using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// Panel controller for claiming a username.
/// Handles UI events, input validation, and delegates claim logic to UsernameClaim.
/// </summary>
public class AuthPanelHandler : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject AuthPanel;
    [SerializeField] private GameObject MainMenu;

    [Header("UI References")]
    [SerializeField] private TMP_Text usernameInputField; // UI text input
    [SerializeField] private Button claimButton;
    [SerializeField] private Button tryLaterButton;
    [SerializeField] private TMP_Text errorText;

    [Header("Logic")]
    [SerializeField] private UsernameClaim usernameClaim;

    private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]{3,16}$");

    private void Awake()
    {
        claimButton.onClick.AddListener(OnClaimClicked);
        tryLaterButton.onClick.AddListener(OnTryLaterClicked);

        ClearError();
    }

    /// <summary>
    /// Validates if the username meets the allowed pattern.
    /// </summary>
    private static bool IsValidUsername(string input) =>
        !string.IsNullOrEmpty(input) && UsernameRegex.IsMatch(input);

    private void ClearError()
    {
        errorText.text = string.Empty;
    }

    private async void OnTryLaterClicked()
    {
        SwitchScene(AuthPanel, MainMenu);
    }

    private async void OnClaimClicked()
    {
        string nameToCheck = usernameInputField.text.Trim();


        if (!IsValidUsername(nameToCheck))
        {
            Debug.Log($"Invalid username attempt: {nameToCheck}");
            errorText.text = "Use 3–16 letters, numbers, or _ (no spaces)";
            return;
        }

        SetClaimButtonInteractable(false);

        bool success = await usernameClaim.ClaimUsername(nameToCheck);

        if (success)
        {
            errorText.text = "Username claimed!";
            gameObject.SetActive(false); // Hide this panel
            // Optionally trigger game start here
        }
        else
        {
            errorText.text = "That name is taken. Try another.";
        }

        SetClaimButtonInteractable(true);
    }

    private void SetClaimButtonInteractable(bool state) =>
        claimButton.interactable = state;

    private static void SwitchScene(GameObject fromScene, GameObject toScene)
    {
        if (fromScene != null) fromScene.SetActive(false);
        if (toScene != null) toScene.SetActive(true);
    }
}
