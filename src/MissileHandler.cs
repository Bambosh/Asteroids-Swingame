using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace MyGame.src
{
    class MissileHandler
    {
        public List<Missile> _missles = new List<Missile>();

        private int _cooldown, _cooldownLength;
        private int _maxMissiles, _missileVelocity;

        public MissileHandler(int maxMissiles, int missileVelocity, int cooldownLength)
        {
            _cooldown = 0;
            _cooldownLength = cooldownLength;
            _maxMissiles = maxMissiles;
            _missileVelocity = missileVelocity;
        }

        public void AddMissile(CanShoot owner, float x, float y, int direction, Color colour)
        {
            /* Will only add a missile if:
             * 1. The cooldown timer is not currently active
             * 2. The maximum number of missiles on-screen has not been reached
             */            
            if ((_cooldown <= 0) && (_missles.Count < _maxMissiles)) {
                SwinGame.PlaySoundEffect(SwinGame.SoundEffectNamed("shoot"));
                _missles.Add(new Missile(owner, x, y, direction, _missileVelocity, colour));
                _cooldown = _cooldownLength;
            }
        }

        public void Update()
        {
            UpdateMissiles();
            CullMissiles();

            if (_cooldown > 0)
            {
                _cooldown--;
            }
        }

        private void UpdateMissiles()
        {
            foreach (Missile m in _missles)
            {
                m.Update();
            }
        }

        private void CullMissiles()
        {
            //Removes all inactive missiles from memory
            for (int i = _missles.Count - 1; i >= 0; i--)
            {
                if (!_missles[i].Active)
                {
                    _missles.RemoveAt(i);
                }
            }
        }

        public void ClearAllMissiles()
        {
            _missles.Clear();
        }

        public void DrawAll()
        {
            foreach (Missile m in _missles)
            {
                m.Draw();
            }
        }

        public void PlayerCollidesWithMissile(Player p) {
            //Checks all missiles against the passed-in player object
            foreach (Missile m in _missles) {
                if ((SwinGame.CircleTriangleCollision(m.Shape, p.Shape)) && (p.Collision)) {
                    p.Destroy();
                    m.Active = false;
                }                
            }
        }
    }
}
