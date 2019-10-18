using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    class Alien: GameObject, HostileObject, CanShoot {

        private const int MAX_MISSILES = 2;
        private const int MISSILE_VEL = 5;
        private const int COOLDOWN_LENGTH = 60;

        private const int ALIEN_SIZE = 35;
        private const int ALIEN_VELOCITY = 3;

        private const int LIVES = 5;
        private const int MERCY_LENGTH = 90;

        private int _score;
        private int _lives, _mercyTimer;
        private bool _collision, _visible, _active;
        private Circle _shape;

        private MissileHandler _missiles = new MissileHandler(MAX_MISSILES, MISSILE_VEL, COOLDOWN_LENGTH);

        public Alien(int x, int y, int direction, Color colour) : base(x, y, direction, colour){
            _score = 0;
            _xVel = 0;
            _yVel = 0;
            _lives = LIVES;

            //Snaps alien offscreen on spawn so it can slide in, rather than popping in.
            if ((_direction <= 45) || ((_direction >= 135) && (_direction <= 225)) || (_direction >= 315)) {
                _x = -ALIEN_SIZE - 25;
            }
            else {
                _y = -ALIEN_SIZE - 25;
            }

            _active = true;
            _collision = true;
            _visible = true;
        }

        //Returns a gradient from green to red
        //Based on a percentage of current lives over maximum lives
        public Color GetColorFromLives() {
            int health = (_lives*100)/LIVES;
            if (health > 80) {
                return Color.Green;
            }
            else if ((health <=80) && (health > 60)) {
                return Color.YellowGreen;
            }
            else if ((health <= 60) && (health > 40)) {
                return Color.Yellow;
            }
            else if ((health <= 40) && (health > 20)) {
                return Color.OrangeRed;
            }
            else {
                return Color.Red;
            }
        }

        public override void Draw(){
            _shape = SwinGame.CreateCircle(_x, _y, ALIEN_SIZE-5);
            if (_visible) {
                SwinGame.FillRectangle(_colour, _x-ALIEN_SIZE, _y-ALIEN_SIZE, ALIEN_SIZE*2, ALIEN_SIZE*2);
                SwinGame.FillCircle(GetColorFromLives(), _shape);
            }
            _missiles.DrawAll();
        }

        public void Destroy() {
            //Only destroys the alien if it has no lives left
            //Otherwise, goes into a mercy-invincibility state

            _collision = false;
            _mercyTimer = MERCY_LENGTH;
            _lives--;
            if (_lives == 0) {
                _missiles.ClearAllMissiles();
                _active = false;
                SwinGame.PlaySoundEffect(SwinGame.SoundEffectNamed("destroyAlien"));
            }
            else {
                SwinGame.PlaySoundEffect(SwinGame.SoundEffectNamed("hitEnemy"));
            }
        }

        public override void Update(){

            if ((_direction <= 45) || ((_direction >= 135) && (_direction <= 225)) || (_direction >= 315)) {
                //Horizontal Movement
                _yVel = 0;
                if (_x <= 0) {
                    _xVel = ALIEN_VELOCITY;
                }
                if (_x >= SwinGame.ScreenWidth() - ALIEN_SIZE) {
                    _xVel = -ALIEN_VELOCITY;
                }
            }
            else {
                //Vertical Movement
                _xVel = 0;
                if (_y <= 0) {
                    _yVel = ALIEN_VELOCITY;
                }
                if (_y >= SwinGame.ScreenHeight() - ALIEN_SIZE) {
                    _yVel = -ALIEN_VELOCITY;
                }
            }
            
            _x += _xVel;
            _y += _yVel;

            _missiles.Update();

            //Handles mercy invincibility
            if (_mercyTimer > 0) {
                _mercyTimer--;
                if ((_mercyTimer % 10 == 5) || (_mercyTimer % 10 == 0)) {
                    _visible = !_visible;
                }
                if (_mercyTimer <= 0) {
                    _collision = true;
                }
            }
        }

        public void Shoot() {
            Random rand = new Random();
            int dir = rand.Next(359);

            _missiles.AddMissile(this, _x, _y, dir, _colour);
        }

        public MissileHandler Missiles { get => _missiles; }
        public Circle Shape { get => _shape; }
        public int Score { get => _score; set => _score = value; }
        public bool Active { get => _active; set => _active = value; }
        public bool Collision { get => _collision; set => _collision = value; }
    }
}
