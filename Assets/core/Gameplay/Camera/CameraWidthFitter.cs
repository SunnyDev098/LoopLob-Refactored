using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Adjusts orthographic size to always cover a fixed horizontal width in world units.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraWidthFitter : MonoBehaviour
{
    [SerializeField] private float desiredHalfWidth = 10f; // Half of full width to view
    public Transform TopBar ; // TopBarTransform to use in rocket launcher
    private Camera cam;

    private async void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustSize();
        await Task.Delay(100);
        TopBarSetter();
    }

  
    private void AdjustSize()
    {
        // Orthographic size is "half height", height = width / aspect
        cam.orthographicSize = desiredHalfWidth / cam.aspect;
    }

    private void TopBarSetter()
    {
        float topEdge = Camera.main.transform.position.y + Camera.main.orthographicSize;
        TopBar.transform.position = new Vector2(0, topEdge);
    }
}
