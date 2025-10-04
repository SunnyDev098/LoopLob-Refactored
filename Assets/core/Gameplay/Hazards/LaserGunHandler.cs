using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class LaserGunHandler : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip laserShotSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Rotation Settings")]
    [SerializeField] public bool isLeftGun = false ;
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


    private async void Start()
    {
        await Task.Delay(200);

        isLeftGun = Random.value < 0.5f;

        Vector3 pos = transform.position;
        pos.x = isLeftGun ? -5f : 5f;
        transform.position = pos;

        yRotationBase = isLeftGun ? 0 : 180;

        StartCoroutine(RotateAndShootRoutine());

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (isLeftGun && sr != null)
        {
          //  sr.flipX = true;
        }

    }

    private IEnumerator RotateAndShootRoutine()
    {
        while (true)
        {
            targetAngle = Random.Range(minAimAngle, maxAimAngle);
            yield return RotateToAngle(targetAngle);

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
        // Determine shoot direction
        Vector2 shootDirection = isLeftGun ? -transform.right : transform.right;

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
