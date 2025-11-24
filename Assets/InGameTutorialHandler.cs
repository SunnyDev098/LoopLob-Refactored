using Gameplay.Player;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class InGameTutorialHandler : MonoBehaviour
{
    public BallController ballController;

    public GameObject left_hand;
    public GameObject right_hand;
    public GameObject dottedLine;
    public GameObject PowerUps;
    public GameObject ballexplode;


    public GameObject touchAnywhere;

    public int Step = 0;

    public GameObject Arrow;

    private float watingTime;

    private void Awake()
    {
        watingTime = 2f;

    }
    private async void Start()
    {

        if (!DataHandler.Instance.firstTime)
        {
            gameObject.SetActive(false);
            Arrow.SetActive(false);
            ballController.ballEnabled = true;

        }

        else
        {
            PowerUps.SetActive(false);
            ballController.ballEnabled = false;

        }

    }

    private async void Update()
    {


        if(watingTime > 0)
        {


            watingTime -= Time.deltaTime;
            return;
        }

        switch (Step)
        {
            case 0:

                Debug.Log(ballController.gameObject.transform.eulerAngles.z);
                if(ballController.gameObject.transform.eulerAngles.z <315 && ballController.gameObject.transform.eulerAngles.z > 310)
                {
                    Time.timeScale = 0;



                   // ballController.ballEnabled = true;
                    touchAnywhere.SetActive(true);
                    if (userInput() == 1 || userInput() == 2)
                    {

                        ballController.DetachFromAnchor();

                        Step += 1;

                        ballController.ballEnabled = false;

                        Time.timeScale = 1;

                        watingTime = 0.8f;

                        touchAnywhere.SetActive(false);


                    }

                }
             





                break;

            case 1:

                Time.timeScale = 0f;

                dottedLine.SetActive(true);
                left_hand.SetActive(true);
                ballController.ballEnabled = false;

                if (userInput() == 1 )
                {

                    Time.timeScale = 1;



                    ballController.RotateInFreeFlight(true);



                    Step += 1;



                     watingTime = 0.4f;


                }

                break;


            case 2:

                Time.timeScale = 0f;

                dottedLine.SetActive(true);
                left_hand.SetActive(false);
                right_hand.SetActive(true);
                ballController.ballEnabled = false;

                if (userInput() == 2)
                {

                    Time.timeScale = 1;



                    ballController.RotateInFreeFlight(false);

                    dottedLine.SetActive(false);
                    right_hand.SetActive(false);



                    Step += 1;



                    watingTime = 1f;


                }

                break;
            case (3):

                if (userInput() == 1 || userInput() == 2)
                {
                    Step += 1;

                    ballexplode.SetActive(true);

                    watingTime = 3f;
                }




                break;


            case (4):

                ballexplode.SetActive(false);

                ballController.ballEnabled = true;
                  Arrow.SetActive(false);
                  gameObject.SetActive(false);
                  DataHandler.Instance.firstTime = false;
                  PlayerPrefs.SetInt("FirstTime", 0);
                  PlayerPrefs.Save();

                break;




            default:
                break;
        }
    }


    private int userInput()
    {
        // Handle PC mouse input
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width / 2f)
                return 1;
            else
                return 2;
        }

        // Handle mobile touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < Screen.width / 2f)
                    return 1;
                else
                    return 2;
            }
        }

        return 0;
    }


}
