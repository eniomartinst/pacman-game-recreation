using pacman.Enums;

namespace pacman.Models;

public class Tile
{
    public TileType Type { get; set; }
    public bool IsWalkable => Type != TileType.Wall;

    public Tile(TileType type)
    {
        Type = type;
    }
}
