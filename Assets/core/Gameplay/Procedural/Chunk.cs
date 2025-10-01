using Core;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public float height = 50;

    public ChunkManager manager { get; set; }
 
    public float getTop()
    {
        return  transform.position.y+ height/2;
    }
    public float getBottom()
    {
        return transform.position.y + height / 2;
    }
}
