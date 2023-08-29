//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;
using System.Windows.Forms;
using System.Drawing;

namespace TankWars
{
    class BeamAnimation
    {
        private Vector2D origin;
        private Vector2D orientation;
        private int numFrames = 0;

        public BeamAnimation(Vector2D position, Vector2D dir)
        {
            origin = new Vector2D(position);
            orientation = new Vector2D(dir);
        }

        public Vector2D GetOrigin()
        {
            return origin;
        }

        public Vector2D GetOrientation()
        {
            return orientation;
        }

        public int GetNumFrames()
        {
            return numFrames;
        }

        public void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = o as Beam;
            using(Pen pen = new Pen(Color.White, 20.0F - numFrames))
            {
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
            }
            numFrames++;
        }
    }
}
