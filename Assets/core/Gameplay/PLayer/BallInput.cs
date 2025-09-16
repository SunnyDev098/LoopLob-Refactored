using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(BallController))]
    public class BallInput : MonoBehaviour
    {
        private BallController controller;

        private void Awake()
        {
            controller = GetComponent<BallController>();
        }

        private void Update()
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                controller.SetMoveDirection(
                    Quaternion.Euler(0f, 0f, controller.RotationSpeed * Time.deltaTime) *
                    controller.GetMoveDirection()
                );

            if (Input.GetKey(KeyCode.RightArrow))
                controller.SetMoveDirection(
                    Quaternion.Euler(0f, 0f, -controller.RotationSpeed * Time.deltaTime) *
                    controller.GetMoveDirection()
                );
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0) && controller.IsAnchored)
                controller.DetachFromAnchor();
        }
    }
}
