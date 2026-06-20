using pacman.Enums;
using pacman.Map;
using pacman.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pacman.Models;

public class Ghost
{
    public double X { get; set; }
    public double Y { get; set; }
    
    public bool IsVulnerable { get; set; } = false;
    public void SendHome(int startX, int startY)
    {
        X = startX;
        Y = startY;
        IsVulnerable = false; // pra reviver 
    }

    // veolcidade dos fantasms
    public double Speed { get; set; } = 2.5; 
    
    public Direction CurrentDirection { get; set; }
    public int GhostType { get; private set; } 

    private static readonly Random _rnd = new Random();

    public Ghost(double startX, double startY, int type)
    {
        X = startX;
        Y = startY;
        GhostType = type;
        CurrentDirection = _rnd.Next(0, 2) == 0 ? Direction.Left : Direction.Right;
    }

    public void Update(GameMap map)
    {
        int tileSize = GameSettings.TileSize;
        
        if (CanMove(CurrentDirection, map))
        {
            Move(CurrentDirection);

            // 20% de chance de virar num cruzamento
            if (IsAligned(tileSize) && _rnd.Next(0, 100) < 20)
            {
                 TryChangeDirection(map);
            }
        }
        else
        {
            SnapToGrid(tileSize);
            TryChangeDirection(map);
        }
    }

    private void Move(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:    Y -= Speed; break;
            case Direction.Down:  Y += Speed; break;
            case Direction.Left:  X -= Speed; break;
            case Direction.Right: X += Speed; break;
        }
    }

    private bool CanMove(Direction dir, GameMap map)
    {
        int tileSize = GameSettings.TileSize;
        bool alignedX = Math.Abs(X % tileSize) < 0.1;
        bool alignedY = Math.Abs(Y % tileSize) < 0.1;
        double nextX = X;
        double nextY = Y;

        switch (dir)
        {
            case Direction.Up:
                if (!alignedX) return false;
                nextY -= Speed;
                return map.IsWalkable((int)((X + 1) / tileSize), (int)(nextY / tileSize));

            case Direction.Down:
                if (!alignedX) return false;
                nextY += Speed;
                return map.IsWalkable((int)((X + 1) / tileSize), (int)((nextY + tileSize - 1) / tileSize));

            case Direction.Left:
                if (!alignedY) return false;
                nextX -= Speed;
                return map.IsWalkable((int)(nextX / tileSize), (int)((Y + 1) / tileSize));

            case Direction.Right:
                if (!alignedY) return false;
                nextX += Speed;
                return map.IsWalkable((int)((nextX + tileSize - 1) / tileSize), (int)((Y + 1) / tileSize));
        }
        return false;
    }

    private void TryChangeDirection(GameMap map)
    {
        var possibleDirections = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        Direction opposite = GetOpposite(CurrentDirection);
        if (possibleDirections.Contains(opposite)) possibleDirections.Remove(opposite);

        possibleDirections = possibleDirections.OrderBy(x => _rnd.Next()).ToList();

        foreach (var dir in possibleDirections)
        {
            if (CanMove(dir, map))
            {
                CurrentDirection = dir;
                return;
            }
        }
        
        if (CanMove(opposite, map)) CurrentDirection = opposite;
    }

    private Direction GetOpposite(Direction dir) => dir switch
    {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        _ => Direction.None
    };

    private bool IsAligned(int tileSize) => Math.Abs(X % tileSize) < 0.1 && Math.Abs(Y % tileSize) < 0.1;
    private void SnapToGrid(int tileSize) { X = Math.Round(X / tileSize) * tileSize; Y = Math.Round(Y / tileSize) * tileSize; }
}