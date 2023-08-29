//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankWars
{
    public partial class Form1 : Form
    {
        private GameController theController;

        private World theWorld;

        DrawingPanel drawingPanel;

        delegate void ReSetConnect(bool b);

        public Form1(GameController ctl)
        {
            InitializeComponent();
            theController = ctl;
            theWorld = theController.GetWorld();

            // register the handler that can deal with the error and inform the world changed
            theController.WorldChangedEvent += OnFrame;
            theController.ErrorEvent += ShowError;

            this.KeyDown += HandleKeyDown;
            this.KeyUp += HandleKeyUp;

            drawingPanel = new DrawingPanel(theController);
            drawingPanel.AutoSize = true;
            drawingPanel.Location = new Point(10, 50);
            drawingPanel.Size = new Size(Constant.viewSize, Constant.viewSize);
            drawingPanel.BackColor = Color.Black;
            Controls.Add(drawingPanel);

            // register the handler that can deal with mouse clicking
            drawingPanel.MouseDown += HandleMouseDown;
            drawingPanel.MouseUp += HandleMouseUp;
            drawingPanel.MouseMove += HandleMouseMoving;

            // register the handler that can deal with the beam and tank dying animiations
            theController.BeamFired += drawingPanel.AddBeamAnimation;
            theController.TankExplosion += drawingPanel.AddExplosionAnimiation;
        }

        /// <summary>
        /// method to invoke to redrew every frame
        /// </summary>
        private void OnFrame()
        {
            if (!IsHandleCreated)
                return;
            MethodInvoker mIv = new MethodInvoker(() => Invalidate(true));
            try
            {
                Invoke(mIv);
            }
            catch { }
        }

        /// <summary>
        /// to deal with when connect button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            entered_server_box.Enabled = false;
            entered_name_box.Enabled = false;
            KeyPreview = true;
            theController.Connect(entered_server_box.Text, entered_name_box.Text);
        }

        /// <summary>
        /// display an error message 
        /// </summary>
        /// <param name="err"></param>
        private void ShowError(string err)
        {
            ReSet(true);
            MessageBox.Show(err, "error", MessageBoxButtons.OK);
        }

        /// <summary>
        /// reset connection button
        /// </summary>
        /// <param name="b"></param>
        private void ReSet(bool b)
        {
            if (ConnectButton.InvokeRequired)
            {
                ReSetConnect d = new ReSetConnect(ReSet);
                Invoke(d, new object[] { b });
            }
            else
            {
                ConnectButton.Enabled = b;
                entered_server_box.Enabled = true;
                entered_name_box.Enabled = true;
                KeyPreview = false;
            }
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            else if (e.KeyCode == Keys.W)
                theController.HandleMoveRequest("up");
            else if (e.KeyCode == Keys.A)
                theController.HandleMoveRequest("left");
            else if (e.KeyCode == Keys.S)
                theController.HandleMoveRequest("down");
            else if (e.KeyCode == Keys.D)
                theController.HandleMoveRequest("right");

            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }


        /// <summary>
        /// Key up handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                theController.CancelMoveRequest("up");
            else if (e.KeyCode == Keys.A)
                theController.CancelMoveRequest("left");
            else if (e.KeyCode == Keys.S)
                theController.CancelMoveRequest("down");
            else if (e.KeyCode == Keys.D)
                theController.CancelMoveRequest("right");
        }

        /// <summary>
        /// Handle mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                theController.HandleMouseRequest("main");
            else if (e.Button == MouseButtons.Right)
                theController.HandleMouseRequest("alt");
        }

        /// <summary>
        /// Handle mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            theController.CancelMouseRequest();
        }

        /// <summary>
        /// Handle mouse moving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseMoving(object sender, MouseEventArgs e)
        {
            theController.turretDirectionRequest(e.X, e.Y);
        }
    }
}
