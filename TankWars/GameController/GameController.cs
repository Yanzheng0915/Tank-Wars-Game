//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TankWars
{
    public class GameController
    {
        public delegate void ErrorEventHandler(string err);
        public event ErrorEventHandler ErrorEvent;

        public delegate void WorldChangedEventHandler();
        public event WorldChangedEventHandler WorldChangedEvent;

        public delegate void BeamFiredEventHandler(Beam b);
        public event BeamFiredEventHandler BeamFired;

        public delegate void TankExplosionEventHandler(Tank t);
        public event TankExplosionEventHandler TankExplosion;

        private string playerName;
        private int playerID;
        private int color = 0;

        // This variable is to check whether the wall data is sent
        public bool wallDataReceived = false;

        private readonly ControlCommand cc = new ControlCommand();
        private readonly Dictionary<int, int> colorOfObjec = new Dictionary<int, int>();

        public World theWorld = new World();

        private bool upKeyPressed = false;
        private bool leftKeyPressed = false;
        private bool downKeyPressed = false;
        private bool rightKeyPressed = false;

        /// <summary>
        /// This method is to connect with the server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="name"></param>
        public void Connect(string server, string name)
        {
            if (name.Length == 0)
            {
                ErrorEvent("Please put the name in the textbox");
                return;
            }
            if (name.Length <= 16)
            {
                playerName = name;
            }
            else
            {
                ErrorEvent("The name should be no longer than 16 characters");
                return;
            }

            Networking.ConnectToServer(OnConnect, server, 11000);
        }

        /// <summary>
        /// This method is invoked when the connection is ready.
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                ErrorEvent(state.ErrorMessage);
                return;
            }

            Networking.Send(state.TheSocket, playerName + "\n");

            // The socket is ready to receive the beginning message from the server
            state.OnNetworkAction = ReceiveMessage;
            Networking.GetData(state);
        }

        /// <summary>
        /// This method is to receive the playID and worldSize from the server
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveMessage(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                ErrorEvent(state.ErrorMessage);
                return;
            }

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"\n");

            playerID = Int32.Parse(parts[0]);
            theWorld.worldSize = Int32.Parse(parts[1]);
            // Clean the string builder in the socket state
            state.RemoveData(0, playerID.ToString().Length + theWorld.worldSize.ToString().Length + 2);
            ProcessMessages(state);

            // Change the OnNetworkAction to receive data from every frame
            state.OnNetworkAction = ReceiveFrameData;
            Networking.GetData(state);
        }

        /// <summary>
        /// This method is to receive data from every frame
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveFrameData(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                ErrorEvent(state.ErrorMessage);
                return;
            }

            ProcessMessages(state);
            // Inform the View that the world has changed
            WorldChangedEvent();

            if (wallDataReceived)
            {
                Networking.Send(state.TheSocket, JsonConvert.SerializeObject(cc) + '\n');
                cc.FireType = "none";
            }

            Networking.GetData(state);
        }

        /// <summary>
        /// This method is to process message from the server
        /// </summary>
        /// <param name="state"></param>
        private void ProcessMessages(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            lock (theWorld)
            {
                foreach (string p in parts)
                {
                    // Ignore empty strings added by the regex splitter
                    if (p.Length == 0)
                        continue;
                    // The regex splitter will include the last string even if it doesn't end with a '\n',
                    // So we need to ignore it if this happens. 
                    if (p[p.Length - 1] != '\n')
                        break;

                    UpdateModel(p);

                    // Then remove it from the SocketState's growable buffer
                    state.RemoveData(0, p.Length);
                }
            }

        }

        /// <summary>
        /// This method is to update the world with information from the server
        /// </summary>
        /// <param name="jsonMessage"></param>
        private void UpdateModel(string jsonMessage)
        {
            JObject obj = JObject.Parse(jsonMessage);

            // This is the case that is a tank.
            JToken token = obj["tank"];
            if (token != null)
            {
                wallDataReceived = true;
                Tank tank = JsonConvert.DeserializeObject<Tank>(jsonMessage);
                lock (theWorld)
                {
                    if (!colorOfObjec.ContainsKey(tank.ID))
                    {
                        if (color >= 8)
                            color = 0;
                        colorOfObjec.Add(tank.ID, color);
                        color++;
                    }

                    if (tank.Disconnected || tank.Died || tank.HitPoints == 0)
                    {
                        theWorld.Tanks.Remove(tank.ID);
                        colorOfObjec.Remove(tank.ID);
                        TankExplosion(tank);
                    }

                    if (theWorld.Tanks.ContainsKey(tank.ID))
                    {
                        theWorld.Tanks[tank.ID] = tank;
                    }
                    else
                    {
                        theWorld.Tanks.Add(tank.ID, tank);
                    }
                }
                return;
            }

            // This is the case that is a projectile.
            token = obj["proj"];
            if (token != null)
            {
                Projectile projectile = JsonConvert.DeserializeObject<Projectile>(jsonMessage);
                lock (theWorld)
                {
                    if (projectile.IsDied)
                    {
                        theWorld.Projectiles.Remove(projectile.ID);
                    }

                    if (theWorld.Projectiles.ContainsKey(projectile.ID))
                    {
                        theWorld.Projectiles[projectile.ID] = projectile;
                    }
                    else
                    {
                        theWorld.Projectiles.Add(projectile.ID, projectile);
                    }
                }
                return;
            }

            // This is the case that is a wall.
            token = obj["wall"];
            if (token != null)
            {
                Wall wall = JsonConvert.DeserializeObject<Wall>(jsonMessage);
                lock (theWorld)
                {
                    if (theWorld.Walls.ContainsKey(wall.ID))
                    {
                        theWorld.Walls[wall.ID] = wall;
                    }
                    else
                    {
                        theWorld.Walls.Add(wall.ID, wall);
                    }
                }
                return;
            }

            // This is the case that is a beam
            token = obj["beam"];
            if (token != null)
            {
                Beam beam = JsonConvert.DeserializeObject<Beam>(jsonMessage);
                // Fire the event to draw the beam animation
                BeamFired(beam);
                return;
            }

            // This is the case that is a power.
            token = obj["power"];
            if (token != null)
            {
                Powerup powerup = JsonConvert.DeserializeObject<Powerup>(jsonMessage);
                lock (theWorld)
                {
                    if (powerup.IsDied)
                    {
                        theWorld.Powerups.Remove(powerup.ID);
                    }

                    if (theWorld.Powerups.ContainsKey(powerup.ID))
                    {
                        theWorld.Powerups[powerup.ID] = powerup;
                    }
                    else
                    {
                        theWorld.Powerups.Add(powerup.ID, powerup);
                    }
                }
                return;
            }
        }

        /// <summary>
        /// Return the player tank with the playID
        /// </summary>
        /// <returns></returns>
        public Tank getPlayerTank()
        {
            lock (theWorld)
            {
                if (theWorld.Tanks.ContainsKey(playerID))
                    return theWorld.Tanks[playerID];
                return null;
            }
        }

        /// <summary>
        /// Return the world
        /// </summary>
        /// <returns></returns>
        public World GetWorld()
        {
            return theWorld;
        }

        /// <summary>
        /// The method to sent move request
        /// </summary>
        /// <param name="direction"></param>
        public void HandleMoveRequest(string direction)
        {
            if (direction == "up")
            {
                upKeyPressed = true;
                cc.MovingDirection = "up";
            }
            else if (direction == "left")
            {
                leftKeyPressed = true;
                cc.MovingDirection = "left";
            }
            else if (direction == "down")
            {
                downKeyPressed = true;
                cc.MovingDirection = "down";
            }
            else if (direction == "right")
            {
                rightKeyPressed = true;
                cc.MovingDirection = "right";
            }

        }

        /// <summary>
        /// the method to sent cancel move request when key up
        /// </summary>
        /// <param name="direction"></param>
        public void CancelMoveRequest(string direction)
        {
            if (direction == "up")
            {
                upKeyPressed = false;
                cc.MovingDirection = "none";
            }
            else if (direction == "left")
            {
                leftKeyPressed = false;
                cc.MovingDirection = "none";
            }
            else if (direction == "down")
            {
                downKeyPressed = false;
                cc.MovingDirection = "none";
            }
            else if (direction == "right")
            {
                rightKeyPressed = false;
                cc.MovingDirection = "none";
            }

            // To handle with two keys pressed event
            if (upKeyPressed == true)
                cc.MovingDirection = "up";
            else if (leftKeyPressed == true)
                cc.MovingDirection = "left";
            else if (downKeyPressed == true)
                cc.MovingDirection = "down";
            else if (rightKeyPressed == true)
                cc.MovingDirection = "right";
        }

        /// <summary>
        /// the method to send mouse down request 
        /// </summary>
        /// <param name="turretType"></param>
        public void HandleMouseRequest(string turretType)
        {
            if (turretType == "main")
                cc.FireType = "main";
            else if (turretType == "alt")
                cc.FireType = "alt";
        }

        /// <summary>
        /// the method to send mouse up request 
        /// </summary>
        public void CancelMouseRequest()
        {
            cc.FireType = "none";
        }

        /// <summary>
        /// the method to sent turret direction change Request
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void turretDirectionRequest(double x, double y)
        {
            Vector2D direction = new Vector2D(x - Constant.viewSize / 2, y - Constant.viewSize / 2);
            direction.Normalize();
            cc.turretDirection = new Vector2D(direction.GetX(), direction.GetY());
        }

        /// <summary>
        /// return the tank color with the input ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int getTankColor(int id)
        {
            try
            {
                return colorOfObjec[id];
            }
            catch (Exception)
            {
                return 8;
            }
        }
    }
}
