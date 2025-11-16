using UnityEngine;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GhostMessageSubmitter : MonoBehaviour
{
    private FirebaseFirestore db;

    [Header("Message Settings")]
    [SerializeField] private int maxMessageLength = 60;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    /// <summary>
    /// Submits a death message to Firestore.
    /// </summary>
    public async Task SubmitDeathMessageAsync(int deathHeight, string rawMessage)
    {
        string messageText = FormatMessage(rawMessage);
        if (string.IsNullOrEmpty(messageText))
        {
            Debug.LogWarning("Death message is empty!");
            return;
        }

        var deathMessageData = BuildMessageData(messageText, deathHeight);

        try
        {
            await db.Collection("death_messages").AddAsync(deathMessageData);
            Debug.Log("Death message submitted successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error submitting death message: {ex.Message}");
        }
    }

    /// <summary>
    /// Enforces max length and trims whitespace.
    /// </summary>
    private string FormatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return string.Empty;

        message = message.Trim();
        if (message.Length > maxMessageLength)
            message = message.Substring(0, maxMessageLength);

        return message;
    }

    /// <summary>
    /// Builds Firestore document data for a death message.
    /// </summary>
    private Dictionary<string, object> BuildMessageData(string messageText, int deathHeight)
    {
        return new Dictionary<string, object>
        {
            { "text", messageText },
            { "timestamp", FieldValue.ServerTimestamp },
            { "height", deathHeight },
            { "writer_username", DataHandler.Instance.GetUserName() }
        };
    }
}
