using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    abstract class GameObject
    {
        protected float _x, _y, _xVel, _yVel;
        protected int _direction;
        protected Color _colour;

        public GameObject(float x, float y, int direction, Color colour)
        {
            _x = x;
            _y = y;
            _xVel = 0;
            _yVel = 0;
            _direction = direction;
            _colour = colour;
        }

        public abstract void Update();

        public abstract void Draw();
    }
}
