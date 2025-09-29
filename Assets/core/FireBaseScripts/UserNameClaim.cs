using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UsernameClaim : MonoBehaviour
{
    public GameObject popUp;
    public GameObject authPanel;
    public GameObject mainMenuPanel;

    private const string COLLECTION_USERNAMES = "usernames";
    private const string PREF_USERNAME = "username";

    private FirebaseAuth auth;
    private FirebaseFirestore firestore;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
    }

    private string CleanUsername(string input) =>
        string.IsNullOrEmpty(input)
            ? string.Empty
            : new string(
                input.Where(c =>
                    !char.IsWhiteSpace(c) &&
                    c != '\u200B' && c != '\u200C' &&
                    c != '\u200D' && c != '\uFEFF'
                ).ToArray()
              ).ToLowerInvariant();

    public async Task<bool> ClaimUsername(string rawInput)
    {
        string username = CleanUsername(rawInput);

        if (!await EnsureSignedIn()) return false;
        if (await IsUsernameTaken(username)) return false;

        string token = FireBaseStarter.CurrentFcmToken ?? string.Empty;
        if (!await SaveUsername(username, token)) return false;

        SaveLocally(username);
        ShowSuccess();

        return true;
    }

    private async Task<bool> EnsureSignedIn()
    {
        if (auth.CurrentUser != null) return true;

        try
        {
            await auth.SignInAnonymouslyAsync();
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[FirebaseAuth] Sign-in failed: " + ex);
            return false;
        }
    }

    private async Task<bool> IsUsernameTaken(string username)
    {
        try
        {
            var snapshot = await firestore.Collection(COLLECTION_USERNAMES)
                                            .Document(username)
                                            .GetSnapshotAsync();
            return snapshot.Exists;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[Firestore] Username check failed: " + ex);
            return true; // Assume taken or unavailable
        }
    }

    private async Task<bool> SaveUsername(string username, string token)
    {
        var data = new Dictionary<string, object>()
        {
            { "uid", auth.CurrentUser.UserId },
            { "timestamp", FieldValue.ServerTimestamp },
            { "fcmTokens", string.IsNullOrEmpty(token) ? new List<string>() : new List<string> { token } }
        };

        try
        {
            await firestore.Collection(COLLECTION_USERNAMES)
                           .Document(username)
                           .SetAsync(data);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[Firestore] Save failed: " + ex);
            return false;
        }
    }

    private void SaveLocally(string username)
    {
        PlayerPrefs.SetString(PREF_USERNAME, username);
        PlayerPrefs.Save();
    }

    private void ShowSuccess()
    {
      //  if (pop_up) pop_up.GetComponent<UsernamePanel>().errorText.text = "Username saved!";
        if (authPanel) authPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }
}
