using System.Threading.Tasks;
using UnityEngine;

public class PanelDecider : MonoBehaviour
{

    public GameObject popUP;
    public GameObject MainMenu;
   async void   Start()
    {

        // await Task.Delay(100);
        if (PlayerPrefs.HasKey("PlayerUserName"))
        {
            popUP.SetActive(false);
            MainMenu.SetActive(true);
        }
        else if (DataHandler.Instance.firstTime)
        {
            popUP.SetActive(true);
            MainMenu.SetActive(false);
        }
    }


    public void ShowAuth()
    {
        popUP.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void ShowMenu()
    {
        popUP.SetActive(false);
        MainMenu.SetActive(true);
    }
}
