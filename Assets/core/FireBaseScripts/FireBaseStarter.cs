using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Analytics;
using System;
using System.Threading.Tasks;
//using Firebase.Messaging;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class FireBaseStarter : MonoBehaviour
{

    public static string CurrentFcmToken { get; private set; }



    private FirebaseAuth auth;

    async void Start()
    {

        Debug.Log("Startup: waiting for notification permission...");

        //  Wait for notification permission before Firebase init
        await RequestNotificationPermission();

        Debug.Log("Permission flow done. Starting Firebase...");

        //  Firebase init
        await InitFirebase();

       
    }

    private async Task RequestNotificationPermission()
    {
        // Let Unity render at least 1 frame first (prevents freeze)
        await Task.Yield();

#if UNITY_ANDROID
        if (GetAndroidSDKInt() >= 33 &&
            !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            await Task.Delay(500); // Small wait for dialog
        }
#elif UNITY_IOS
        var req = new AuthorizationRequest(
            AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound, true);
        while (!req.IsFinished)
            await Task.Yield();
        Debug.Log("iOS Notification Permission: " + req.Granted);
#endif
    }

    private async Task InitFirebase()
    {
        try
        {
            var depStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (depStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase is ready.");

                // -------- AUTH INIT --------
                auth = FirebaseAuth.DefaultInstance;
                await SignInAnonymously();
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                // -------- MESSAGING INIT --------
                /*
                FirebaseMessaging.MessageReceived += OnMessageReceived;
                FirebaseMessaging.TokenReceived += OnTokenReceived;

                // Fetch current token
                CurrentFcmToken = await FirebaseMessaging.GetTokenAsync();
                Debug.Log("Initial FCM Token: " + CurrentFcmToken);

                // Try initial Firestore update if username already exists
                await UpdateTokenInFirestoreIfUserExists(CurrentFcmToken);

                Debug.Log("Firebase Messaging listeners set.");
                */

            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + depStatus);
            }
        }
        catch (AggregateException agex)
        {
            Debug.LogError("FIREBASE AGGREGATE EXCEPTION:");
            foreach (var ex in agex.InnerExceptions)
            {
                Debug.LogError(ex.Message);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private async Task SignInAnonymously()
    {
        FirebaseUser user = null;
        try
        {
            var signInResult = await auth.SignInAnonymouslyAsync();
            user = signInResult.User;
        }
        catch (FirebaseException ex)
        {
            Debug.LogError(ex.Message);
            Debug.LogError($"Firebase Inner: {ex.InnerException}");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        if (user != null)
        {
            Debug.Log("Signed in as: " + user.UserId);
        }
        else
        {
            Debug.LogError("Anonymous sign-in failed: user is null.");
        }
    }
    /*
    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        var notification = e.Message.Notification;
        string title = notification?.Title ?? "(no title)";
        string body = notification?.Body ?? "(no body)";
        Debug.Log($"Push notification received: {title} - {body}");
    }

    private async void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        CurrentFcmToken = e.Token;
        Debug.Log("FCM Token (updated): " + CurrentFcmToken);

        // Auto-update Firestore if a username exists
        await UpdateTokenInFirestoreIfUserExists(CurrentFcmToken);
    }
   */
    private async Task UpdateTokenInFirestoreIfUserExists(string token)
    {
        if (!string.IsNullOrEmpty(token) && PlayerPrefs.HasKey("username"))
        {
            try
            {
                var username = PlayerPrefs.GetString("username");
                var firestore = FirebaseFirestore.DefaultInstance;
                var userRef = firestore.Collection("usernames").Document(username);

                await userRef.UpdateAsync("fcmTokens", new System.Collections.Generic.List<string> { token });
                Debug.Log("Firestore token updated for username: " + username);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error updating Firestore token: " + ex.Message);
            }
        }
    }
     
#if UNITY_ANDROID
    private int GetAndroidSDKInt()
    {
        using (var versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return versionClass.GetStatic<int>("SDK_INT");
        }
    }
#endif
}
