using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraBackGroundHandler : MonoBehaviour
{

    public List<Sprite> BackGroundsList;
    public int CurrentIndex;
    public SpriteRenderer CurrentBackGround;

    private void Awake()
    {

    }
   async void  Start()
    {
      //  await Task.Delay(50);
        CurrentIndex = DataHandler.Instance.GetBackGroundIndex();
        CurrentBackGround.sprite = BackGroundsList[DataHandler.Instance.backGroundIndex];
        Debug.Log(DataHandler.Instance.GetBackGroundIndex());
    }


    public void ChangeBackGround(int index)
    {
        CurrentBackGround.sprite = BackGroundsList[index];
        CurrentIndex = index;
        DataHandler.Instance.SetBackGroundIndex(index);

    }

   
}
