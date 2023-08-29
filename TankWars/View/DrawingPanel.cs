//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    public class DrawingPanel : Panel
    {
        private GameController theGameController;
        private HashSet<BeamAnimation> beamAnimations = new HashSet<BeamAnimation>();
        private HashSet<ExplosionAnimation> explosionAnimations = new HashSet<ExplosionAnimation>();
        /////////Image////////
        private Image backgroundImage = Image.FromFile(@"..\..\..\Resources\image\Background.png");
        private Image wallImage = Image.FromFile(@"..\..\..\Resources\image\WallSprite.png");
        private Image redTankImage = Image.FromFile(@"..\..\..\Resources\image\RedTank.png");
        private Image BlueTankImage = Image.FromFile(@"..\..\..\Resources\image\BlueTank.png");
        private Image DarkTankImage = Image.FromFile(@"..\..\..\Resources\image\DarkTank.png");
        private Image GreenTank = Image.FromFile(@"..\..\..\Resources\image\GreenTank.png");
        private Image LightGreenTank = Image.FromFile(@"..\..\..\Resources\image\LightGreenTank.png");
        private Image OrangeTank = Image.FromFile(@"..\..\..\Resources\image\OrangeTank.png");
        private Image PurpleTank = Image.FromFile(@"..\..\..\Resources\image\PurpleTank.png");
        private Image YellowTank = Image.FromFile(@"..\..\..\Resources\image\YellowTank.png");
        private Image redTurretIamge = Image.FromFile(@"..\..\..\Resources\image\RedTurret.png");
        private Image BlueTurret = Image.FromFile(@"..\..\..\Resources\image\BlueTurret.png");
        private Image DarkTurret = Image.FromFile(@"..\..\..\Resources\image\DarkTurret.png");
        private Image GreenTurret = Image.FromFile(@"..\..\..\Resources\image\GreenTurret.png");
        private Image LightGreenTurret = Image.FromFile(@"..\..\..\Resources\image\LightGreenTurret.png");
        private Image OrangeTurret = Image.FromFile(@"..\..\..\Resources\image\OrangeTurret.png");
        private Image PurpleTurret = Image.FromFile(@"..\..\..\Resources\image\PurpleTurret.png");
        private Image YellowTurret = Image.FromFile(@"..\..\..\Resources\image\YellowTurret.png");
        private Image redShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-red.png");
        private Image blueShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-blue.png");
        private Image brownShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-brown.png");
        private Image greenShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-green.png");
        private Image greyShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-grey.png");
        private Image violetShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-violet.png");
        private Image whiteShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-white.png");
        private Image yellowShotIamge = Image.FromFile(@"..\..\..\Resources\image\shot-yellow.png");
        private Image powerUpImage = Image.FromFile(@"..\..\..\Resources\image\powerUp.png");


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gc"></param>
        public DrawingPanel(GameController gc)
        {
            DoubleBuffered = true;
            theGameController = gc;
        }


        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, int wordSize, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            e.Graphics.TranslateTransform(coordinateOffset(wordSize, worldX), coordinateOffset(wordSize, worldY));
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Draw the image of tank
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {

            Tank t = o as Tank;
            if (t.HitPoints == 0)
                return;
            int ts = Constant.tankSize;
            Image tankSource = redTankImage;
            //given a unique color to tank
            int color = theGameController.getTankColor(t.ID);
            switch (color)
            {
                case 1:
                    tankSource = BlueTankImage;
                    break;
                case 2:
                    tankSource = DarkTankImage;
                    break;
                case 3:
                    tankSource = GreenTank;
                    break;
                case 4:
                    tankSource = LightGreenTank;
                    break;
                case 5:
                    tankSource = OrangeTank;
                    break;
                case 6:
                    tankSource = PurpleTank;
                    break;
                case 7:
                    tankSource = YellowTank;
                    break;
            }
            e.Graphics.DrawImage(tankSource, -ts / 2, -ts / 2, ts, ts);
        }

        /// <summary>
        /// Draw the image of wall
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(wallImage, 0, 0, Constant.wallSize, Constant.wallSize);
        }

        /// <summary>
        /// Draw the image of turret
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            if (t.HitPoints == 0)
                return;
            Image turretSource = redTurretIamge;
            //given a unique color to turret
            int color = theGameController.getTankColor(t.ID);
            switch (color)
            {
                case 1:
                    turretSource = BlueTurret;
                    break;
                case 2:
                    turretSource = DarkTurret;
                    break;
                case 3:
                    turretSource = GreenTurret;
                    break;
                case 4:
                    turretSource = LightGreenTurret;
                    break;
                case 5:
                    turretSource = OrangeTurret;
                    break;
                case 6:
                    turretSource = PurpleTurret;
                    break;
                case 7:
                    turretSource = YellowTurret;
                    break;
            }
            int ts = Constant.turretSize;
            e.Graphics.DrawImage(turretSource, -ts / 2, -ts / 2, ts, ts);
        }

        /// <summary>
        /// Draw the image of tank accessories incloud health bar name and score
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TankAccessoriesDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            if (t.HitPoints == 0)
                return;
            Color c;
            int healthBarLength = 60;
            if (t.HitPoints == 3)
                c = Color.Green;
            else if (t.HitPoints == 2)
            {
                c = Color.Blue;
                healthBarLength = 40;
            }
            else
            {
                c = Color.Red;
                healthBarLength = 20;
            }

            e.Graphics.FillRectangle(new SolidBrush(c), new Rectangle(-30, -60, healthBarLength, Constant.healthBarHight));
            e.Graphics.DrawString(t.Name + ":" + t.Score, new Font("Arial", 25), Brushes.Black, -60, 35);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {

            int powsize = Constant.powerUpSize;
            e.Graphics.DrawImage(powerUpImage, -powsize / 2, -powsize / 2, powsize, powsize);
        }

        /// <summary>
        /// Draw the image of projectile
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void projectileDrawer(object o, PaintEventArgs e)
        {
            Projectile p = o as Projectile;
            int color = theGameController.getTankColor(p.Owner);
            Image projectileSource = redShotIamge;
            //giving a color ot projectile
            switch (color)
            {
                case 1:
                    projectileSource = blueShotIamge;
                    break;
                case 2:
                    projectileSource = brownShotIamge;
                    break;
                case 3:
                    projectileSource = greenShotIamge;
                    break;
                case 4:
                    projectileSource = greyShotIamge;
                    break;
                case 5:
                    projectileSource = violetShotIamge;
                    break;
                case 6:
                    projectileSource = whiteShotIamge;
                    break;
                case 7:
                    projectileSource = yellowShotIamge;
                    break;
            }

            int ps = Constant.projectilSize;
            e.Graphics.DrawImage(projectileSource, -ps / 2, -ps / 2, ps, ps);
        }

        /// <summary>
        /// adjust world coordingante to space coordingate 
        /// </summary>
        /// <param name="wordSize"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private static int coordinateOffset(int wordSize, double location)
        {
            return (int)(location + wordSize / 2);
        }

        /// <summary>
        /// adding beam animation 
        /// </summary>
        /// <param name="b"></param>
        public void AddBeamAnimation(Beam b)
        {
            BeamAnimation BA = new BeamAnimation(b.Origin, b.Direction);
            this.Invoke(new MethodInvoker(() => { beamAnimations.Add(BA); }));
        }

        /// <summary>
        /// adding explosion animation 
        /// </summary>
        /// <param name="t"></param>
        public void AddExplosionAnimiation(Tank t)
        {
            ExplosionAnimation EA = new ExplosionAnimation(t.Location);
            explosionAnimations.Add(EA);
        }

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            lock (theGameController.theWorld)
            {
                if (!theGameController.wallDataReceived)
                    return;
                int ws = theGameController.theWorld.worldSize;

                //set the location 
                double playerX = theGameController.getPlayerTank().Location.GetX();
                double playerY = theGameController.getPlayerTank().Location.GetY();
                int offSet = (int)(ws / 2.0 * Constant.viewSize / ws);
                double inverseTranslateX = -coordinateOffset(theGameController.theWorld.worldSize, playerX) + offSet;
                double inverseTranslateY = -coordinateOffset(theGameController.theWorld.worldSize, playerY) + offSet;

                e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);
                e.Graphics.DrawImage(backgroundImage, 0, 0, ws, ws);

                //draw the wall 
                foreach (Wall w in theGameController.theWorld.Walls.Values)
                {
                    int wallSize = Constant.wallSize;
                    int length = (int)Math.Abs(w.Endpoint1.GetX() - w.Endpoint2.GetX());
                    int weight = (int)Math.Abs(w.Endpoint1.GetY() - w.Endpoint2.GetY());

                    int xBound = -wallSize / 2 + (int)Math.Min(w.Endpoint1.GetX(), w.Endpoint2.GetX());
                    int yBound = -wallSize / 2 + (int)Math.Min(w.Endpoint1.GetY(), w.Endpoint2.GetY());
                    for (int i = 0; i <= length; i += wallSize)
                    {
                        for (int m = 0; m <= weight; m += wallSize)
                        {
                            DrawObjectWithTransform(e, w, xBound + i, yBound + m, 0, ws, WallDrawer);
                        }
                    }
                }
                //drwa the powerup
                foreach (Powerup p in theGameController.theWorld.Powerups.Values)
                {
                    if (!p.IsDied)
                        DrawObjectWithTransform(e, p, p.Location.GetX(), p.Location.GetY(), 0, theGameController.theWorld.worldSize, PowerupDrawer);
                }
                //drwa the tank
                foreach (Tank t in theGameController.theWorld.Tanks.Values)
                {
                    DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY(), t.Orientation.ToAngle(), theGameController.theWorld.worldSize, TankDrawer);
                    DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY(), t.Aiming.ToAngle(), theGameController.theWorld.worldSize, TurretDrawer);
                    DrawObjectWithTransform(e, t, t.Location.GetX(), t.Location.GetY(), 0, theGameController.theWorld.worldSize, TankAccessoriesDrawer);
                }
                //drwa the Projectile
                foreach (Projectile p in theGameController.theWorld.Projectiles.Values)
                {
                    if (!p.IsDied)
                        DrawObjectWithTransform(e, p, p.Location.GetX(), p.Location.GetY(), p.Orientation.ToAngle(), theGameController.theWorld.worldSize, projectileDrawer); ;
                }
                //drwa the Explosion Animation
                HashSet<ExplosionAnimation> explosionAnimationsToBeRemoved = new HashSet<ExplosionAnimation>();
                foreach (ExplosionAnimation ea in explosionAnimations)
                {
                    double x = ea.GetLocationn().GetX();
                    double y = ea.GetLocationn().GetY();
                    DrawObjectWithTransform(e, ea, x, y, 0, theGameController.theWorld.worldSize, ea.ExplosionDrawer);
                    if (ea.GetNumFrames() > 60)
                    {
                        explosionAnimationsToBeRemoved.Add(ea);
                    }
                }
                foreach (ExplosionAnimation ea in explosionAnimationsToBeRemoved)
                {
                    explosionAnimations.Remove(ea);
                }
                //drwa the Beam Animation
                HashSet<BeamAnimation> beamAnimationsToBeRemoved = new HashSet<BeamAnimation>();
                foreach (BeamAnimation b in beamAnimations)
                {
                    double x = b.GetOrigin().GetX();
                    double y = b.GetOrigin().GetY();
                    double angle = b.GetOrientation().ToAngle();
                    DrawObjectWithTransform(e, b, x, y, angle, theGameController.theWorld.worldSize, b.BeamDrawer);
                    if (b.GetNumFrames() > 20)
                    {
                        beamAnimationsToBeRemoved.Add(b);
                    }
                }
                foreach (BeamAnimation b in beamAnimationsToBeRemoved)
                {
                    beamAnimations.Remove(b);
                }
            }
            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }


    }
}

