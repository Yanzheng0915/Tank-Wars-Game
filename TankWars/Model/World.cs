//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    public class World
    {
        public Dictionary<int, Tank> Tanks { get; set; }
        public Dictionary<int, Projectile> Projectiles { get; set; }
        public Dictionary<int, Wall> Walls { get; set; }
        public Dictionary<int, Beam> Beams { get; set; }
        public Dictionary<int, Powerup> Powerups { get; set; }
        public Dictionary<int, ControlCommand> ControlCommands { get; set; }


        public int worldSize;

        public World()
        {
            ControlCommands = new Dictionary<int, ControlCommand>();
            Tanks = new Dictionary<int, Tank>();
            Projectiles = new Dictionary<int, Projectile>();
            Walls = new Dictionary<int, Wall>();
            Beams = new Dictionary<int, Beam>();
            Powerups = new Dictionary<int, Powerup>();
        }

        public void TankUpdate(Tank t, ControlCommand c, Vector2D aiming)
        {
            Tanks[t.ID] = t;
            ControlCommands[t.ID] = c;
            Tanks[t.ID].Orientation = aiming;
            Tanks[t.ID].Orientation.Normalize();
        }

        public void ProjectileUpdate(Projectile p)
        {
            Projectiles[p.ID] = p;
        }

        public void WallUpdate(Wall w)
        {
            Walls[w.ID] = w;
        }

        public void powerupUpdate(Powerup p)
        {
            Powerups[p.ID] = p;
        }

        public void BeamUpdate(Beam b)
        {
            Beams[b.ID] = b;
        }

        public void Setlocation(Tank tank, Vector2D l)
        {
            Tanks[tank.ID].Location = l;
        }

        public void RemoveProjectile(int ID)
        {
            Projectiles.Remove(ID);
        }
    }
}
