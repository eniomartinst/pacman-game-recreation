namespace pacman.Models;

public class Pellet
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Points { get; set; }

    public Pellet(int x, int y, int points)
    {
        X = x;
        Y = y;
        Points = points;
    }
}
