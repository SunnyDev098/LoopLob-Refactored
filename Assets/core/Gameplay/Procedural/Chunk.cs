using Core;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public float height = 50;

    public ChunkManager manager { get; set; }
    void Start()
    {
        if (GetComponent<PlanetGenerator>() != null)
        {
    
            var theY =  getTop()/5000 ;
            if (theY > 1)
            {
                GetComponent<PlanetGenerator>().difficulty = theY;
            }
            else
            {
                GetComponent<PlanetGenerator>().difficulty = 1;
            }
        }
    }
    public float getTop()
    {
        return  transform.position.y+ height/2;
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(manager != null)
        {
            Gizmos.DrawWireCube(transform.position + Vector3.up * height/2, transform.up);
        }
        else
        {

        }
    }
    */
}
