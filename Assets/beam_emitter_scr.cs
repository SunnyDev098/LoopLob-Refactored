using UnityEngine;

public class beam_emitter_scr : MonoBehaviour
{
    // Assign these in the Inspector
    public Sprite[] sprites;
    public float interval = 0.05f;

    private SpriteRenderer sr;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
       // GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume * game_manager_scr.sfx_volume * 3;

        sr = GetComponent<SpriteRenderer>();
        if (sprites != null && sprites.Length > 0)
        {
            sr.sprite = sprites[0];
        }
    }

    void Update()
    {
        // Don't animate if no sprites assigned
        if (sprites == null || sprites.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            currentIndex = (currentIndex + 1) % sprites.Length; // loop to first after last
            sr.sprite = sprites[currentIndex];
        }
    }
}
