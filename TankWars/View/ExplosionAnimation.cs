//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TankWars
{
    internal class ExplosionAnimation
    {
        private Image Explosion = Image.FromFile(@"..\..\..\Resources\image\Explosion.png");
        private Vector2D location;
        private int numFrames = 5;
        public Vector2D GetLocationn()
        {
            return location;
        }

        internal int GetNumFrames()
        {
            return numFrames;
        }

        public ExplosionAnimation(Vector2D location)
        {
            this.location = location;
        }

        public void ExplosionDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Explosion, -numFrames/2, -numFrames / 2, numFrames, numFrames); 
            numFrames+=1;
        }
    }
}