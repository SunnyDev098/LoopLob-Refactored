using UnityEngine;

/// <summary>
/// Base class for all placement objects, purely data: grid spans + position.
/// </summary>
public abstract class PlacableObject
{
    public int horizontalSpan { get; protected set; }
    public int verticalSpan { get; protected set; }

    public abstract Vector2 GetPosition();
}

/// <summary>
/// Static hazard occupying fixed grid cells.
/// </summary>
public class StaticPlacable : PlacableObject
{
    private Vector2 pos;

    public StaticPlacable(Vector2 position, int hSpan, int vSpan)
    {
        pos = position;
        horizontalSpan = hSpan;
        verticalSpan = vSpan;
    }

    public override Vector2 GetPosition() => pos;
}

/// <summary>
/// Planet: spans dynamically adjusted based on scale and movement range.
/// </summary>
public class PlanetPlacable : PlacableObject
{
    private Vector2 pos;

    public PlanetPlacable(Vector2 position, float scale, float moveRangeX, float moveRangeY)
    {
        pos = position;
        horizontalSpan = Mathf.CeilToInt(scale + moveRangeX);
        verticalSpan = Mathf.CeilToInt(scale + moveRangeY);
    }

    public override Vector2 GetPosition() => pos;
}

/// <summary>
/// Alien ship: spans dynamically adjusted based on sprite size and movement range.
/// </summary>
public class AlienShipPlacable : PlacableObject
{
    private Vector2 pos;

    public AlienShipPlacable(Vector2 position, int baseWidth, int baseHeight, float moveRangeX, float moveRangeY)
    {
        pos = position;
        horizontalSpan = Mathf.CeilToInt(baseWidth + moveRangeX);
        verticalSpan = Mathf.CeilToInt(baseHeight + moveRangeY);
    }

    public override Vector2 GetPosition() => pos;
}
