using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    class HostileHandler
    {
        private List<Asteroid> _asteroids = new List<Asteroid>();
        private List<Alien> _aliens = new List<Alien>();
        
        private Random _random = new Random();        

        public HostileHandler(){
        }

        //Creates @num asteroids of any size except small.
        public void NewAsteroid(int num) {
            for (int i = 0; i < num; i++) {
                int x = -100;
                int y = -100;
                int dir = _random.Next(359);
                Array values = Enum.GetValues(typeof(AsteroidSize));
                AsteroidSize rndSize = (AsteroidSize)values.GetValue(_random.Next(values.Length));
                if (rndSize == AsteroidSize.Small) { rndSize = AsteroidSize.Large; }
                _asteroids.Add(new Asteroid(x, y, dir, Color.White, rndSize));
            }
        }

        //Overload, in case I need to create an asteroid of a specific size
        public void NewAsteroid(int num, AsteroidSize size) {
            for (int i = 0; i < num; i++) {
                int x = -100;
                int y = -100;
                int dir = _random.Next(359);
                _asteroids.Add(new Asteroid(x, y, dir, Color.White, size));
            }
        }

        //Overload to create an asteroid with the exact properties I need
        public void NewAsteroid(int x, int y, int dir, AsteroidSize size) {
            _asteroids.Add(new Asteroid(x, y, dir, Color.White, size));
        }

        //Creates single new alien at random position
        public void NewAlien() {
            int x = _random.Next(SwinGame.ScreenWidth());
            int y = _random.Next(SwinGame.ScreenHeight());
            int dir = _random.Next(359);
            _aliens.Add(new Alien(x, y, dir, Color.ForestGreen));
        }

        public void ClearAllHostiles() {
            _asteroids.Clear();
            _aliens.Clear();
        }

        public void Update(){
            int LargeAsteroidCount = 0;
            foreach (Asteroid a in _asteroids) {
                a.Update();
                //Count number of larger asteroids, to decide whether to spawn more
                if (a.Size != AsteroidSize.Small){
                    LargeAsteroidCount++;
                }
            }
            foreach (Alien a in _aliens) {
                a.Update();
                a.Shoot();
            }
            //Adds more hostiles if enough have been destroyed  
            if (LargeAsteroidCount < 15) {
                NewAsteroid(15);
                NewAlien();
            }
            //Removes any inactive hostiles from memory
            CullHostiles();
        }

        public void DrawAll(){
            foreach (Asteroid a in _asteroids) {
                a.Draw();
            }
            foreach (Alien a in _aliens) {
                a.Draw();
            }
        }

        private int WrapTo360(int dir) {
            if (dir > 359) {
                dir = dir - 360;
            }
            if (dir < 0) {
                dir = dir + 360;
            }
            return dir;
        }

        private void CullHostiles() {
            for (int i = _asteroids.Count - 1; i >= 0; i--) {
                if (!_asteroids[i].Active) {
                    DestroyAsteroid(_asteroids[i]);                    
                } 
            }
            for (int i = _aliens.Count - 1; i >= 0; i--) { 
                if (!_aliens[i].Active) {
                    _aliens.Remove(_aliens[i]);
                }
            }
        }

        //Splits asteroid into numerous smaller asteroids at the same position
        //Gives each one a different direction
        private void DestroyAsteroid(Asteroid a) {
            int x = (int)a.X;
            int y = (int)a.Y;
            int randDir = _random.Next(359);
            int randDirAdjusted = WrapTo360(randDir + _random.Next(20, 300));
            int randDirPlus120 = WrapTo360(randDir + 120);
            int randDirPlus240 = WrapTo360(randDir + 240);

            _asteroids.Remove(a);

            if (a.Size == AsteroidSize.Extra) {
                NewAsteroid(x, y, randDir, AsteroidSize.Large);
                NewAsteroid(x, y, randDirAdjusted, AsteroidSize.Large);
            }
            else if (a.Size == AsteroidSize.Large) {
                NewAsteroid(x, y, randDir, AsteroidSize.Medium);
                NewAsteroid(x, y, randDirPlus120, AsteroidSize.Medium);
                NewAsteroid(x, y, randDirPlus240, AsteroidSize.Medium);
            }
            else if (a.Size == AsteroidSize.Medium) {
                NewAsteroid(x, y, randDir, AsteroidSize.Small);
                NewAsteroid(x, y, randDirAdjusted, AsteroidSize.Small);
            }
        }

        public void CollidesWithHostile(Player p) {
            //Behaviour for Asteroids
            foreach (Asteroid a in _asteroids) {
                //Check player + asteroid collision
                if (SwinGame.CircleTriangleCollision(a.Shape, p.Shape) && p.Collision) {
                    p.Destroy();
                }
                //Check player missle + asteroid collision
                foreach (Missile playerMissile in p.Missiles._missles) {
                    if (SwinGame.CircleCircleCollision(a.Shape, playerMissile.Shape)) {
                        SwinGame.PlaySoundEffect(SwinGame.SoundEffectNamed("hitEnemy"));
                        a.Active = false;
                        playerMissile.Active = false;
                        playerMissile.AddScore();
                    }
                }
            }
            //Behaviour for aliens
            foreach(Alien a in _aliens) {
                //Check player + alien collision
                if (SwinGame.CircleTriangleCollision(a.Shape, p.Shape) && p.Collision) {
                    p.Destroy();
                }
                //Check player + alien missiles collision
                a.Missiles.PlayerCollidesWithMissile(p);
                //Check player missle + alien collision
                foreach (Missile playerMissile in p.Missiles._missles) {
                    if (SwinGame.CircleCircleCollision(a.Shape, playerMissile.Shape) && a.Collision) {
                        a.Destroy();
                        playerMissile.Active = false;
                        playerMissile.AddScore();
                    }
                }
            }
        }
    }
}

