using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IOrbitAnchor
    {
        void OnAnchored(BallController ball);
    }
}
