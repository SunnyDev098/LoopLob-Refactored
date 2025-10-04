using Core;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public float height = 50;
    public ChunkManager manager { get; set; }

    public float getTop()
    {
        return transform.position.y + height / 2;
    }

    public float getBottom()
    {
        return transform.position.y - height / 2;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Set Gizmo color
        Gizmos.color = Color.green;

        // Chunk center
        Vector3 center = transform.position;

        // Size: Width = 10, Height = variable height
        Vector3 size = new Vector3(10f, height, 0.1f);

        // Draw rectangle for chunk bounds
        Gizmos.DrawWireCube(center, size);

        // Optionally draw top & bottom edges as lines
        Vector3 topEdgeLeft = new Vector3(center.x - 5f, getTop(), center.z);
        Vector3 topEdgeRight = new Vector3(center.x + 5f, getTop(), center.z);

        Vector3 bottomEdgeLeft = new Vector3(center.x - 5f, getBottom(), center.z);
        Vector3 bottomEdgeRight = new Vector3(center.x + 5f, getBottom(), center.z);

        Gizmos.DrawLine(topEdgeLeft, topEdgeRight);
        Gizmos.DrawLine(bottomEdgeLeft, bottomEdgeRight);
    }
#endif
}
