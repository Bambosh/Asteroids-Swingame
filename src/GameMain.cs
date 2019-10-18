using System;
using SwinGameSDK;

namespace MyGame.src
{
    public class GameMain
    {
        public static void Main()
        {
            GameHandler gameHandler = new GameHandler(EnumState.GameMenu);

            SwinGame.LoadFontNamed("titleFont", "cour.ttf", 120);
            SwinGame.LoadFontNamed("bigFont", "cour.ttf", 60);
            SwinGame.LoadFontNamed("smallFont", "cour.ttf", 40);
            SwinGame.LoadFontNamed("tinyFont", "cour.ttf", 25);

            SwinGame.OpenAudio();
            SwinGame.LoadSoundEffectNamed("shoot", "Shoot.ogg");
            SwinGame.LoadSoundEffectNamed("hitEnemy", "HitEnemy.ogg");
            SwinGame.LoadSoundEffectNamed("destroyAlien", "DestroyAlien.ogg");
            SwinGame.LoadSoundEffectNamed("destroyPlayer", "DestroyPlayer.ogg");
            
            SwinGame.OpenGraphicsWindow("GameMain", 1280, 720);
           
            while (!SwinGame.WindowCloseRequested())
            {
                gameHandler.HandleInput();
                gameHandler.UpdateState();
                gameHandler.DrawGame();
            }
            SwinGame.CloseAudio();
            SwinGame.ReleaseAllResources();
        }
    }
}