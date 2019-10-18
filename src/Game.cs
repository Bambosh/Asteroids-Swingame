using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    class Game: Mode
    {
        private const int GAME_LENGTH = 5400;
        private const int GAMEOVER_LENGTH = 200;
        private Color ASTEROID_COLOUR = Color.White;
        private Color UFOALIEN_COLOUR = Color.Green;

        private bool _setup = true;
        private int _gameTimer, _gameOverTimer;

        private List<Player> _players = new List<Player>();
        private HostileHandler _hostiles = new HostileHandler();
        private GameHandler _gameHandler;
        private Random _random = new Random();

        public Game(GameHandler gameHandler) {
            _gameHandler = gameHandler;
        }

        private void SetupGame() {
            //Always initialises player 1
            _players.Add(new Player(_random.Next(200, SwinGame.ScreenWidth() - 200),
                                    _random.Next(100, SwinGame.ScreenHeight() - 100),
                                    _random.Next(0, 359),
                                    _gameHandler.P1Colour));

            //Initialises player 2 in multiplayer scenarios
            if (_gameHandler.Multiplayer) {
                _players.Add(new Player(_random.Next(200, SwinGame.ScreenWidth() - 200),
                                        _random.Next(100, SwinGame.ScreenHeight() - 100),
                                        _random.Next(0, 359),
                                        _gameHandler.P2Colour));
            }

            //Enables timed ruleset if applicable
            if (_gameHandler.GameState == EnumState.Timed) {
                _gameTimer = GAME_LENGTH;
                foreach (Player p in _players) {
                    p.LivesEnabled = false;
                }
            }

            //Spans initial wave of hostiles
            _hostiles.NewAsteroid(20);
            _setup = false;
        }

        private void CleanUpGame() {
            //Saves high scores to memory
            if (_gameHandler.GameState == EnumState.Survival) {
                if (_gameHandler.HighScores[0] < _players[0].Score) {
                    _gameHandler.HighScores[0] = _players[0].Score;
                }
                if (_gameHandler.Multiplayer) {
                    if (_gameHandler.HighScores[1] < _players[0].Score) {
                        _gameHandler.HighScores[1] = _players[0].Score;
                    }
                    if (_gameHandler.HighScores[2] < _players[1].Score) {
                        _gameHandler.HighScores[2] = _players[1].Score;
                    }
                }
            }
            else if (_gameHandler.GameState == EnumState.Timed) {
                if (_gameHandler.HighScores[3] < _players[0].Score) {
                    _gameHandler.HighScores[3] = _players[0].Score;
                }
                if (_gameHandler.Multiplayer) {
                    if (_gameHandler.HighScores[4] < _players[0].Score) {
                        _gameHandler.HighScores[4] = _players[0].Score;
                    }
                    if (_gameHandler.HighScores[5] < _players[1].Score) {
                        _gameHandler.HighScores[5] = _players[1].Score;
                    }
                }
            }
            _gameHandler.SaveHighScores();

            _players.Clear();
            _hostiles.ClearAllHostiles();
            _gameTimer = 0;
            _gameHandler.GameState = EnumState.GameMenu;
            _setup = true;
        }

        public void HandleInput(){
            //Player 1 controls are defined here
            _players[0].HandleInput(KeyCode.AKey, KeyCode.DKey, KeyCode.WKey, KeyCode.SpaceKey);

            //Player 2 controls are defined here
            if (_gameHandler.Multiplayer) {
                _players[1].HandleInput(KeyCode.LeftKey, KeyCode.RightKey, KeyCode.UpKey, KeyCode.RightShiftKey);
            }

            //Debug command to spawn alien
            if (SwinGame.KeyReleased(KeyCode.PKey)) {
                _hostiles.NewAlien();
            }

            //Debug command to end game early
            if (SwinGame.KeyDown(KeyCode.BackspaceKey) && (_gameOverTimer == 0)) {
                _gameOverTimer = GAMEOVER_LENGTH;
            }
        }

        public void UpdateState(){
            if (_setup) {
                //Only called once, at the start of a game
                SetupGame();
            }

            //Enforces custom rules for timed mode
            if (_gameHandler.GameState == EnumState.Timed) {
                if (_gameTimer > 0) {
                    _gameTimer--;
                    if (_gameTimer <= 0) {
                        _gameOverTimer = GAMEOVER_LENGTH;
                    }
                }
            }

            //Updates each player & checks whether to end the game
            foreach (Player p in _players) {
                p.Update();
                if (p.Lives < 0 && (_gameOverTimer <= 0)) {
                    _gameOverTimer = GAMEOVER_LENGTH;
                }
            }

            //Updates all hostiles
            _hostiles.Update();
            
            if (_gameOverTimer <= 0) {
                CheckCollisions();
            }
            else {
                _gameOverTimer--;
                if (_gameOverTimer <= 0) {
                    CleanUpGame();
                }
            }
        }

        public void CheckCollisions(){
            //First, checks all hostiles against the first player
            _hostiles.CollidesWithHostile(_players[0]);
            //Then checks all hostiles against the second player, if it exists
            if (_gameHandler.Multiplayer) {
                _hostiles.CollidesWithHostile(_players[1]);            
            }
            //If in battle mode, checks player collisions against the other's missiles
            if (_gameHandler.GameState == EnumState.Battle) {
                _players[0].Missiles.PlayerCollidesWithMissile(_players[1]);
                _players[1].Missiles.PlayerCollidesWithMissile(_players[0]);
            }
        }

        //Handles all text on game over screen
        private void DrawGameOver() {   
            bool p1wins = false, p2wins = false;

            SwinGame.DrawText("GAME OVER", Color.Red, SwinGame.FontNamed("titleFont"), 300, 100);

            if ((_gameHandler.GameState == EnumState.Timed) || (_gameHandler.GameState == EnumState.Survival)) {
                SwinGame.DrawText(string.Format("P1 Score: " + _players[0].Score), _players[0].Color, SwinGame.FontNamed("bigFont"), 390, 220);
                if (_gameHandler.Multiplayer) {
                    SwinGame.DrawText(string.Format("P2 Score: " + _players[1].Score), _players[1].Color, SwinGame.FontNamed("bigFont"), 390, 280);
                    if (_players[0].Score >= _players[1].Score)
                        p1wins = true;
                    if (_players[0].Score <= _players[1].Score) {
                        p2wins = true;
                    }
                }
            }
            else if (_gameHandler.GameState == EnumState.Battle) {
                if (_players[0].Lives >= _players[1].Lives)
                    p1wins = true;
                if (_players[0].Lives <= _players[1].Lives) {
                    p2wins = true;
                }
            }
            if (_gameHandler.Multiplayer) {
                if (p1wins && !p2wins)
                    SwinGame.DrawText("PLAYER 1 WINS", _players[0].Color, SwinGame.FontNamed("bigFont"), 390, 350);
                else if (!p1wins && p2wins) {
                    SwinGame.DrawText("PLAYER 2 WINS", _players[1].Color, SwinGame.FontNamed("bigFont"), 390, 350);
                }
                else {
                    SwinGame.DrawText("EVERYBODY WINS", Color.White, SwinGame.FontNamed("bigFont"), 380, 350);
                }
            }
        }

        //Draws and handles the score-board and timer that displays during the game
        private void DrawScoreBoard() {
            if ((_gameHandler.GameState == EnumState.Timed) || (_gameHandler.GameState == EnumState.Survival)) {
                SwinGame.DrawText(string.Format("Score: " + _players[0].Score), _players[0].Color, SwinGame.FontNamed("smallFont"), 10, 10);
                if (_gameHandler.Multiplayer) {
                    SwinGame.DrawText(string.Format("Score: " + _players[1].Score), _players[1].Color, SwinGame.FontNamed("smallFont"), 10, 50);
                }
                if (_gameHandler.GameState == EnumState.Timed) {
                    int s = _gameTimer / 60;
                    int ms = (((_gameTimer % 60) * 5) / 30);
                    SwinGame.DrawText(string.Format("Time: " + s + "." + ms), Color.LightGrey, SwinGame.FontNamed("smallFont"), 10, SwinGame.ScreenHeight()-50);
                }
            }
        }

        public void DrawAll() {
            _hostiles.DrawAll();

            _players[0].Draw();
            if (_gameHandler.Multiplayer) { 
                 _players[1].Draw();
            }

            DrawScoreBoard();

            if (_gameOverTimer > 0) {
                DrawGameOver();
            }
        }
    }
}
