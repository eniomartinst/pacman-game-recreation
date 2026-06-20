using pacman.Enums;
using pacman.Map;
using pacman.Game;
using System;

namespace pacman.Models;

public class Player
{
    public double X { get; set; }
    public double Y { get; set; }
    public Direction CurrentDirection { get; set; } = Direction.None;
    
    // Velocidade doo pacman
    public double Speed { get; set; } = 2.5; 
    
    public int RotationAngle { get; private set; } = 0;

    public void Update(GameMap map)
    {
        if (CurrentDirection == Direction.None) return;

        int tileSize = GameSettings.TileSize;
        double nextX = X;
        double nextY = Y;
        
        bool alignedX = Math.Abs(X % tileSize) < 0.1;
        bool alignedY = Math.Abs(Y % tileSize) < 0.1;
        bool canMove = false;

        switch (CurrentDirection)
        {
            case Direction.Up:
                if (alignedX)
                {
                    nextY -= Speed;
                    RotationAngle = -90;
                    canMove = map.IsWalkable((int)((X + 1) / tileSize), (int)(nextY / tileSize));
                }
                break;

            case Direction.Down:
                if (alignedX)
                {
                    nextY += Speed;
                    RotationAngle = 90;
                    canMove = map.IsWalkable((int)((X + 1) / tileSize), (int)((nextY + tileSize - 1) / tileSize));
                }
                break;

            case Direction.Left:
                if (alignedY)
                {
                    nextX -= Speed;
                    RotationAngle = 180;
                    canMove = map.IsWalkable((int)(nextX / tileSize), (int)((Y + 1) / tileSize));
                }
                break;

            case Direction.Right:
                if (alignedY)
                {
                    nextX += Speed;
                    RotationAngle = 0;
                    canMove = map.IsWalkable((int)((nextX + tileSize - 1) / tileSize), (int)((Y + 1) / tileSize));
                }
                break;
        }

        if (canMove)
        {
            X = nextX;
            Y = nextY;
        }
        else
        {
            // Auto-alinhamento visual se travar
            if (CurrentDirection == Direction.Up || CurrentDirection == Direction.Down)
                 X = Math.Round(X / tileSize) * tileSize;
            else if (CurrentDirection == Direction.Left || CurrentDirection == Direction.Right)
                 Y = Math.Round(Y / tileSize) * tileSize;
        }
    }
}