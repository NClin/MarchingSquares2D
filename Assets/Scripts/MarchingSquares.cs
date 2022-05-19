using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public struct MarchingPoint
{
    public float Value;
    public Vector2Int gridPosition;
}

/// <summary>
/// Index begins at top left and proceeds clockwise:
/// 
/// 0 is top left
/// 1 is top right,
/// 2 is bottom right,
/// 4 is bottom left.
/// </summary>
public struct MarchingSquare
{
    private bool[] Square;

    public MarchingSquare(bool[] square)
    {
        if (square.Length != 4)
        {
            throw new Exception("Square must be 4 nodes");
        }
        Square = square;
    }

    public string hashString()
    {
        string hash = "";
        for (int i = 0; i < 4; i++)
        {
            if (Square[i]) hash += 1;
            else hash += 0;
        }

        return hash;
    }
} 



public class MarchingSquares
{
    private MarchingPoint[,] grid;

    public MarchingSquares(float[,] values)
    {
        SetGrid(values);
    }
    private void SetGrid(float[,] values)
    {
        int width = values.GetLength(0);
        int height = values.GetLength(1);
        grid = new MarchingPoint[width, height];

        // this feels clumsy but is it ok?
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].Value = values[x, y];
                grid[x, y].gridPosition = new Vector2Int(x, y);
            }   
        }
    }

    public float GetValue(Vector2Int position)
    {
        return grid[position.x, position.y].Value;
    }

    public void SetValue(Vector2Int position, float value)
    {
        grid[position.x, position.y].Value = value;
    }

    /// <summary>
    /// Asks if the value of a given position is below a certain threshold.
    /// Values below threshold are closed.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="threshold">values above threshold are closed</param>
    /// <returns></returns>
    public bool IsOpen(Vector2Int position, float threshold)
    {
        if (grid[position.x, position.y].Value < threshold) return true;
        return false;
    }

    /// <summary>
    /// Square with position in top left.
    /// True is closed: The question we ask is "is this node closed".
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public MarchingSquare GetSquareFor(Vector2Int position, float threshold)
    {
        if (position.x >= grid.GetLength(0) - 1 ||
            position.y >= grid.GetLength(1) - 1)
        {
            throw new Exception("No square exists for position.");
        }

        bool[] square = new bool[4];
        square[0] = grid[position.x, position.y + 1].Value > threshold;
        square[1] = grid[position.x + 1, position.y + 1].Value > threshold;
        square[2] = grid[position.x + 1, position.y].Value > threshold;
        square[3] = grid[position.x, position.y].Value > threshold;

        return new MarchingSquare(square);
    }

    public float GetSquareAverageValue(Vector2Int position)
    {
        if (position.x >= grid.GetLength(0) - 1 ||
            position.y >= grid.GetLength(1) - 1)
        {
            throw new Exception("No square exists for position.");
        }

        float total = 0;
        total += grid[position.x, position.y + 1].Value;
        total += grid[position.x + 1, position.y + 1].Value;
        total += grid[position.x + 1, position.y].Value;
        total += grid[position.x, position.y].Value;

        return total / 4;
    }

    public MarchingPoint GetClosestPoint(Vector2 position, float threshold = 0)
    {
        float minDist = float.MaxValue;
        MarchingPoint closestPoint = new MarchingPoint(); 
        foreach (var point in grid)
        {
            if (point.Value < threshold) continue;
            var dist = Vector2.Distance(position, point.gridPosition);
            if (dist < minDist)
            {
                minDist = dist;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    public MarchingPoint[] GetPointsInRadius(Vector2 position, float radius, float threshold = 0)
    {
        float minDist = float.MaxValue;
        List<MarchingPoint> radiusPoints = new List<MarchingPoint>(); 
        foreach (var point in grid)
        {
            if (point.Value < threshold) continue;
            var dist = Vector2.Distance(position, point.gridPosition);
            if (dist < radius)
            {
                radiusPoints.Add(point);
            }
        }
        return radiusPoints.ToArray();
    }
}
