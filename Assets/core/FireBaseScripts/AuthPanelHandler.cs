using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

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
    [SerializeField] private TMP_InputField usernameInputField; // user can type
   // [SerializeField] private TMP_Text usernameInputField; // UI text input
    [SerializeField] private Button claimButton;
    [SerializeField] private Button tryLaterButton;
    [SerializeField] private TMP_Text errorText;

    [Header("Logic")]
    [SerializeField] private UsernameClaim usernameClaim;
    [SerializeField] private PanelDecider panelDecider;


    private static readonly string[] BlockedWords = {
    // English profanity / harassment
    "fuck", "shit", "bitch", "bastard", "asshole", "dick", "pussy", "cunt", "slut",
    "whore", "faggot", "retard", "moron", "idiot", "jerk", "gay", "boob", "tit",

    // Hate / discrimination
    "nigger", "negro", "chink", "gook", "spic", "kike", "nazi", "hitler",
    "terrorist", "isis", "kkk", "slave", "lynch",

    // Sexual & adult / explicit behavior
    "sex", "porn", "xxx", "fetish", "nude", "naked", "orgasm", "cum", "cock",
    "penis", "vagina", "anus", "sperm", "semen", "virgin", "blowjob", "handjob",
     "dildo", "masturbate",

    // Offensive informal variants
    "wtf", "stfu", "lmao", "lmfao", "omfg", "fml", "suckit", "killyourself",
    "die", "hangyourself", "selfharm",

    // Common abusive derivatives
    "shithead", "fuckface", "motherfucker", "sonofabitch", "asswipe", "asshat",
    "dipshit", "shitbag", "shitlord",

    // Admin / staff / impersonation protection
    "admin", "moderator", "mod", "dev", "developer", "owner", "creator", "staff",

    // Common real‑world slurs (added for robustness)
    "tranny", "lesbo", "dyke", "queer", "homo", "beaner", "cracker", "coon",

    // General disguised spellings
    "fuk", "phuck", "fucc", "shet", "bish", "bich", "dik", "pusy", "kunt", "boobz"
};


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
        panelDecider.ShowMenu();
        DataHandler.Instance.GoScoreboard = false;
    }

    private async void OnClaimClicked()
    {
        string nameToCheck = usernameInputField.text.Trim();

        // Too short or too long
        if (nameToCheck.Length < 3 || nameToCheck.Length > 16)
        {
            errorText.text = "Name must be 3–16 characters long.";
            return;
        }

        // Invalid characters (only letters, numbers, underscore)
        if (!Regex.IsMatch(nameToCheck, @"^[a-zA-Z0-9_]+$"))
        {
            errorText.text = "Only letters, numbers, and _ are allowed.";
            return;
        }

        // Inappropriate word
        foreach (string bad in BlockedWords)
        {
            if (BlockedWords.Any(w => nameToCheck.ToLower().Contains(w)))
            {
                errorText.text = "Inappropriate word! Try something else.";
                return;
            }
        }

        SetClaimButtonInteractable(false);

        bool success = await usernameClaim.ClaimUsername(nameToCheck);

        if (success)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                errorText.text = "Cannot reach Server. Please check your internet.";
            }
            else
            {
                errorText.text = "That name is taken. Try another.";
            }

        }

        SetClaimButtonInteractable(true);
    }


    private void SetClaimButtonInteractable(bool state) =>
        claimButton.interactable = state;

 
}
