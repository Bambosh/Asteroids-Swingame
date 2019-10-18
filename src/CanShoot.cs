using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src
{
    interface CanShoot
    {
        void Shoot();
    
        MissileHandler Missiles { get; }

        int Score { get; set; }
    }
}
