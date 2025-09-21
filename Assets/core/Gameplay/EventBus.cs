using System;

namespace Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Global event system for decoupled game communication.
    /// </summary>
    public static class EventBus
    {
        public static event Action<int> OnScoreChanged;
        public static event Action<int> OnCoinCollected;
        public static event Action OnGameStarted;
        public static event Action OnGameOver;
        public static event Action OnPowerUpCollected;
        public static event Action OnPlayerDeath;

        public static void RaiseScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
        public static void RaiseCoinCollected(int totalCoins) => OnCoinCollected?.Invoke(totalCoins);
        public static void RaiseGameStarted() => OnGameStarted?.Invoke();
        public static void RaiseGameOver() => OnGameOver?.Invoke();
        public static void RaisePowerUpCollected() => OnPowerUpCollected?.Invoke();
        public static void RaisePlayerDeath() => OnPlayerDeath?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetEvents()
        {
            OnScoreChanged = null;
            OnCoinCollected = null;
            OnGameStarted = null;
            OnGameOver = null;
            OnPowerUpCollected = null;
            OnPlayerDeath = null;
        }
    }
}
