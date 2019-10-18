using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    class Missile: GameObject
    {
        const int DELETION_TIMER = 60;
        const int RADIUS = 3;
        const int SCORE_VALUE = 20;

        private CanShoot _owner;
        private int _timer;
        private bool _active;

        public Missile(CanShoot owner, float x, float y, int direction, int missileVelocity, Color colour) : base (x, y, direction, colour)
        {
            _owner = owner;
            double directionInRadians = (Math.PI * direction) / 180;
            _xVel = (missileVelocity * (float)Math.Sin(directionInRadians));
            _yVel = -(missileVelocity * (float)Math.Cos(directionInRadians));
            _timer = 0;
            _active = true;
        }

        public override void Update()
        {
            _x += _xVel;
            _y += _yVel;

            //Wraps missile around screen
            if (_x < -RADIUS) { _x = RADIUS + SwinGame.ScreenWidth(); }
            if (_y < -RADIUS) { _y = RADIUS + SwinGame.ScreenHeight(); }
            if (_x > RADIUS + SwinGame.ScreenWidth()) { _x = -RADIUS; }
            if (_y > RADIUS + SwinGame.ScreenHeight()) { _y = -RADIUS; }

            //If missile has existed for too long, delete it
            _timer++;
            if (_timer >= DELETION_TIMER)
            {
                _active = false;
            }
        }

        //Adds score to whoever owns the missile_handler this missile belongs to
        public void AddScore() {
            _owner.Score += SCORE_VALUE;
        }

        public override void Draw()
        {
            SwinGame.FillCircle(_colour, _x, _y, RADIUS);
        }

        public bool Active { get => _active; set => _active = value; }

        public Circle Shape {get => SwinGame.CreateCircle(_x, _y, RADIUS);}
        public CanShoot Owner { get => _owner; set => _owner = value; }
    }
}
