using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;


namespace MyGame.src {
    interface HostileObject {
        void Draw();
        void Update();
        Circle Shape { get; }
        bool Active { get; set; }
    }
}
