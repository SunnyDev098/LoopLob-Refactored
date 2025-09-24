namespace Gameplay.player
{
    using UnityEngine;
    using Core;
    public class GameOverHandler : MonoBehaviour
    {
        [SerializeField] private GameObject explosionVFX;
        [SerializeField] private SpriteRenderer ballSR;
        [SerializeField] private SpriteRenderer auraSR;
        private void HandleGameOverEffects()
        {
            if (explosionVFX != null) explosionVFX.SetActive(true);
            if (ballSR != null) ballSR.enabled = false;
            if (auraSR != null) auraSR.enabled = false;
        }
        private void OnEnable() => EventBus.OnGameOver += HandleGameOverEffects;
        private void OnDisable() => EventBus.OnGameOver -= HandleGameOverEffects;
        
    }

}
