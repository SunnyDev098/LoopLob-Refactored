using Gameplay.player;
using UnityEngine;

namespace Gameplay.Player
{
    public class BallInput : MonoBehaviour
    {
        private BallController controller;
        private SpritePulseController spritePulseController;
        private BallAudioHandler ballAudioHandler;


        private void Awake()
        {
            controller = GetComponent<BallController>();
            spritePulseController = GetComponent<SpritePulseController>();
            ballAudioHandler = GetComponent<BallAudioHandler>();

        }

        private void Update()
        {
            HandleRotationInput();
            HandleDetachInput();
        }

        private void HandleRotationInput()
        {
            bool leftKey = Input.GetKeyDown(KeyCode.LeftArrow);
            bool rightKey = Input.GetKeyDown(KeyCode.RightArrow);
            bool leftClick = Input.GetMouseButtonDown(0) && Input.mousePosition.x < Screen.width / 2f;
            bool rightClick = Input.GetMouseButtonDown(0) && Input.mousePosition.x >= Screen.width / 2f;

            if (leftKey || leftClick)
            {
                controller.RotateInFreeFlight(true);
                spritePulseController.PlayPulse(0);
                ballAudioHandler.playRotation();

            }


            if (rightKey || rightClick)
            {
                controller.RotateInFreeFlight(false);
                spritePulseController.PlayPulse(1);
                ballAudioHandler.playRotation();

            }


        }

        private void HandleDetachInput()
        {
            if (Input.GetMouseButtonDown(0) && controller.IsAnchored)
                controller.DetachFromAnchor();
        }

       
    }
}
