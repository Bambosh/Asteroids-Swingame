using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src {
    interface Mode {
        void HandleInput();

        void UpdateState();

        void DrawAll();
    }
}
