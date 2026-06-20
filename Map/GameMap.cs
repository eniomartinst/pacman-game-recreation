using pacman.Enums;
using pacman.Models;
using pacman.Game; // Para acessar GameSettings.TileSize

namespace pacman.Map;

public class GameMap
{
    public int Width => Tiles.GetLength(1);
    public int Height => Tiles.GetLength(0);
    public Tile[,] Tiles { get; }

    // Propriedades para guardar onde as entidades nascem (em PIXELS)
    public double PacmanStartX { get; private set; }
    public double PacmanStartY { get; private set; }
    public double GhostStartX { get; private set; }
    public double GhostStartY { get; private set; }

    public GameMap(Tile[,] tiles, int pRow, int pCol, int gRow, int gCol)
    {
        Tiles = tiles;
        
        // Converte índice da matriz (linha/coluna) para Pixels (X/Y)
        // Coluna * 20 = X
        // Linha * 20 = Y
        PacmanStartX = pCol * GameSettings.TileSize;
        PacmanStartY = pRow * GameSettings.TileSize;
        
        GhostStartX = gCol * GameSettings.TileSize;
        GhostStartY = gRow * GameSettings.TileSize;
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return false;

        return Tiles[y, x].IsWalkable;
    }
}