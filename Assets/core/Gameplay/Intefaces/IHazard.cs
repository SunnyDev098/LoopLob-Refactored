using Gameplay.Player;

namespace Gameplay.Interfaces
{
    public interface IHazard
    {
      void OnHit(BallController ball);
    }
}