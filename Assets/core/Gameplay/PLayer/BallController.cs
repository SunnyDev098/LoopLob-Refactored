using UnityEngine;
using Core;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 200f;

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform arrowIndicator;

        private Vector2 moveDirection;
        private bool isAnchored;

        private void Reset()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (!isAnchored)
                MoveBall();
            else
                RotateAroundAnchor();
        }

        private void MoveBall()
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }

        private void RotateAroundAnchor()
        {
            if (arrowIndicator != null)
                arrowIndicator.eulerAngles = transform.position; // placeholder arrow rotation
        }

        private void RotateBall(float amount)
        {
            moveDirection = Quaternion.Euler(0f, 0f, amount) * moveDirection;
        }

        public void AttachToAnchor(Transform anchor)
        {
            isAnchored = true;
            GameManager.Instance.SetBallAttached(true);
            moveDirection = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }

        public void DetachFromAnchor()
        {
            isAnchored = false;
            GameManager.Instance.SetBallAttached(false);

            // Assign some launch direction after detaching
            moveDirection = transform.up.normalized;
            EventBus.RaiseGameStarted();
        }

        public bool IsAnchored => isAnchored;

        public void SetMoveDirection(Vector2 newDir)
        {
            moveDirection = newDir.normalized;
        }

        public Vector2 GetMoveDirection() => moveDirection;

        public void StopMovement()
        {
            moveDirection = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }

        public float RotationSpeed => rotationSpeed;
    }
}
