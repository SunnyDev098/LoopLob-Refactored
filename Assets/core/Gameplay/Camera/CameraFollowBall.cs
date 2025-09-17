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
            Vector3 targetPos = new Vector3(
                transform.position.x,
                ball.position.y + verticalOffset,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSpeed
            );
        }
    }

    public void SetBall(Transform ballTransform)
    {
        ball = ballTransform;
        ballController = ball.GetComponent<BallController>();
    }
}
