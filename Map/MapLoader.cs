using pacman.Enums;
using pacman.Models;

namespace pacman.Map;

public static class MapLoader
{

    private const char SymbolWall        = '#'; // Parede
    private const char SymbolPellet      = '.'; // Ponto Pequeno
    private const char SymbolPower       = 'o'; // Poder (Ponto Grande)
    private const char SymbolFruit       = 'F'; // Fruta de vida
    private const char SymbolPacman      = 'P'; // Posição Inicial do Pacman
    private const char SymbolGhost       = 'G'; // Posição Inicial dos Fantasmas
    private const char SymbolEmpty       = ' '; // Espaço Vazio

public static GameMap LoadDefaultMap()
    {
        string[] map =
        {
            "###########################",
            "#............#............#",
            "#o###.######.#.######.###o#",
            "#............F............#",
            "#.###.#.###########.#.###.#",
            "#...#.................#...#",
            "###.#.#.###     ###.#.#.###",
            "##....#.###  G  ###.#....##",
            "##.##.#.###########.#.##.##",
            "##.##.#.............#.##.##",
            "##.##.#.##.#####.##.#.##.##",
            "#............#............#",
            "#.###.######.#.######.###.#",
            "#o..#.................#..o#",
            "###.#.#.###########.#.#.###",
            "#.....#......#......#.....#",
            "#.##########.#.##########.#",
            "#............P............#",
            "###########################"
        };

        return ParseMap(map);
    }

    private static GameMap ParseMap(string[] map)
    {
        int height = map.Length;
        int width = map[0].Length;
        var tiles = new Tile[height, width];
        
        // Coordenadas padrão (caso esqueça de colocar P ou G no mapa)
        int pRow = 1, pCol = 1; 
        int gRow = 1, gCol = 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                char c = map[y][x];

                // Verificação baseada nas Constantes da Legenda
                if (c == SymbolPacman)
                {
                    pRow = y; pCol = x;
                    tiles[y, x] = new Tile(TileType.Empty); // O chão vira vazio
                }
                else if (c == SymbolGhost)
                {
                    gRow = y; gCol = x;
                    tiles[y, x] = new Tile(TileType.Empty); // O chão vira vazio
                }
                else
                {
                    // Converte o caractere para o Tipo de Tile
                    tiles[y, x] = c switch
                    {
                        SymbolWall   => new Tile(TileType.Wall),
                        SymbolPellet => new Tile(TileType.Pellet),
                        SymbolPower  => new Tile(TileType.PowerPellet),
                        SymbolFruit  => new Tile(TileType.Fruit),
                        SymbolEmpty  => new Tile(TileType.Empty),
                        _            => new Tile(TileType.Empty) // Qualquer caractere desconhecido vira chão
                    };
                }
            }
        }

        // Retorna o mapa processado com as coordenadas encontradas
        return new GameMap(tiles, pRow, pCol, gRow, gCol);
    }
}