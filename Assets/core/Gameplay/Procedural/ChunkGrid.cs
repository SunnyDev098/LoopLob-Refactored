using UnityEngine;
using System.Collections.Generic;

public class ChunkGrid
{
    public Vector2 Origin { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float CellSize { get; private set; }

    // Tracks cells that are still empty
    private HashSet<Vector2Int> availableCells;
    public IReadOnlyCollection<Vector2Int> AvailableCells => availableCells;

    public ChunkGrid(Vector2 origin, int cols, int rows, float cellSize)
    {
        Origin = origin;
        Width = cols;
        Height = rows;
        CellSize = cellSize;

        availableCells = new HashSet<Vector2Int>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                availableCells.Add(new Vector2Int(x, y));
            }
        }
    }

    public Vector2 CellToWorld(Vector2Int cell)
    {
        float worldX = Origin.x + (cell.x + Random.RandomRange(1 ,3)) * CellSize; 
        float worldY = Origin.y + (cell.y + Random.RandomRange(1, 3)) * CellSize;
        return new Vector2(worldX, worldY);
    }

    public bool CanPlaceAt(PlacableObject obj, Vector2Int startCell, Vector2Int? rangeClampX = null)
    {
        // Basic bounds
        if (startCell.x < 0 || startCell.x + obj.horizontalSpan > Width)
            return false;
        if (startCell.y < 0 || startCell.y + obj.verticalSpan > Height)
            return false;

        // Optional horizontal clamp
        if (rangeClampX.HasValue)
        {
            int minAllowed = rangeClampX.Value.x;
            int maxAllowed = rangeClampX.Value.y;
            if (startCell.x < minAllowed || (startCell.x + obj.horizontalSpan - 1) > maxAllowed)
                return false;
        }

        // Check occupancy
        for (int dx = 0; dx < obj.horizontalSpan; dx++)
        {
            for (int dy = 0; dy < obj.verticalSpan; dy++)
            {
                Vector2Int check = new Vector2Int(startCell.x + dx, startCell.y + dy);
                if (!availableCells.Contains(check))
                    return false;
            }
        }
        return true;
    }

    public bool TryPlaceAt(PlacableObject obj, Vector2Int startCell, out Vector2 finalWorldPos, Vector2Int? rangeClampX = null)
    {
        if (!CanPlaceAt(obj, startCell, rangeClampX))
        {
            finalWorldPos = Vector2.zero;
            return false;
        }

        // Reserve cells
        for (int dx = 0; dx < obj.horizontalSpan; dx++)
        {
            for (int dy = 0; dy < obj.verticalSpan; dy++)
            {
                availableCells.Remove(new Vector2Int(startCell.x + dx, startCell.y + dy));
            }
        }

        finalWorldPos = CellToWorld(startCell);
        return true;
    }

    // Fisher–Yates shuffle
    public static class ShuffleUtil
    {
        public static void ShuffleInPlace<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
