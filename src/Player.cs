using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    class Player : GameObject, CanShoot
    {
        private const int SHIP_WIDTH = 20;
        private const int SHIP_HEIGHT = 50;

        private const int ROTATION_SPEED = 5;

        private const int ACCELERATION = 5;
        private const int MAX_VEL = 10;
        private const int DRAG = 100;

        private const int MAX_MISSILES = 4;
        private const int MISSILE_VEL = 15;
        private const int COOLDOWN_LENGTH = 10;

        private const int STARTING_LIVES = 3;
        private const int RESPAWN_LENGTH = 300;
        private const int MERCY_LENGTH = 180;

        private MissileHandler _missiles = new MissileHandler(MAX_MISSILES, MISSILE_VEL, COOLDOWN_LENGTH);
        private Triangle _shape;

        private int _lives, _score, _respawnTimer, _mercyTimer;
        private bool _collision, _visible, _livesEnabled;

        private bool _shootKeyLocked = true;
        private bool _thrustAnimActive = false;
        private int _thrustAnimFrame = 0;

        public Player(int x, int y, int direction, Color colour) : base(x, y, direction, colour){
            _score = 0;

            _collision = false;
            _visible = true;
            _livesEnabled = true;

            _lives = STARTING_LIVES;
            _respawnTimer = 0;
            _mercyTimer = MERCY_LENGTH;
        }

        public override void Update(){
            if (_respawnTimer <= 0){
                //Update coordinates
                _x += _xVel;
                _y += _yVel;

                //Wraps player around screen
                if (_x < -SHIP_HEIGHT) { _x = SHIP_HEIGHT + SwinGame.ScreenWidth(); }
                if (_y < -SHIP_HEIGHT) { _y = SHIP_HEIGHT + SwinGame.ScreenHeight(); }
                if (_x > SHIP_HEIGHT + SwinGame.ScreenWidth()) { _x = -SHIP_HEIGHT; }
                if (_y > SHIP_HEIGHT + SwinGame.ScreenHeight()) { _y = -SHIP_HEIGHT; }

                //Apply drag
                _xVel -= (_xVel / DRAG);
                _yVel -= (_yVel / DRAG);

                //Updates and culls missiles
                _missiles.Update();
            }

            //handles respawn mechanism
            else if (_respawnTimer > 0){
                _respawnTimer--;
                if ((_respawnTimer <= 0) && (Lives >= 0)){
                    Respawn();
                }
            }

            //Handles mercy invincibility mechanism
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

        public override void Draw(){
            if (_visible) {
                if (_thrustAnimActive) {
                    DrawThrust();
                }

                //Here's where the actual ship gets drawn
                double directionInRadians = (Math.PI * _direction) / 180;

                float x1 = _x + (SHIP_HEIGHT * (float)Math.Sin(directionInRadians));
                float y1 = _y - (SHIP_HEIGHT * (float)Math.Cos(directionInRadians));

                float x2 = _x + (SHIP_WIDTH * (float)Math.Cos(directionInRadians));
                float y2 = _y + (SHIP_WIDTH * (float)Math.Sin(directionInRadians));

                float x3 = _x - (SHIP_WIDTH * (float)Math.Cos(directionInRadians));
                float y3 = _y - (SHIP_WIDTH * (float)Math.Sin(directionInRadians));

                _shape = SwinGame.CreateTriangle(x1, y1, x2, y2, x3, y3);

                SwinGame.FillTriangle(_colour, _shape);
            }
            if (_visible || (_mercyTimer > 0)) {
                _missiles.DrawAll();
            }
            //Draws life counter next to ship during the mercy period after respawning
            if ((_mercyTimer > 0) && (LivesEnabled)) {
                SwinGame.DrawText(string.Format("x " + _lives), _colour, SwinGame.FontNamed("tinyFont"), _x + 40, _y - 10);
            }
            //Draws explosion animation if the player is dead
            if (_respawnTimer > RESPAWN_LENGTH - 90){
                if (_respawnTimer % 10 >= 5){
                    SwinGame.DrawCircle(Color.Red, _x, _y, 10);
                    SwinGame.DrawCircle(_colour, _x, _y, 20);
                }
                if (_respawnTimer % 10 < 5){
                    SwinGame.DrawCircle(_colour, _x, _y, 15);
                    SwinGame.DrawCircle(Color.Red, _x, _y, 25);
                }
            }
        }

        //Draws fire animation behind ship
        private void DrawThrust() {
            if (_visible) {
                double directionInRadians = ((Math.PI * _direction) / 180) + Math.PI;

                if (_thrustAnimFrame % 10 <= 2) {

                    float x1 = _x + (SHIP_HEIGHT / 2 * (float)Math.Sin(directionInRadians));
                    float y1 = _y - (SHIP_HEIGHT / 2 * (float)Math.Cos(directionInRadians));

                    float x2 = _x + (SHIP_WIDTH / 2 * (float)Math.Cos(directionInRadians));
                    float y2 = _y + (SHIP_WIDTH / 2 * (float)Math.Sin(directionInRadians));

                    float x3 = _x - (SHIP_WIDTH / 2 * (float)Math.Cos(directionInRadians));
                    float y3 = _y - (SHIP_WIDTH / 2 * (float)Math.Sin(directionInRadians));

                    SwinGame.DrawTriangle(Color.Red, x1, y1, x2, y2, x3, y3);
                }
                else if (_thrustAnimFrame % 10 > 2) {

                    float x1 = _x + (SHIP_HEIGHT / 3 * (float)Math.Sin(directionInRadians));
                    float y1 = _y - (SHIP_HEIGHT / 3 * (float)Math.Cos(directionInRadians));

                    float x2 = _x + (SHIP_WIDTH / 3 * (float)Math.Cos(directionInRadians));
                    float y2 = _y + (SHIP_WIDTH / 3 * (float)Math.Sin(directionInRadians));

                    float x3 = _x - (SHIP_WIDTH / 3 * (float)Math.Cos(directionInRadians));
                    float y3 = _y - (SHIP_WIDTH / 3 * (float)Math.Sin(directionInRadians));

                    SwinGame.DrawTriangle(Color.Red, x1, y1, x2, y2, x3, y3);
                }
            }
            _thrustAnimFrame++;
            if (_thrustAnimFrame > 5) {
                _thrustAnimFrame = 0;
            }
        }

        public void HandleInput(KeyCode LeftKey, KeyCode RightKey, KeyCode ThrustKey, KeyCode ShootKey){
            if (_respawnTimer <= 0) {

                if (SwinGame.KeyDown(LeftKey)) {
                    RotateLeft();
                }

                if (SwinGame.KeyDown(RightKey)) {
                    RotateRight();
                }

                if (SwinGame.KeyDown(ThrustKey)) {
                    Thrust();
                    _thrustAnimActive = true;
                }
                else { _thrustAnimActive = false; }

                if (SwinGame.KeyDown(ShootKey)) {
                    //Prevents player from holding down shoot key
                    if (!_shootKeyLocked) {
                        Shoot();
                        _shootKeyLocked = true;
                    }
                }
                else {
                    _shootKeyLocked = false;
                }
            }
        }

        private void Thrust(){
            double directionInRadians = (Math.PI * _direction) / 180;
            _xVel += (((float)Math.Sin(directionInRadians))/ACCELERATION);
            _yVel -= (((float)Math.Cos(directionInRadians))/ACCELERATION);
            if (_xVel > MAX_VEL){
                _xVel = MAX_VEL;
            }
            if (_yVel > MAX_VEL){
                _yVel = MAX_VEL;
            }
        }

        private void RotateLeft(){
            _direction = _direction - ROTATION_SPEED;
            if (_direction < 0){
                _direction = 359;
            }
        }

        private void RotateRight(){
            _direction = _direction + ROTATION_SPEED;
            if (_direction > 359){
                _direction = 0;
            }
        }

        public void Shoot(){
            _missiles.AddMissile(this, _x, _y, _direction, _colour);
        }

        public void Destroy(){
            _collision = false;
            _visible = false;
            if (LivesEnabled) {
                _lives--;
            }
            _respawnTimer = RESPAWN_LENGTH;
            _missiles.ClearAllMissiles();
            SwinGame.PlaySoundEffect(SwinGame.SoundEffectNamed("destroyPlayer"));
        }

        private void Respawn(){
            Random random = new Random();
            
            _visible = true;
            _mercyTimer = MERCY_LENGTH;
            _x = random.Next(20, SwinGame.ScreenWidth() - 20);
            _y = random.Next(20, SwinGame.ScreenHeight() - 20);
            _xVel = 0;
            _yVel = 0;
            _direction = random.Next(0, 359);
        }
        

        public MissileHandler Missiles { get => _missiles; }
        public Triangle Shape { get => _shape; }
        public Color Color { get => _colour; set => _colour = value; }

        public int Lives { get => _lives; set => _lives = value; }
        public int Score { get => _score; set => _score = value; }
        public bool LivesEnabled { get => _livesEnabled; set => _livesEnabled = value; }
        public bool Collision { get => _collision; }
    }
}
