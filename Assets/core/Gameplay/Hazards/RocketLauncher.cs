namespace Gameplay.player {

    using UnityEngine;
    using System.Collections;
    using Core;
    using Gameplay.Interfaces;
    using Gameplay.Player;

    /// <summary>
    /// Enemy that shows a warning sign before launching a missile downward.
    /// </summary>
    public class RocketLauncher : MonoBehaviour,IHitBall
    {
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip warningSfx;
        [SerializeField, Range(0f, 5f)] private float sfxVolumeMultiplier = 3f;

        [Header("Prefabs")]
        [SerializeField] private GameObject missileSignPrefab;
        [SerializeField] private GameObject missilePrefab;

        [Header("References")]
        private Transform topBar;

        [Header("Missile Settings")]
        [SerializeField] private float warningFlashInterval = 0.12f;
        [SerializeField] private int warningFlashes = 5;
        [SerializeField] private float missileSpeed = 25f;
        [SerializeField] private float missileLifetime = 4f;
        [SerializeField] private Vector3 warningYOffset = new Vector3(0, -0.9f, 10);
        [SerializeField] private Vector3 missileYOffset = new Vector3(0, 3f, 10);

        private bool IsTriggered = false;
        private bool IsLaunched = false;
        private void Start()
        {
            if (audioSource != null)
                audioSource.volume = GameManager.Instance.GetSfxVolume() * sfxVolumeMultiplier;

            topBar = GameManager.Instance.TopBar;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        private void Update()
        {


            if (IsLaunched) return;

            if (!IsTriggered) return;
            
            IsLaunched=true;
            LaunchMissile();

        }
        public void LaunchMissile()
        {
            StartCoroutine(LaunchMissileRoutine());
        }

        private IEnumerator LaunchMissileRoutine()
        {
            // Choose random X between left and right bars
            float xPos = Random.Range(GameManager.Instance.Ball.transform.position.x-1 , GameManager.Instance.Ball.transform.position.x +1 );

            // Create warning sign under top bar
            GameObject sign = Instantiate(missileSignPrefab, topBar.transform);
            sign.transform.localPosition = warningYOffset + new Vector3(xPos - topBar.transform.position.x, 0, 0);

            // Flash warning repeatedly
            for (int i = 0; i < warningFlashes; i++)
            {
                sign.SetActive(true);
                PlayWarningSfx();
                yield return new WaitForSeconds(warningFlashInterval);
                sign.SetActive(false);
                yield return new WaitForSeconds(warningFlashInterval);
            }

            // Spawn missile
            GameObject missile = Instantiate(missilePrefab);
            missile.transform.position = new Vector3(xPos, topBar.transform.position.y, 0) + missileYOffset;
            missile.transform.rotation = Quaternion.Euler(0, 0, 180);

            // Adjust missile audio volume
            var missileAudio = missile.GetComponent<AudioSource>();
            if (missileAudio != null)
                missileAudio.volume = GameManager.Instance.GetSfxVolume() * sfxVolumeMultiplier;

            // Move missile down over time
            float elapsed = 0f;
            while (elapsed < missileLifetime && missile != null)
            {
                missile.transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Cleanup
            if (missile != null) Destroy(missile);
            if (sign != null) Destroy(sign);
        }

        private void PlayWarningSfx()
        {
            if (audioSource != null && warningSfx != null)
                audioSource.PlayOneShot(warningSfx);
        }
        public void OnHitBall(BallController ballController)
        {
           IsTriggered = true;
        }

    }

}

