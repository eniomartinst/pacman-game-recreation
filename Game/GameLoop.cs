using Microsoft.UI.Dispatching;

namespace pacman.Game;

public class GameLoop
{
    private readonly DispatcherTimer _timer;
    private readonly GameManager _game;

    public GameLoop(GameManager game)
    {
        _game = game;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };

        _timer.Tick += (_, _) => _game.Update();
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
}
