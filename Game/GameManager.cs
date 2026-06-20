using pacman.Models;

namespace pacman.Game;

public class GameManager
{
    // Essa classe não está sendo mais usada pela GamePage, 
    // mas mantemos ela aqui vazia para não dar erro de compilação.
    public Player Player { get; } = new();

    public void Update()
    {
        // COMENTAMOS ESTA LINHA POIS ELA CAUSA O ERRO:
        // Player.Update(); 
    }
}