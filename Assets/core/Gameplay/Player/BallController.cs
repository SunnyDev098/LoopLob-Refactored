using UnityEngine;
using Core;
using Gameplay.player;
using UnityEngine.UIElements;

namespace Gameplay.Player
{
    public class BallController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] public float moveSpeed = 20f;       // Linear speed after release
        [SerializeField] private float rotationSpeed = 200; // Orbit speed in degrees/sec
        [SerializeField] private float freeBallrotationSpeed = 200; // Orbit speed in degrees/sec

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;

        // Orbit state
        private bool isAnchored;
        public Transform anchorPlanet;
        private float totalRotation;
        private float orbitRadius;
        private float bounceXLimit = 5;
        // Movement
        public Vector2 moveDirection;   // current free-flight direction
        private Vector2 tangentDirection; // tangent at detach moment


        [SerializeField] private AuraHandler auraHandler;
        [SerializeField] private BallAudioHandler ballAudioHandler;


        public bool ballEnabled = true;
        public float lastAnchorTime;
       // public bool clockWise = true;
        private void Reset()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            orbitRadius = 1;

            isAnchored = true;

            lastAnchorTime = Time.time;
        }

        private void Update()
        {
            if (GameManager.Instance.IsGamePaused()) return;
            if (!isAnchored)
            {
                // Transform-based free flight movement
                transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
                CheckHorizontalBounce();
            }
        }

        private void FixedUpdate()
        {
            if (isAnchored)
                RotateAroundAnchor();
        }

        // ———————————————————————— Movement ———————————————————————— //

        private void RotateAroundAnchor()
        {
            if (!anchorPlanet) return;
            if (GameManager.Instance.IsGamePaused()) return;

            float rotationAmount = rotationSpeed * Time.fixedDeltaTime;

            // Ensure exact orbit radius is maintained
            if (orbitRadius <= 0f)
            {
                // Calculate once on first frame of anchor:
                // Distance from ball to planet is the starting orbit radius
                orbitRadius = Vector2.Distance(transform.position, anchorPlanet.position);
            }


            transform.RotateAround(anchorPlanet.position, Vector3.back, rotationAmount);
            totalRotation += rotationAmount;
            // Keep radius locked (correct any drift over time)
            Vector3 dir = (transform.position - anchorPlanet.position).normalized;
            transform.position = anchorPlanet.position + dir * orbitRadius;

            // Tangent direction for when we detach
            tangentDirection = new Vector2(-dir.y, dir.x).normalized;
            /*
            // Orbit around planet
            if (clockWise)
            {
                transform.RotateAround(anchorPlanet.position, Vector3.back, rotationAmount);
                totalRotation += rotationAmount;
                // Keep radius locked (correct any drift over time)
                Vector3 dir = (transform.position - anchorPlanet.position).normalized;
                transform.position = anchorPlanet.position + dir * orbitRadius;

                // Tangent direction for when we detach
                tangentDirection = new Vector2(-dir.y, dir.x).normalized;
            }
            else
            {
                transform.RotateAround(anchorPlanet.position, Vector3.forward, rotationAmount);
                totalRotation += rotationAmount;
                // Keep radius locked (correct any drift over time)
                Vector3 dir = (transform.position - anchorPlanet.position).normalized;
                transform.position = anchorPlanet.position + dir * orbitRadius;

                // Tangent direction for when we detach
                tangentDirection = new Vector2(-dir.y, dir.x).normalized;

            }
            */



        }
        // ———————————————————————— Bounce Logic ———————————————————————— //

        private void CheckHorizontalBounce()
        {
            Vector3 pos = transform.position;

            if (pos.x >= bounceXLimit )
            {
                moveDirection.x = -moveDirection.x;
                transform.position += new Vector3(-0.25f, moveDirection.y*0.1f, 0);
                ballAudioHandler.playBounce();

                float angle = Mathf.Atan2(-moveDirection.y, -moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            else if (pos.x <= -bounceXLimit)
            {
                moveDirection.x = -moveDirection.x;
                transform.position += new Vector3(0.25f, moveDirection.y * 0.1f, 0);
                ballAudioHandler.playBounce();

                float angle = Mathf.Atan2(-moveDirection.y, -moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

            }
        }
        // ———————————————————————— Public Actions ———————————————————————— //

        public void AttachToAnchor(Transform anchor)
        {

            Debug.Log(transform.eulerAngles.z);
            /*
            if(Mathf.Abs(transform.eulerAngles.z+90) < 180)
            {
                clockWise = true;
            }
            else
            {
                clockWise = false;

            }
            */
            anchorPlanet = anchor;
            isAnchored = true;
            totalRotation = 0f;

            var planetAttr = anchorPlanet.GetComponent<PlanetAttribute>();
            planetAttr.IsAnchoreToBall = true;

            moveDirection = Vector2.zero;
            rb.linearVelocity = Vector2.zero;

            OnBallAnchoredToPlanet();
            GameManager.Instance.SetBallAttached(true);
            ballAudioHandler.playAttach();
            lastAnchorTime = Time.time;

            Vector2 toAnchor = anchor.position - transform.position;


            Vector3 dir = (transform.position - anchorPlanet.position).normalized;
            transform.position = anchorPlanet.position + dir * orbitRadius;

            // Tangent direction for when we detach
            tangentDirection = new Vector2(-dir.y, dir.x).normalized;

            moveDirection = -tangentDirection;

        

            float angle = Mathf.Atan2(-moveDirection.y, -moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        public void DetachFromAnchor()
        {
            if (!anchorPlanet) return;


            if (lastAnchorTime + 0.1f > Time.time) return;

            moveDirection = -tangentDirection;

            /*
            if (clockWise)
            {
                moveDirection = -tangentDirection;

            }
            else
            {
                moveDirection = tangentDirection;

            }
            */

            isAnchored = false;
            OnBallReleasedFromPlanet();
            anchorPlanet.GetComponent<PlanetAttribute>().IsAnchoreToBall = false;
            anchorPlanet.GetComponent<PlanetAttribute>().HideOrbitVisual();
            anchorPlanet = null;

            float angle = Mathf.Atan2(-moveDirection.y,- moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);


            GameManager.Instance.SetBallAttached(false);
            EventBus.RaiseGameStarted();
        }

        public void AnchorTo(Transform planetTransform)
        {
            if (!isAnchored)
                AttachToAnchor(planetTransform);

        }
        public void GetCoin()
        {

            GameManager.Instance.AddCoin(1);
            ballAudioHandler.playCoin();
        }
        public void RotateInFreeFlight(bool toLeft)
        {
            if (isAnchored || GameManager.Instance.IsGamePaused()) return;
            float delta; 
            if (ballEnabled)
            {
                 delta = (toLeft ? 1f : -1f) * freeBallrotationSpeed * Time.deltaTime;

            }
            else
            {
                 delta = (toLeft ? 1f : -1f) * freeBallrotationSpeed * Time.unscaledDeltaTime;

            }



            moveDirection = Quaternion.Euler(0f, 0f, delta) * moveDirection;

            float angle = Mathf.Atan2(-moveDirection.y, -moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }


        public void StopMovement()
        {
            moveDirection = Vector2.zero;
        }

        private void OnBallReleasedFromPlanet()
        {
            auraHandler.StartAura();
        }

        private void OnBallAnchoredToPlanet()
        {
            auraHandler.ResetAura();
        }
        // ———————————————————————— Getters/Setters ———————————————————————— //

        public void SetMoveDirection(Vector2 newDir) => moveDirection = newDir.normalized;
        public void SetOrbitRadius(float radius) => orbitRadius = radius;
        public Vector2 GetMoveDirection() => moveDirection;

        public bool IsAnchored => isAnchored;
        public float RotationSpeed => rotationSpeed;
        public float TotalRotation => totalRotation;
    }
}
