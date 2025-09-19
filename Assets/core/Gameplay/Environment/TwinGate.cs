using UnityEngine;
using System.Collections.Generic;
using Gameplay.Player;
using Gameplay.Interfaces;

/// <summary>
/// Moves the ball from one gate to its twin instantly.
/// Can also spawn the twin gates at specific positions.
/// </summary>
public class TwinGate : MonoBehaviour, ITwinGate
{
    [Tooltip("Cooldown to avoid rapid ping-pong teleporting.")]
    [SerializeField] private float teleportCooldown = 0.5f;

    [Header("Gate References")]
    [SerializeField] private Transform thisGate;
    [SerializeField] private Transform otherGate;

    [Header("Spawn Settings")]
    [SerializeField] private float sideX = 5f;
    [SerializeField] private float gateZ = 5f;
    [SerializeField] private float yOffset = 5f;

    private static readonly Dictionary<Transform, float> lastTeleportTimes = new();

    public void OnTeleport(BallController ball)
    {
        if (IsOnCooldown(ball.transform) || otherGate == null)
            return;

        if (otherGate.TryGetComponent<BoxCollider2D>(out var twinCollider))
        {
            twinCollider.enabled = false;
            ball.StartCoroutine(ReenableCollider(twinCollider, 0.05f));
        }

        ball.transform.position = otherGate.position;
        lastTeleportTimes[ball.transform] = Time.time;
    }

    public void Spawn(float y)
    {
        var newGate = Instantiate(this, Vector3.zero, Quaternion.identity);

        if (newGate.thisGate == null || newGate.otherGate == null)
        {
            Debug.LogWarning($"{newGate.name} needs both gate references assigned in Inspector.");
            return;
        }

        bool leftFirst = Random.value < 0.5f;
        newGate.thisGate.position = new Vector3(leftFirst ? -sideX : sideX, y, gateZ);
        newGate.otherGate.position = new Vector3(leftFirst ? sideX : -sideX, y + yOffset, gateZ);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<BallController>(out var ball))
            return;

        OnTeleport(ball);
    }

    private bool IsOnCooldown(Transform obj)
    {
        return lastTeleportTimes.TryGetValue(obj, out var lastTime) &&
               Time.time - lastTime < teleportCooldown;
    }

    private System.Collections.IEnumerator ReenableCollider(Collider2D col, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (col != null)
            col.enabled = true;
    }
}
