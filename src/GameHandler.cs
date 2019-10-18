using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SwinGameSDK;

namespace MyGame.src {
    class GameHandler {
        private EnumState _gameState;
        private bool multiplayer;
        private Mode _menu, _game;
        private Color _p1Colour, _p2Colour;
        
        private int[] _highScores= new int[] { 0, 0, 0, 0, 0, 0 };
        /* High Score Indexes:
         * 0 = 1p Survival
         * 1 = 2p Survival - p1
         * 2 = 2p survival - p2
         * 3 = 1p timed
         * 4 = 2p timed - p1
         * 5 = 2p timed - p2
         */

        public GameHandler(EnumState gameState) {
            _gameState = gameState;
            _menu = new Menu(this);
            _game = new Game(this);
            LoadHighScores();
        }

        public void LoadHighScores() {
            try{
                StreamReader reader = new StreamReader(string.Format(AppDomain.CurrentDomain.BaseDirectory + "AsteroidsData.txt"));
                try {
                    for (int i = 0; i < _highScores.Count(); i++) {
                        _highScores[i] = Convert.ToInt32(reader.ReadLine());
                    }
                }
                finally {
                    reader.Close();
                }
            }
            catch (Exception e) {
                Console.Error.WriteLine("Error loading file: {0}", e.Message);
                _highScores = new int[] { 0, 0, 0, 0, 0, 0 };
            }
        }

        public void SaveHighScores() {
            StreamWriter writer = new StreamWriter(string.Format(AppDomain.CurrentDomain.BaseDirectory + "AsteroidsData.txt"));
            try {
                foreach( int i in _highScores) {
                    writer.WriteLine(i);
                }
            }
            catch (Exception e) {
                Console.Error.WriteLine("Error Saving file: {0}", e.Message);
            }
            finally {
                writer.Close();
            }
        }

        public void HandleInput() {
            SwinGame.ProcessEvents();
            if (GameState == EnumState.GameMenu) {
                _menu.HandleInput();
            }
            else {
                _game.HandleInput();
            }
        }
        public void UpdateState() {
            if (GameState == EnumState.GameMenu) {
                _menu.UpdateState();
            }
            else {
                _game.UpdateState();
            }
        }

        public void DrawGame() {
            SwinGame.ClearScreen(Color.Black);

            if (GameState == EnumState.GameMenu) {
                _menu.DrawAll();
            }
            else {
                _game.DrawAll();
            }

            SwinGame.RefreshScreen(60);
        }

        public EnumState GameState { get => _gameState; set => _gameState = value; }
        public Color P1Colour { get => _p1Colour; set => _p1Colour = value; }
        public Color P2Colour { get => _p2Colour; set => _p2Colour = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public int[] HighScores { get => _highScores; set => _highScores = value; }
    }
}
