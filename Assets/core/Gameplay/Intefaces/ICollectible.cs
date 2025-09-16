using Gameplay.Player;

namespace Gameplay.Interfaces
{
    public interface ICollectible
    {
        void OnCollected(BallController ball);
    }
}