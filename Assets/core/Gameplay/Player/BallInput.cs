using Gameplay.player;
using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(BallController))]
    public class BallInput : MonoBehaviour
    {
        private BallController controller;
        private SpritePulseController spritePulseController;


        private void Awake()
        {
            controller = GetComponent<BallController>();
            spritePulseController = GetComponent<SpritePulseController>();

        }

        private void Update()
        {
            HandleRotationInput();
            HandleDetachInput();
        }

        private void HandleRotationInput()
        {
            bool leftKey = Input.GetKey(KeyCode.LeftArrow);
            bool rightKey = Input.GetKey(KeyCode.RightArrow);
            bool leftClick = Input.GetMouseButtonDown(0) && Input.mousePosition.x < Screen.width / 2f;
            bool rightClick = Input.GetMouseButtonDown(0) && Input.mousePosition.x >= Screen.width / 2f;

            if (leftKey || leftClick)
            {
                controller.RotateInFreeFlight(true);
                spritePulseController.PlayPulse(0);
            }
                

            if (rightKey || rightClick)
            {
                controller.RotateInFreeFlight(false);
                spritePulseController.PlayPulse(1);
            }
                

        }

        private void HandleDetachInput()
        {
            if (Input.GetMouseButtonDown(0) && controller.IsAnchored)
                controller.DetachFromAnchor();
        }

       
    }
}
