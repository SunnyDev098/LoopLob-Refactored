using System.Threading.Tasks;
using UnityEngine;

public class PanelDecider : MonoBehaviour
{

    public GameObject AuthPanel;
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
        else
        {
            popUP.SetActive(true);
            MainMenu.SetActive(false);
        }
    }

   
}
