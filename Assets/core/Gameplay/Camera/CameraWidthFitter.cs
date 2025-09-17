using UnityEngine;

/// <summary>
/// Adjusts orthographic size to always cover a fixed horizontal width in world units.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraWidthFitter : MonoBehaviour
{
    [SerializeField] private float desiredHalfWidth = 8f; // Half of full width to view
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustSize();
    }

    private void LateUpdate()
    {
        AdjustSize();
    }

    private void AdjustSize()
    {
        // Orthographic size is "half height", height = width / aspect
        cam.orthographicSize = desiredHalfWidth / cam.aspect;
    }
}
