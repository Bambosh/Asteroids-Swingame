using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src {
    class Menu: Mode {
        private GameHandler _gameHandler;
        private enum InputType { Click, Hover }
        private enum PlayerColours { Red, Orange, Yellow, Green, Blue, Indigo, Violet }
        private PlayerColours _p1Colour = PlayerColours.Orange, _p2Colour = PlayerColours.Blue;

        public Menu(GameHandler gameHandler) {
            _gameHandler = gameHandler;
        }

        //Converts the PlayerColours enum to actual colours you can initialize the players with
        private Color ColourConverter(PlayerColours c) {
            if (c == PlayerColours.Red) {
                return Color.Red;
            }
            else if (c == PlayerColours.Orange) {
                return Color.Orange;
            }
            else if (c == PlayerColours.Yellow) {
                return Color.Yellow;
            }
            else if (c == PlayerColours.Green) {
                return Color.Green;
            }
            else if (c == PlayerColours.Blue) {
                return Color.Blue;
            }
            else if (c == PlayerColours.Indigo) {
                return Color.Indigo;
            }
            else if (c == PlayerColours.Violet) {
                return Color.Violet;
            }
            else {
                return Color.White;
            }
        }

        private PlayerColours ColourIncrement(PlayerColours c1, PlayerColours c2) {
            c1++;
            if (c1 > PlayerColours.Violet) {
                c1 = PlayerColours.Red;
            }
            if (c1 == c2) {
                c1 = ColourIncrement(c1, c2);
            }
            return c1;
        }

        private PlayerColours ColourDecrement(PlayerColours c1, PlayerColours c2) {
            c1--;
            if (c1 < PlayerColours.Red) {
                c1 = PlayerColours.Violet;
            }
            if (c1 == c2) {
                c1 = ColourDecrement(c1, c2);
            }
            return c1;
        }

        //Checks is mouse is in the rectangle passed through the parameters
        private bool MouseRegion(int btnX, int btnY, int btnWidth, int btnHeight) {
            float cursorX = SwinGame.MouseX();
            float cursorY = SwinGame.MouseY();
            float clickRegionX = btnX + btnWidth;
            float clickRegionY = btnY + btnHeight;
            
            if ((cursorX >= btnX) && (cursorX <= clickRegionX) && (cursorY >= btnY) && (cursorY <= clickRegionY)) {
                return true;
            }
            return false;
        }

        public void HandleInput() {
            if (SwinGame.MouseClicked(MouseButton.LeftButton)) {
                //1p Survival
                if (MouseRegion(30, 180, 350, 60)) {
                    _gameHandler.GameState = EnumState.Survival;
                    _gameHandler.Multiplayer = false;
                }

                //1p Score Attack
                if (MouseRegion(30, 270, 350, 60)) {
                    _gameHandler.GameState = EnumState.Timed;
                    _gameHandler.Multiplayer = false;
                }
                //2p Survival
                if (MouseRegion(30, 460, 350, 60)) {
                    _gameHandler.GameState = EnumState.Survival;
                    _gameHandler.Multiplayer = true;
                }
                //2p Score Attack
                if (MouseRegion(30, 540, 350, 60)) {
                    _gameHandler.GameState = EnumState.Timed;
                    _gameHandler.Multiplayer = true;
                }
                //2p Battle
                if (MouseRegion(30, 630, 350, 60)) {
                    _gameHandler.GameState = EnumState.Battle;
                    _gameHandler.Multiplayer = true;
                }

                //P1 Colour Left
                if (MouseRegion(660, 620, 135, 60)) {
                    _p1Colour = ColourDecrement(_p1Colour, _p2Colour);
                }
                //P1 Colour Right
                if (MouseRegion(805, 620, 135, 60)) {
                    _p1Colour = ColourIncrement(_p1Colour, _p2Colour);
                }
                //P2 Colour Left
                if (MouseRegion(960, 620, 135, 60)) {
                    _p2Colour = ColourDecrement(_p2Colour, _p1Colour);
                }
                //P2 Colour Right
                if (MouseRegion(1105, 620, 135, 60)) {
                    _p2Colour = ColourIncrement(_p2Colour, _p1Colour);
                }
            }
        }

        public void UpdateState() {
            _gameHandler.P1Colour = ColourConverter(_p1Colour);
            _gameHandler.P2Colour = ColourConverter(_p2Colour);
        }

        public void DrawAll() {
            SwinGame.DrawText("ASTEROIDS", Color.Grey, SwinGame.FontNamed("titleFont"), 600, 20);
            SwinGame.DrawText("One Player", Color.White, SwinGame.FontNamed("bigFont"), 30, 90);
            SwinGame.DrawText("Two Players", Color.White, SwinGame.FontNamed("bigFont"), 30, 360);

            //1p Survival
            if (MouseRegion(30, 180, 350, 60)) {
                SwinGame.FillRectangle(Color.White, 30, 180, 350, 60);
                SwinGame.DrawText("Survival", Color.Black, SwinGame.FontNamed("smallFont"), 50, 190);
                SwinGame.DrawText("High Score:", Color.White, SwinGame.FontNamed("smallFont"), 410, 150);
                SwinGame.DrawText(_gameHandler.HighScores[0].ToString(), ColourConverter(_p1Colour), SwinGame.FontNamed("smallFont"), 410, 200);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 30, 180, 350, 60);
                SwinGame.DrawText("Survival", Color.White, SwinGame.FontNamed("smallFont"), 50, 190);
            }

            //1p Score Attack
            if (MouseRegion(30, 270, 350, 60)) {
                SwinGame.FillRectangle(Color.White, 30, 270, 350, 60);
                SwinGame.DrawText("Score Attack", Color.Black, SwinGame.FontNamed("smallFont"), 50, 280);
                SwinGame.DrawText("High Score:", Color.White, SwinGame.FontNamed("smallFont"), 410, 150);
                SwinGame.DrawText("High Score:", Color.White, SwinGame.FontNamed("smallFont"), 410, 150);
                SwinGame.DrawText(_gameHandler.HighScores[3].ToString(), ColourConverter(_p1Colour), SwinGame.FontNamed("smallFont"), 410, 200);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 30, 270, 350, 60);
                SwinGame.DrawText("Score Attack", Color.White, SwinGame.FontNamed("smallFont"), 50, 280);
            }

            //2p Survival
            if (MouseRegion(30, 450, 350, 60)) {
                SwinGame.FillRectangle(Color.White, 30, 450, 350, 60);
                SwinGame.DrawText("Survival", Color.Black, SwinGame.FontNamed("smallFont"), 50, 460);
                SwinGame.DrawText("High Score:", Color.White, SwinGame.FontNamed("smallFont"), 410, 150);
                SwinGame.DrawText(_gameHandler.HighScores[1].ToString(), ColourConverter(_p1Colour), SwinGame.FontNamed("smallFont"), 410, 200);
                SwinGame.DrawText(_gameHandler.HighScores[2].ToString(), ColourConverter(_p2Colour), SwinGame.FontNamed("smallFont"), 410, 250);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 30, 450, 350, 60);
                SwinGame.DrawText("Survival", Color.White, SwinGame.FontNamed("smallFont"), 50, 460);
            }

            //2p Score Attack
            if (MouseRegion(30, 540, 350, 60)) {
                SwinGame.FillRectangle(Color.White, 30, 540, 350, 60);
                SwinGame.DrawText("Score Attack", Color.Black, SwinGame.FontNamed("smallFont"), 50, 550);
                SwinGame.DrawText("High Score:", Color.White, SwinGame.FontNamed("smallFont"), 410, 150);
                SwinGame.DrawText(_gameHandler.HighScores[4].ToString(), ColourConverter(_p1Colour), SwinGame.FontNamed("smallFont"), 410, 200);
                SwinGame.DrawText(_gameHandler.HighScores[5].ToString(), ColourConverter(_p2Colour), SwinGame.FontNamed("smallFont"), 410, 250);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 30, 540, 350, 60);
                SwinGame.DrawText("Score Attack", Color.White, SwinGame.FontNamed("smallFont"), 50, 550);
            }

            //2p Battle
            if (MouseRegion(30, 630, 350, 60)) {
                SwinGame.FillRectangle(Color.White, 30, 630, 350, 60);
                SwinGame.DrawText("Battle", Color.Black, SwinGame.FontNamed("smallFont"), 50, 640);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 30, 630, 350, 60);
                SwinGame.DrawText("Battle", Color.White, SwinGame.FontNamed("smallFont"), 50, 640);
            }

            //Colour picker setup in the bottom right
            SwinGame.DrawText("CHOOSE YOUR COLOUR", Color.White, SwinGame.FontNamed("smallFont"), 740, 250);
            SwinGame.DrawText("Player 1", Color.White, SwinGame.FontNamed("smallFont"), 705, 310);
            SwinGame.DrawText("Player 2", Color.White, SwinGame.FontNamed("smallFont"), 1005, 310);
            SwinGame.DrawRectangle(Color.White, 650, 360, 600, 330);
            SwinGame.DrawLine(Color.White, 950, 360, 950, 690);
            SwinGame.FillTriangle(_gameHandler.P1Colour, 690, 600, 910, 600, 800, 380);
            SwinGame.FillTriangle(_gameHandler.P2Colour, 990, 600, 1210, 600, 1100, 380);

            //P1 Colour Left
            if (MouseRegion(660, 620, 135, 60)) {
                SwinGame.FillRectangle(Color.White, 660, 620, 135, 60);
                SwinGame.FillTriangle(Color.Black, 690, 650, 765, 630, 765, 670);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 660, 620, 135, 60);
                SwinGame.FillTriangle(Color.White, 690, 650, 765, 630, 765, 670);
            }

            //P1 Colour Right
            if (MouseRegion(805, 620, 135, 60)) {
                SwinGame.FillRectangle(Color.White, 805, 620, 135, 60);
                SwinGame.FillTriangle(Color.Black, 910, 650, 835, 630, 835, 670);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 805, 620, 135, 60);
                SwinGame.FillTriangle(Color.White, 910, 650, 835, 630, 835, 670);
            }

            //P2 Colour Left
            if (MouseRegion(960, 620, 135, 60)) {
                SwinGame.FillRectangle(Color.White, 960, 620, 135, 60);
                SwinGame.FillTriangle(Color.Black, 990, 650, 1065, 630, 1065, 670);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 960, 620, 135, 60);
                SwinGame.FillTriangle(Color.White, 990, 650, 1065, 630, 1065, 670);
            }

            //P2 Colour Right
            if (MouseRegion(1105, 620, 135, 60)) {
                SwinGame.FillRectangle(Color.White, 1105, 620, 135, 60);
                SwinGame.FillTriangle(Color.Black, 1210, 650, 1135, 630, 1135, 670);
            }
            else {
                SwinGame.DrawRectangle(Color.White, 1105, 620, 135, 60);
                SwinGame.FillTriangle(Color.White, 1210, 650, 1135, 630, 1135, 670);
            }
        }
    }
}
