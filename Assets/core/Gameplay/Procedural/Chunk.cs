using Core;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public float height = 50;

    public ChunkManager manager { get; set; }
    void Start()
    {
        if (GetComponent<ChunkGenerator>() != null)
        {
    
            var theY =  getTop()/5000 ;
            if (theY > 1)
            {
                GetComponent<ChunkGenerator>().difficulty = theY;
            }
            else
            {
                GetComponent<ChunkGenerator>().difficulty = 1;
            }
        }
    }
    public float getTop()
    {
        return  transform.position.y+ height/2;
    }
   
}
