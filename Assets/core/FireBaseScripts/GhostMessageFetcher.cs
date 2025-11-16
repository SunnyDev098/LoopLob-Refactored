using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using TMPro;
using UnityEngine.UI;

public class GhostMessageFetcher : MonoBehaviour
{
    [Header("Firestore Settings")]
    private FirebaseFirestore db;

    [Header("Billboard Prefab")]
    [Tooltip("Prefab with TMP or UI Text components for username and message")]
    [SerializeField] private GameObject deathMessageBillboardPrefab;

    [SerializeField] private Vector3 spawnBasePosition = Vector3.zero;

    [Header("Billboard Child References")]
    [Tooltip("Child containing the username text")]
    [SerializeField] private int usernameChildIndex = 5;
    [Tooltip("Child containing the message text")]
    [SerializeField] private int messageChildIndex = 6;

    private async void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        await FetchAndDisplayMessages();
    }

    /// <summary>
    /// Fetches last 10 death messages from Firestore and spawns billboards.
    /// </summary>
    private async Task FetchAndDisplayMessages()
    {
        try
        {
            var snapshot = await db.Collection("death_messages")
                                   .OrderByDescending("timestamp")
                                   .Limit(10)
                                   .GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                Debug.Log("No death messages found.");
                return;
            }

            foreach (var doc in snapshot.Documents)
            {
                string text = GetFieldOrDefault(doc, "text", "N/A");
                string username = GetFieldOrDefault(doc, "writer_username", "Unknown");
                int height = GetFieldOrDefault(doc, "height", 0);

                SpawnBillboard(text, username, height);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error fetching death messages: {ex.Message}");
        }
    }

    /// <summary>
    /// Safely gets a field value from a Firestore document, or returns default if missing.
    /// </summary>
    private T GetFieldOrDefault<T>(DocumentSnapshot doc, string fieldName, T defaultValue)
    {
        return doc.ContainsField(fieldName) ? doc.GetValue<T>(fieldName) : defaultValue;
    }

    /// <summary>
    /// Instantiates billboard prefab and fills in text fields.
    /// </summary>
    private void SpawnBillboard(string messageText, string username, int height)
    {
        Vector3 position = spawnBasePosition + Vector3.up * height;
        var billboard = Instantiate(deathMessageBillboardPrefab, position, Quaternion.identity);

        SetTextOnChild(billboard, usernameChildIndex, username);
        SetTextOnChild(billboard, messageChildIndex, messageText);

        Debug.Log($"Spawned billboard for: {username} - '{messageText}' at height {height}");
    }

    /// <summary>
    /// Sets TMP or UI Text component text on the target child index, if found.
    /// </summary>
    private void SetTextOnChild(GameObject parent, int childIndex, string text)
    {
        if (parent.transform.childCount <= childIndex)
        {
            Debug.LogWarning($"Child index {childIndex} out of range for {parent.name}");
            return;
        }

        Transform child = parent.transform.GetChild(childIndex);
        if (child.TryGetComponent(out TextMeshProUGUI tmp))
        {
            tmp.text = text;
        }
        else if (child.TryGetComponent(out Text uiText))
        {
            uiText.text = text;
        }
        else
        {
            Debug.LogWarning($"No text component found on child index {childIndex} of {parent.name}");
        }
    }
}
