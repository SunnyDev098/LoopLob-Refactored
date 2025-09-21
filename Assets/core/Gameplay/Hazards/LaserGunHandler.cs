using UnityEngine;
using System.Collections;

public class LaserGunHandler : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip laserShotSound;
    private AudioSource audioSource;
    // some new change2

    [Header("Rotation Settings")]
    [SerializeField] private bool isLeftGun = false;
    [SerializeField] private float minAimAngle = -25f;
    [SerializeField] private float maxAimAngle = 25f;
    [SerializeField] private float rotationSpeed = 40f; // degrees per second

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float timeBetweenShots = 0.15f;
    [SerializeField] private float shotOffsetZ = 1f; // z-offset for spawn position

    private int yRotationBase;
    private bool isRotating = false;
    private float targetAngle;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
          //  audioSource.volume = game_manager_scr.sfx_volume * 3f;
        }

        yRotationBase = isLeftGun ? 0 : 180;
    }

    private void Start()
    {
        StartCoroutine(RotateAndShootRoutine());
    }

    private IEnumerator RotateAndShootRoutine()
    {
        while (true)
        {
            targetAngle = Random.Range(minAimAngle, maxAimAngle);

            yield return RotateToAngle(targetAngle);

            // Could be multiple shots, but set to 1 for now
            FireProjectile();
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private IEnumerator RotateToAngle(float target)
    {
        isRotating = true;

        yield return new WaitForSeconds(0.5f);

        float current = NormalizeAngle(transform.eulerAngles.z);
        while (Mathf.Abs(Mathf.DeltaAngle(current, target)) > 1f)
        {
            current = NormalizeAngle(transform.eulerAngles.z);
            float step = rotationSpeed * Time.deltaTime;
            float newAngle = Mathf.MoveTowardsAngle(current, target, step);

            transform.eulerAngles = new Vector3(0, yRotationBase, newAngle);
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, yRotationBase, target);
        isRotating = false;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null)
            return;

        Vector3 spawnPos = transform.position + new Vector3(0, 0, shotOffsetZ);
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, transform.rotation);

        if (audioSource != null && laserShotSound != null)
        {
            audioSource.PlayOneShot(laserShotSound);
        }

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = transform.right; // "forward" in Unity 2D

        if (rb != null)
        {
            rb.linearVelocity = shootDirection * projectileSpeed;
        }
        else
        {
            StartCoroutine(MoveAndDestroy(projectile, shootDirection));
        }

        Destroy(projectile, 5f);
    }

    private IEnumerator MoveAndDestroy(GameObject obj, Vector2 direction)
    {
        float timer = 0f;
        while (timer < 5f && obj != null)
        {
            obj.transform.position += (Vector3)(direction * projectileSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
