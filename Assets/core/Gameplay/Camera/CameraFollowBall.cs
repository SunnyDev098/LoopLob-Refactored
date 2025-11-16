using Core;
using Gameplay.Player;
using UnityEngine;

/// <summary>
/// Moves the camera vertically to follow the ball only when it is NOT anchored.
/// Assumes the BallController script has an IsAnchored property.
/// </summary>
public class CameraFollowBall : MonoBehaviour
{
    public Transform ball;
    [SerializeField] private float verticalOffset = 10f;
    [SerializeField] private float followSpeed = 10f;

    private BallController ballController;

    private void Awake()
    {
        if (ball != null)
            ballController = ball.GetComponent<BallController>();
    }

    private void LateUpdate()
    {
        if (ball == null || ballController == null)
            return;

        if (!ballController.IsAnchored)
        {
            float currentCamY = transform.position.y;
            float targetY = ball.position.y + verticalOffset;

            // Only update camera if ball is above the current camera position
            if (targetY > currentCamY)
            {
                Vector3 targetPos = new Vector3(
                    transform.position.x,
                    targetY,
                    transform.position.z
                );

                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPos,
                    Time.deltaTime * followSpeed
                );
            }
        }

        if (GameManager.Instance!=null)
        {
           GameManager.Instance.difficulty = ((transform.position.y / 2500) + 0.05f) *1.5f;

        }
    }

    public void SetBall(Transform ballTransform)
    {
        ball = ballTransform;
        ballController = ball.GetComponent<BallController>();
    }
}
