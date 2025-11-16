namespace Gameplay.player
{
    using UnityEngine;
    using Core;
    using System.Collections.Generic;
    using System.Collections;

    public class GameOverHandler : MonoBehaviour
    {
        [SerializeField] private GameObject explosionVFX;
        [SerializeField] private SpriteRenderer ballSR;
        [SerializeField] private SpriteRenderer auraSR;






        [SerializeField] private List<Sprite> sprites;
        [SerializeField] private float interval = 0.1f; // seconds between frames

        private void HandleGameOverEffects()
        {
            if (explosionVFX != null) explosionVFX.SetActive(true);
           // if (ballSR != null) ballSR.enabled = false;
            if (auraSR != null) auraSR.enabled = false;
            StartCoroutine(CycleSprites());
        }
        private void OnEnable() => EventBus.OnGameOver += HandleGameOverEffects;
        private void OnDisable() => EventBus.OnGameOver -= HandleGameOverEffects;


        private IEnumerator CycleSprites()
        {
            if (sprites == null || sprites.Count == 0)
                yield break;

            int index = 0;
            int cycles = 0;
            
            while (cycles <8)
            {
                ballSR.sprite = sprites[index];
                index = (index + 1) % sprites.Count;
                yield return new WaitForSeconds(interval);
                cycles++;
            }
        }
    }

}
