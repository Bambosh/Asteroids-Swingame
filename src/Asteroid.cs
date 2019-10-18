using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    class Asteroid: GameObject, HostileObject
    {
        private AsteroidSize _size;
        private Circle _shape;
        private bool _active;


        public Asteroid(int x, int y, int direction, Color colour, AsteroidSize size) : base(x, y, direction, colour){
            _size = size;
            _xVel = 2;
            _yVel = 2;

            _active = true;
        }

        //Determines the size of each asteroid class
        private float GetRadius() {
            if (_size == AsteroidSize.Extra) {
                return 100;
            }
            if (_size == AsteroidSize.Large) {
                return 50;
            }
            if (_size == AsteroidSize.Medium) {
                return 35;
            }
            else {
                return 20;
            }
        }

        public override void Draw(){
            _shape = SwinGame.CreateCircle(_x, _y, GetRadius());
            SwinGame.FillCircle(_colour, _shape);
        }

        public override void Update(){
            double directionInRadians = (Math.PI * _direction) / 180;
            _xVel = (float)Math.Sin(directionInRadians);
            _yVel = (float)Math.Cos(directionInRadians);

            //Update coordinates
            _x +=_xVel;
            _y += _yVel;

            //Wrap asteroid around screen if it is not small
            if (_size != AsteroidSize.Small) {
                if (_x < -GetRadius()) { _x = GetRadius() + SwinGame.ScreenWidth(); }
                if (_y < -GetRadius()) { _y = GetRadius() + SwinGame.ScreenHeight(); }
                if (_x > GetRadius() + SwinGame.ScreenWidth()) { _x = -GetRadius(); }
                if (_y > GetRadius() + SwinGame.ScreenHeight()) { _y = -GetRadius(); }
            }
            //If asteroid is small, remove it from memory when it goes offscreen
            else {
                if (_x < -GetRadius()) { Active = false; }
                if (_y < -GetRadius()) { Active = false; }
                if (_x > GetRadius() + SwinGame.ScreenWidth()) { Active = false; }
                if (_y > GetRadius() + SwinGame.ScreenHeight()) { Active = false; }
            }
        }

        public AsteroidSize Size { get => _size; }
        public Circle Shape { get => _shape; }
        public float X { get => _x; }
        public float Y { get => _y; }
        public bool Active { get => _active; set => _active = value; }
    }
}
