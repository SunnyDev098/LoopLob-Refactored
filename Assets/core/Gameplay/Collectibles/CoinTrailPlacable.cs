using UnityEngine;
using System.Collections.Generic;

public class CoinTrailPlacable : PlacableObject
{
    private List<Vector2> positions;

    public CoinTrailPlacable(List<Vector2> worldPositions, int hSpan, int vSpan)
    {
        positions = worldPositions;
        horizontalSpan = hSpan;
        verticalSpan = vSpan;
    }

    public override Vector2 GetPosition()
    {
        // Return starting position, but actual path needs special handling
        return positions[0];
    }

    public List<Vector2> GetPositions() => positions;
}
