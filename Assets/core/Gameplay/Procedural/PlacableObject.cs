using System.Collections.Generic;
using UnityEngine;
public abstract class PlacableObject
{
    public float safeRadius;
    public float minDifficulty;
    public int difficultyPoint;

    protected PlacableObject(float safeRadius, float minDifficulty, int difficultyPoint)
    {
        this.safeRadius = safeRadius;
        this.minDifficulty = minDifficulty;
        this.difficultyPoint = difficultyPoint;
    }

    public bool InSafeRadius(Vector2 candidatePos, IEnumerable<PlacableObject> others)
    {
        foreach (var other in others)
        {
            float combined = safeRadius + other.safeRadius;
            if (Vector2.Distance(candidatePos, other.GetPosition()) < combined)
                return false;
        }
        return true;
    }

    public abstract Vector2 GetPosition();
}

public class PlanetPlacable : PlacableObject
{
    private readonly Transform t;
    public PlanetPlacable(Transform t) : base(3f, 0f, 1) { this.t = t; }
    public override Vector2 GetPosition() => t.position;
}

public class SpikePlacable : PlacableObject
{
    private readonly Transform t;
    public SpikePlacable(Transform t) : base(2f, 0.0f, 2) { this.t = t; }
    public override Vector2 GetPosition() => t.position;
}

public class BlackHolePlacable : PlacableObject
{
    private readonly Transform t;
    public BlackHolePlacable(Transform t) : base(3f, 0.0f, 5) { this.t = t; }
    public override Vector2 GetPosition() => t.position;
}

public class BeamEmitterPlacable : PlacableObject
{
    private readonly Transform t;
    public BeamEmitterPlacable(Transform t) : base(3f, 0.0f, 4) { this.t = t; }
    public override Vector2 GetPosition() => t.position;
}

public class AlienShipPlacable : PlacableObject
{
    private readonly Transform t;
    public AlienShipPlacable(Transform t) : base(2f, 0.0f, 3) { this.t = t; }
    public override Vector2 GetPosition() => t.position;
}

public class LaserGunPlacable : PlacableObject
{
    private readonly Transform t;
    public LaserGunPlacable(Transform t) : base(2f, 0.0f, 3) { this.t = t; }
    public override Vector2 GetPosition() => t.position;
}

public class RocketLauncherPlacable : PlacableObject
{
    private readonly Transform t;
    public RocketLauncherPlacable(Transform t) : base(0f, 0.0f, 4) { this.t = t; }
    public override Vector2 GetPosition() => t.position;
}
