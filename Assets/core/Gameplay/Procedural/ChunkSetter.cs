using UnityEngine;
using Environment;  // for your ProceduralSettings class

public class ChunkSetter : MonoBehaviour
{
    [SerializeField] private ProceduralSettings config; // Assign in Inspector

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("ChunkSetter: No ProceduralSettings asset assigned!");
            return;
        }

        Vector3 scale = transform.localScale;
        transform.localScale = scale;
    }
}
