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
    public Transform BottomBar ; 
    private Camera cam;

    private  void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustSize();
    }
    private void Start()
    {
        BarSetter();

    }

    private void AdjustSize()
    {
        // Orthographic size is "half height", height = width / aspect
        cam.orthographicSize = desiredHalfWidth / cam.aspect;
    }

    private void BarSetter()
    {
        float topEdge = Camera.main.transform.position.y + Camera.main.orthographicSize;
        TopBar.transform.position = new Vector2(0, topEdge);
        float BottomEdge = Camera.main.transform.position.y - Camera.main.orthographicSize;
        BottomBar.transform.position = new Vector2(0, BottomEdge);
    }
}
