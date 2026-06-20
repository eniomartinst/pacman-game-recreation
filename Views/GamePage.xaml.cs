using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging; 
using Microsoft.UI.Xaml.Shapes;
using pacman.Enums;
using pacman.Models;
using pacman.Map;
using pacman.Game;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Threading.Tasks; 
using System.Linq; 
using Windows.System;

namespace pacman.Views;

public sealed partial class GamePage : Page
{
    // ==========================================
    // 1. VARIÁVEIS DE ESTADO E MOTOR DO JOGO
    // ==========================================
    private readonly DispatcherTimer _timer; 
    private DispatcherTimer _powerTimer;
    private readonly Player _player; 
    private GameMap _gameMap; 
    private readonly List<Ghost> _ghosts = new(); 
    
    // --- Lógica de Jogo e Animação ---
    private int _powerTicksLeft = 0;  
    private int _animTick = 0;  
    private bool _isMouthOpen = false;  
    
    // --- Lógica de Controles ---
    private bool _isMuted = false;
    private bool _isPaused = false;
    
    // --- Pontuação e Vidas ---
    private readonly string _scoreFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscores.txt");
    private int _score = 0;
    private int _lives = 3;
    private int _totalPellets = 0;
    private bool _isGameRunning = false;
    
    // --- Controle de Concorrência de Áudio ---
    private DateTime _lastChompTime = DateTime.MinValue;
    private DateTime _lastGhostSoundTime = DateTime.MinValue;

    private readonly BitmapImage _imgPellet = new(new Uri("ms-appx:///Assets/Images/pellet.png"));
    private readonly BitmapImage _imgPower = new(new Uri("ms-appx:///Assets/Images/power_pellet.png"));
    private readonly BitmapImage _imgFruit = new(new Uri("ms-appx:///Assets/Images/grappe_fruit.png"));
    
    private readonly BitmapImage _imgPacRight = new(new Uri("ms-appx:///Assets/Images/pacman_right.png"));
    private readonly BitmapImage _imgPacLeft = new(new Uri("ms-appx:///Assets/Images/pacman_left.png"));
    private readonly BitmapImage _imgPacUp = new(new Uri("ms-appx:///Assets/Images/pacman_up.png"));
    private readonly BitmapImage _imgPacDown = new(new Uri("ms-appx:///Assets/Images/pacman_down.png"));
    private readonly BitmapImage _imgPacMouthRight = new(new Uri("ms-appx:///Assets/Images/pacman_mouth_right.png"));
    private readonly BitmapImage _imgPacMouthLeft = new(new Uri("ms-appx:///Assets/Images/pacman_mouth_left.png"));
    private readonly BitmapImage _imgPacMouthUp = new(new Uri("ms-appx:///Assets/Images/pacman_mouth_up.png"));
    private readonly BitmapImage _imgPacMouthDown = new(new Uri("ms-appx:///Assets/Images/pacman_mouth_down.png"));

    private readonly BitmapImage _imgGhostFear = new(new Uri("ms-appx:///Assets/Images/ghost_fear.png"));
    private readonly BitmapImage _imgGhostStartFear = new(new Uri("ms-appx:///Assets/Images/ghost_start_fear.png"));
    
    private readonly BitmapImage _imgRedUp = new(new Uri("ms-appx:///Assets/Images/ghost_red_up.png"));
    private readonly BitmapImage _imgRedDown = new(new Uri("ms-appx:///Assets/Images/ghost_red_down.png"));
    private readonly BitmapImage _imgRedLeft = new(new Uri("ms-appx:///Assets/Images/ghost_red_left.png"));
    private readonly BitmapImage _imgRedRight = new(new Uri("ms-appx:///Assets/Images/ghost_red_right.png"));

    private readonly BitmapImage _imgPinkUp = new(new Uri("ms-appx:///Assets/Images/ghost_pink_up.png"));
    private readonly BitmapImage _imgPinkDown = new(new Uri("ms-appx:///Assets/Images/ghost_pink_down.png"));
    private readonly BitmapImage _imgPinkLeft = new(new Uri("ms-appx:///Assets/Images/ghost_pink_left.png"));
    private readonly BitmapImage _imgPinkRight = new(new Uri("ms-appx:///Assets/Images/ghost_pink_right.png"));

    private readonly BitmapImage _imgBlueUp = new(new Uri("ms-appx:///Assets/Images/ghost_blue_up.png"));
    private readonly BitmapImage _imgBlueDown = new(new Uri("ms-appx:///Assets/Images/ghost_blue_down.png"));
    private readonly BitmapImage _imgBlueLeft = new(new Uri("ms-appx:///Assets/Images/ghost_blue_left.png"));
    private readonly BitmapImage _imgBlueRight = new(new Uri("ms-appx:///Assets/Images/ghost_blue_right.png"));

    private readonly BitmapImage _imgOrangeUp = new(new Uri("ms-appx:///Assets/Images/ghost_orange_up.png"));
    private readonly BitmapImage _imgOrangeDown = new(new Uri("ms-appx:///Assets/Images/ghost_orange_down.png"));
    private readonly BitmapImage _imgOrangeLeft = new(new Uri("ms-appx:///Assets/Images/ghost_orange_left.png"));
    private readonly BitmapImage _imgOrangeRight = new(new Uri("ms-appx:///Assets/Images/ghost_orange_right.png"));


    /// <summary>
    /// Construtor da página. Inicializa os componentes visuais, carrega o mapa e configura os Timers.
    /// </summary>
    public GamePage()
    {
        InitializeComponent();
        
        _gameMap = MapLoader.LoadDefaultMap();
        GameCanvas.Width = _gameMap.Width * GameSettings.TileSize;
        GameCanvas.Height = _gameMap.Height * GameSettings.TileSize;

        _player = new Player { X = _gameMap.PacmanStartX, Y = _gameMap.PacmanStartY }; 
        InitializeGhosts();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += GameLoop;

        _powerTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _powerTimer.Tick += PowerTimer_Tick;

        Loaded += (s, e) => 
        {
            Draw();
            RootGrid.Focus(FocusState.Programmatic);
            PlaySoundNative("pacman_intermission.wav");
        };

        this.PointerPressed += (s, e) =>
        {
            RootGrid.Focus(FocusState.Pointer);
        };
    }

    private void PowerTimer_Tick(object? sender, object e)
    {
        _powerTicksLeft--;
        if (_powerTicksLeft <= 0)
        {
            _powerTimer.Stop();
            foreach (var ghost in _ghosts) ghost.IsVulnerable = false;
        }
    }

    private void InitializeGhosts()
    {
        _ghosts.Clear();
        double gx = _gameMap.GhostStartX;
        double gy = _gameMap.GhostStartY;
        
        // Cria 4 fantasmas em posições levemente diferentes na Ghost House
        _ghosts.Add(new Ghost(gx, gy, 0));       
        _ghosts.Add(new Ghost(gx - 20, gy, 1));  
        _ghosts.Add(new Ghost(gx + 20, gy, 2));  
        _ghosts.Add(new Ghost(gx, gy - 20, 3));  
    }

#pragma warning disable CA1416 
    /// <summary>
    /// Gerenciador de áudio nativo. Ignora o toque caso o jogo esteja mutado.
    /// </summary>
    private void PlaySoundNative(string fileName)
    {
        if (_isMuted) return;

        try
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = System.IO.Path.Combine(basePath, "Assets", "Sounds", fileName);
            if (File.Exists(fullPath))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(fullPath);
                player.Play();
            }
        }
        catch { /* Ignora erros de driver de áudio para não travar o jogo */ }
    }
#pragma warning restore CA1416

    private void StartGame_Click(object sender, RoutedEventArgs e)
    {
        MenuGrid.Visibility = Visibility.Collapsed;
        HighScoreGrid.Visibility = Visibility.Collapsed;
        ResetGameData();
        PlaySoundNative("pacman_beginning.wav");
        _isGameRunning = true;
        _isPaused = false;
        _timer.Start();
        RootGrid.Focus(FocusState.Programmatic);
    }

    private void ShowHighScores_Click(object sender, RoutedEventArgs e)
    {
        MenuGrid.Visibility = Visibility.Collapsed;
        HighScoreGrid.Visibility = Visibility.Visible;
        LoadHighScores();
    }

    private void BackToMenu_Click(object sender, RoutedEventArgs e)
    {
        HighScoreGrid.Visibility = Visibility.Collapsed;
        MenuGrid.Visibility = Visibility.Visible;
        PlaySoundNative("pacman_intermission.wav");
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Exit();

    /// <summary>
    /// MOTOR PRINCIPAL (GAME LOOP) E COLISÕES
    /// </summary>
    private void GameLoop(object? sender, object e)
    {
        if (!_isGameRunning || _isPaused) return; 

        _animTick++;
        if (_animTick % 10 == 0) _isMouthOpen = !_isMouthOpen;

        _player.Update(_gameMap);

        foreach (var ghost in _ghosts)
        {
            ghost.Update(_gameMap);

            if (Math.Abs(ghost.X - _player.X) < 15 && Math.Abs(ghost.Y - _player.Y) < 15)
            {
                if (ghost.IsVulnerable)
                {
                    _score += 200;
                    ScoreText.Text = $"Score: {_score}";
                    ghost.SendHome((int)_gameMap.GhostStartX, (int)_gameMap.GhostStartY); // Respawn
                    
                    PlaySoundNative("pacman_eatghost.wav"); 
                    _lastChompTime = DateTime.Now; 
                }
                else
                {
                    HandleDeath(); // Pac-Man tocou num fantasma perigoso
                    return;
                }
            }
        }
        
        CheckCollisionsWithItems();

        // Controle de Tensão: Toca o som dos fantasmas caso o Pac-Man não esteja comendo
        if ((DateTime.Now - _lastChompTime).TotalMilliseconds > 500 && (DateTime.Now - _lastGhostSoundTime).TotalMilliseconds > 800)
        {
            PlaySoundNative("ghost_sound.wav");
            _lastGhostSoundTime = DateTime.Now;
        }

        Draw(); 
    }

    private void HandleDeath()
    {
        PlaySoundNative("pacman_death.wav");
        _lives--;
        LivesText.Text = $"Vidas: {_lives}";

        if (_lives > 0)
        {
            // Reseta posições se ainda houver vidas
            _player.X = _gameMap.PacmanStartX;
            _player.Y = _gameMap.PacmanStartY;
            _player.CurrentDirection = Direction.None;
            InitializeGhosts();
            Draw();
        }
        else GameOver();
    }

    private async void GameOver()
    {
        _isGameRunning = false;
        _timer.Stop();
        SaveScore();

        var dialog = new ContentDialog
        {
            Title = "GAME OVER",
            Content = $"Pontuação Final: {_score}",
            PrimaryButtonText = "Reiniciar",
            SecondaryButtonText = "Menu Principal",
            CloseButtonText = "Sair",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        
        if (result == ContentDialogResult.Primary)
        {
            ResetGameData();
            PlaySoundNative("pacman_beginning.wav");
            _isGameRunning = true;
            _isPaused = false;
            _timer.Start();
        }
        else if (result == ContentDialogResult.Secondary)
        {
            MenuGrid.Visibility = Visibility.Visible;
            PlaySoundNative("pacman_intermission.wav");
        }
        else Application.Current.Exit();
    }

    private void ResetGameData()
    {
        _score = 0;
        _lives = 3;
        ScoreText.Text = "Score: 0";
        LivesText.Text = "Vidas: 3";
        _gameMap = MapLoader.LoadDefaultMap();
        CountPellets();
        _powerTimer.Stop();
        _powerTicksLeft = 0;
        InitializeGhosts();
    }


    private void SaveScore()
    {
        if (_score == 0) return; 
        List<int> scores = new List<int>();
        
        // Lê as pontuações existentes
        if (File.Exists(_scoreFilePath))
        {
            var lines = File.ReadAllLines(_scoreFilePath);
            foreach (var line in lines) if (int.TryParse(line, out int s)) scores.Add(s);
        }
        
        // Adiciona a nova, ordena via LINQ e guarda apenas o Top 5
        scores.Add(_score);
        scores = scores.OrderByDescending(s => s).Take(5).ToList();
        File.WriteAllLines(_scoreFilePath, scores.Select(s => s.ToString()));
    }

    private void LoadHighScores()
    {
        if (File.Exists(_scoreFilePath))
        {
            var lines = File.ReadAllLines(_scoreFilePath);
            var textoFormatado = "";
            for (int i = 0; i < lines.Length; i++) textoFormatado += $"{i + 1}. JOGADOR - {lines[i]} PTS\n";
            HighScoresListText.Text = textoFormatado;
        }
        else HighScoresListText.Text = "Ainda não há pontuações.\nSeja o primeiro a jogar!";
    }

    private void CountPellets()
    {
        _totalPellets = 0;
        for (int y = 0; y < _gameMap.Height; y++)
        {
            for (int x = 0; x < _gameMap.Width; x++)
            {
                if (_gameMap.Tiles[y, x].Type == TileType.Pellet || _gameMap.Tiles[y, x].Type == TileType.PowerPellet)
                    _totalPellets++;
            }
        }
    }

    /// <summary>
    /// Interação do jogador com OS itens do mapa
    /// </summary>
    private void CheckCollisionsWithItems()
    {
        int tileSize = GameSettings.TileSize;
        
        // Converte as coordenadas contínuas de Pixel para Índices Inteiros da Matriz
        int gridX = (int)((_player.X + tileSize / 2) / tileSize);
        int gridY = (int)((_player.Y + tileSize / 2) / tileSize);

        if (gridY < 0 || gridY >= _gameMap.Height || gridX < 0 || gridX >= _gameMap.Width) return;
        var tile = _gameMap.Tiles[gridY, gridX];

        if (tile.Type == TileType.Pellet || tile.Type == TileType.PowerPellet || tile.Type == TileType.Fruit)
        {
            // Cooldown de áudio para bolinhas normais (Waka Waka)
            if ((DateTime.Now - _lastChompTime).TotalMilliseconds > 250 && tile.Type != TileType.PowerPellet)
            {
                PlaySoundNative("pacman_chomp.wav");
                _lastChompTime = DateTime.Now;
            }

            // Fruta: +100 Pontos e +1 Vida (Máx 3)
            if (tile.Type == TileType.Fruit) 
            {
                _score += 100;
                if (_lives < 3) 
                {
                    _lives++;
                    LivesText.Text = $"Vidas: {_lives}";
                }
            }
            else 
            {
                _score += (tile.Type == TileType.Pellet) ? 10 : 50;
            }
            
            ScoreText.Text = $"Score: {_score}";
            
            if (tile.Type != TileType.Fruit) _totalPellets--;

            // Modo Invencível ativado ao comer a Power Pellet
            if (tile.Type == TileType.PowerPellet)
            {
                _powerTicksLeft = 7; 
                _powerTimer.Start();
                foreach (var ghost in _ghosts) ghost.IsVulnerable = true;
                
                // --- NOVO: Toca o som especial ao comer a pílula de poder ---
                PlaySoundNative("pacman_eatfruit.wav"); 
                _lastChompTime = DateTime.Now; // Atualiza o relógio para o som não ser cortado
            }

            tile.Type = TileType.Empty; // Limpa o bloco da matriz

            // Verifica Condição de Vitória
            if (_totalPellets <= 0) GameWin();
        }
    }

    private async void GameWin()
    {
        _isGameRunning = false;
        _timer.Stop();
        _powerTimer.Stop();
        SaveScore();

        var dialog = new ContentDialog
        {
            Title = "🏆 VITÓRIA! 🏆",
            Content = $"Você limpou o labirinto!\nPontuação Final: {_score}",
            PrimaryButtonText = "Menu Principal",
            CloseButtonText = "Sair",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            MenuGrid.Visibility = Visibility.Visible;
            PlaySoundNative("pacman_intermission.wav");
        }
        else Application.Current.Exit();
    }
    
    private void OnKeyDown(object s, KeyRoutedEventArgs e)
    {
        // Mutar o jogo (tecla M)
        if (e.Key == VirtualKey.M)
        {
            _isMuted = !_isMuted;
            if (_isMuted) new System.Media.SoundPlayer().Stop(); // Corta o som imediatamente
            return;
        }

        // Pausar o jogo (tecla P)
        if (e.Key == VirtualKey.P)
        {
            if (!_isGameRunning) return;
            
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                _timer.Stop();
                _powerTimer.Stop();
            }
            else
            {
                _timer.Start();
                if (_powerTicksLeft > 0) _powerTimer.Start();
            }
            return;
        }

        if (!_isGameRunning || _isPaused) return;

        // Atualiza a "Intenção de Movimento" no Player
        switch (e.Key) {
            case VirtualKey.Up: _player.CurrentDirection = Direction.Up; break;
            case VirtualKey.Down: _player.CurrentDirection = Direction.Down; break;
            case VirtualKey.Left: _player.CurrentDirection = Direction.Left; break;
            case VirtualKey.Right: _player.CurrentDirection = Direction.Right; break;
        }
    }

    /// <summary>
    /// Seleciona o sprite do Pac-Man combinando a direção atual com o tick de animação da boca.
    /// </summary>
    private BitmapImage GetPacmanImage()
    {
        return _player.CurrentDirection switch
        {
            Direction.Up => _isMouthOpen ? _imgPacMouthUp : _imgPacUp,
            Direction.Down => _isMouthOpen ? _imgPacMouthDown : _imgPacDown,
            Direction.Left => _isMouthOpen ? _imgPacMouthLeft : _imgPacLeft,
            Direction.Right => _isMouthOpen ? _imgPacMouthRight : _imgPacRight,
            _ => _isMouthOpen ? _imgPacMouthRight : _imgPacRight
        };
    }

    /// <summary>
    /// Seleciona o sprite dos Fantasmas, gerenciando as cores base e os efeitos de medo (piscar).
    /// </summary>
    private BitmapImage GetGhostImage(Ghost g)
    {
        if (g.IsVulnerable)
        {
            // Pisca de branco se faltar 2 segundos ou menos
            if (_powerTicksLeft <= 2 && _isMouthOpen) return _imgGhostStartFear;
            return _imgGhostFear;
        }

        return g.GhostType switch
        {
            0 => g.CurrentDirection switch { Direction.Up => _imgRedUp, Direction.Down => _imgRedDown, Direction.Left => _imgRedLeft, _ => _imgRedRight },
            1 => g.CurrentDirection switch { Direction.Up => _imgPinkUp, Direction.Down => _imgPinkDown, Direction.Left => _imgPinkLeft, _ => _imgPinkRight },
            2 => g.CurrentDirection switch { Direction.Up => _imgBlueUp, Direction.Down => _imgBlueDown, Direction.Left => _imgBlueLeft, _ => _imgBlueRight },
            3 => g.CurrentDirection switch { Direction.Up => _imgOrangeUp, Direction.Down => _imgOrangeDown, Direction.Left => _imgOrangeLeft, _ => _imgOrangeRight },
            _ => _imgRedRight
        };
    }

    /// <summary>
    /// Limpa a tela e redesenha todos os elementos com base na matriz atualizada e nos objetos na memória.
    /// </summary>
    private void Draw()
    {
        GameCanvas.Children.Clear();
        int tileSize = GameSettings.TileSize;
        
        // Renderiza o Cenário (Chão, Paredes e Itens)
        for (int y = 0; y < _gameMap.Height; y++) {
            for (int x = 0; x < _gameMap.Width; x++) {
                var tile = _gameMap.Tiles[y, x];
                if (tile.Type == TileType.Wall) {
                    var r = new Rectangle { Width = tileSize, Height = tileSize, Fill = new SolidColorBrush(Microsoft.UI.Colors.Blue) };
                    Canvas.SetLeft(r, x * tileSize); Canvas.SetTop(r, y * tileSize); GameCanvas.Children.Add(r);
                } else if (tile.Type == TileType.Pellet) {
                    var i = new Image { Source = _imgPellet, Width = 6, Height = 6 };
                    Canvas.SetLeft(i, x * tileSize + 7); Canvas.SetTop(i, y * tileSize + 7); GameCanvas.Children.Add(i);
                } else if (tile.Type == TileType.PowerPellet) {
                    var i = new Image { Source = _imgPower, Width = 14, Height = 14 };
                    Canvas.SetLeft(i, x * tileSize + 3); Canvas.SetTop(i, y * tileSize + 3); GameCanvas.Children.Add(i);
                } else if (tile.Type == TileType.Fruit) {
                    var i = new Image { Source = _imgFruit, Width = tileSize, Height = tileSize };
                    Canvas.SetLeft(i, x * tileSize); Canvas.SetTop(i, y * tileSize); GameCanvas.Children.Add(i);
                }
            }
        }

        // Renderiza os Inimigos
        foreach (var g in _ghosts) {
            var gc = new Image { Source = GetGhostImage(g), Width = tileSize, Height = tileSize };
            Canvas.SetLeft(gc, g.X); Canvas.SetTop(gc, g.Y); GameCanvas.Children.Add(gc);
        }
        
        // Renderiza o Jogador
        var pi = new Image { Source = GetPacmanImage(), Width = tileSize, Height = tileSize };
        Canvas.SetLeft(pi, _player.X); Canvas.SetTop(pi, _player.Y); GameCanvas.Children.Add(pi);
    }
}