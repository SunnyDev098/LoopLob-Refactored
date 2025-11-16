using UnityEngine;

public class OrbitOnLine : MonoBehaviour
{
    public Transform center;   // The circle's center
    public Transform target;   // The moving target to track
    public float radius = 5f;  // Fixed orbit radius

    private void Update()
    {
        if (center == null || target == null) return;

        // 1. Direction from center to target
        Vector3 dir = (target.position - center.position).normalized;

        // 2. Position middle object exactly radius units from center in target’s direction
        Vector3 newPos = center.position + dir * radius;

        // 3. Apply position to our middle object
        transform.position = newPos;
    }
}
