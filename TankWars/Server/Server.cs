// Author: Yanzheng Wu and Qingwen Bao
// University of Utah
// Date: 2021/04/23
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using TankWars;

namespace TankWars
{
    /// <summary>
    /// The game server for receiving text messages from multiple clients
    /// and send the messages to these clients
    /// </summary>
    class Server
    {
        //input setting parameters
        private static int UniverseSize;
        private static int MSPerFrame;
        private static int ProjectileSpeed;
        private static double EngineStrength;
        private static int MaxPowerups;
        private static int MaxPowerupDelay;
        private static int TankSize;
        private static int WallSize;
        private static int RespawnRate;
        private static int FiringDelay;
        // A map of clients that are connected, each with an ID
        private static HashSet<SocketState> clients;
        private static long FrameCount = 0; // keep track of current frame
        private static int PowerupCount = 0;
        private static World theWorld;
        private static bool AdditionalGameMode; // to control whether additional mode open
        private static int MaxReboundTimes;
        public static Dictionary<int, ControlCommand> commands;

        /// <summary>
        /// begin the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            commands = new Dictionary<int, ControlCommand>();
            clients = new HashSet<SocketState>();
            theWorld = new World();
            ReadSettings(@"..\\..\\..\\..\\Resources\settings.xml");
            Server server = new Server();
            // start the loop to connect clients
            server.StartServer();
            Console.WriteLine("server start");

            Thread main = new Thread(UpdateFrame);
            main.Start();
        }

        /// <summary>
        /// Reading from the setting file
        /// </summary>
        /// <param name="path"></param>
        private static void ReadSettings(string path)
        {
            try
            {
                using (XmlReader r = XmlReader.Create(path))
                {
                    while (r.Read())
                    {
                        if (r.IsStartElement())
                        {
                            switch (r.Name)
                            {
                                case "UniverseSize":
                                    r.Read();
                                    UniverseSize = int.Parse(r.Value);
                                    break;

                                case "MSPerFrame":
                                    r.Read();
                                    MSPerFrame = int.Parse(r.Value);
                                    break;

                                case "RespawnRate":
                                    r.Read();
                                    RespawnRate = int.Parse(r.Value);
                                    break;

                                case "ProjectileSpeed":
                                    r.Read();
                                    ProjectileSpeed = int.Parse(r.Value);
                                    break;

                                case "EngineStrength":
                                    r.Read();
                                    EngineStrength = double.Parse(r.Value);
                                    break;

                                case "MaxPowerups":
                                    r.Read();
                                    MaxPowerups = int.Parse(r.Value);
                                    break;

                                case "MaxPowerupDelay":
                                    r.Read();
                                    MaxPowerupDelay = int.Parse(r.Value);
                                    break;

                                case "TankSize":
                                    r.Read();
                                    TankSize = int.Parse(r.Value);
                                    break;

                                case "WallSize":
                                    r.Read();
                                    WallSize = int.Parse(r.Value);
                                    break;

                                case "FiringDelay":
                                    r.Read();
                                    FiringDelay = int.Parse(r.Value);
                                    break;

                                case "AdditionalGameMode":
                                    r.Read();
                                    if (r.Value == "true")
                                        AdditionalGameMode = true;
                                    else
                                        AdditionalGameMode = false;
                                    break;

                                case "MaxReboundTimes":
                                    r.Read();
                                    MaxReboundTimes = int.Parse(r.Value);
                                    break;

                                case "Wall":
                                    //read end point
                                    r.ReadToFollowing("x");
                                    r.Read();
                                    double endpointX = double.Parse(r.Value);
                                    r.ReadToFollowing("y");
                                    r.Read();
                                    double endpointY = double.Parse(r.Value);
                                    r.ReadToFollowing("x");
                                    r.Read();
                                    double endpointX2 = double.Parse(r.Value);
                                    r.ReadToFollowing("y");
                                    r.Read();
                                    double endpointY2 = double.Parse(r.Value);
                                    //update walls
                                    Wall w = new Wall(new Vector2D(endpointX, endpointY), new Vector2D(endpointX2, endpointY2));
                                    theWorld.WallUpdate(w);
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Something was woring when read xml setting file");
            }
        }
        /// <summary>
        /// Update world data in each frame
        /// </summary>
        /// <param name="obj"></param>
        private static void UpdateFrame(object obj)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (true)
            {
                while (sw.ElapsedMilliseconds < MSPerFrame)
                {
                    //do nothing
                }
                sw.Restart();
                
                lock (theWorld)
                {
                    UpdateWorld();
                }
                SendToClients();
            }
        }
        /// <summary>
        /// This method is to send world data to all clients
        /// </summary>
        private static void SendToClients()
        {
            lock (theWorld)
            {
                HashSet<SocketState> disconnected = new HashSet<SocketState>();
                //if the client is disconnect, remove it.
                foreach (SocketState ss in clients)
                {
                    if (!ss.TheSocket.Connected)
                    {
                        disconnected.Add(ss);
                        Console.WriteLine(ss.ID + " disconnected");
                    }
                }
                foreach (SocketState ss in disconnected)
                {
                    clients.Remove(ss);
                    theWorld.Tanks[(int)ss.ID].Disconnected = true;
                    theWorld.Tanks[(int)ss.ID].HitPoints = 0;
                    theWorld.Tanks[(int)ss.ID].Died = true;
                }
                //send the world information to clients
                foreach (SocketState ss in clients)
                {
                    StringBuilder message = new StringBuilder();

                    foreach (Tank t in theWorld.Tanks.Values)
                    {
                        message.Append(JsonConvert.SerializeObject(t) + "\n");
                    }

                    foreach (Projectile p in theWorld.Projectiles.Values)
                        message.Append(JsonConvert.SerializeObject(p) + "\n");

                    foreach (Beam b in theWorld.Beams.Values)
                        message.Append(JsonConvert.SerializeObject(b) + "\n");

                    foreach (Powerup pu in theWorld.Powerups.Values)
                        message.Append(JsonConvert.SerializeObject(pu) + "\n");

                    Networking.Send(ss.TheSocket, message.ToString());
                }
                //update beams status
                theWorld.Beams = new Dictionary<int, Beam>();
                //update the tank status
                foreach (Tank t in theWorld.Tanks.Values)
                {
                    if (t.Died)
                        t.Died = false;
                    if (t.Disconnected)
                        theWorld.Tanks.Remove(t.ID);
                }
            }
        }
        /// <summary>
        /// Initialized the server's state
        /// </summary>
        public Server()
        {
            clients = new HashSet<SocketState>();
        }

        /// <summary>
        /// Start accepting Tcp sockets connections from clients
        /// </summary>
        public void StartServer()
        {
            // This begins an "event loop"
            Networking.StartServer(NewClientConnected, 11000);
        }

        /// <summary>
        /// Method to be invoked by the networking library
        /// </summary>
        /// <param name="state">The SocketState representing the new client</param>
        private void NewClientConnected(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Console.WriteLine(state.ErrorMessage);
                return;
            }
            // change the state's network action to the 
            // receive player data
            state.OnNetworkAction = ReceivePlayerData;

            Networking.GetData(state);
        }
        /// <summary>
        /// Method to be invoked by the networking library
        /// </summary>
        /// <param name="state"></param>
        private void ReceivePlayerData(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Console.WriteLine(state.ErrorMessage);
                return;
            }
            string playerName = state.GetData().Trim('\n');
            int playID = (int)state.ID;
            state.RemoveData(0, playerName.Length);
            
            Tank tank = new Tank(playerName, playID, CreateRandomLocation());
            RespawnTank(tank, CreateRandomLocation());
            // finding an unoccupied part of the world to add the new tank (where it isn't colliding with anything)
            while (CheckTankSpawnCollision(tank) == true)
            {
                tank.Location = CreateRandomLocation();
            }
            Console.WriteLine("Player(" + tank.ID + ") " + "\"" + tank.Name + "\" joined");

            // change the state's network action to receive command data from the client
            state.OnNetworkAction = ReceiveCommandData;
            // send tank ID and worldsize
            Networking.Send(state.TheSocket, tank.ID.ToString() + "\n" + UniverseSize.ToString() + "\n");

            lock (theWorld)
            {
                // update the new tank
                clients.Add(state);
                theWorld.Tanks.Add(tank.ID, tank);
                theWorld.ControlCommands.Add(tank.ID, new ControlCommand());
            }
            lock (theWorld)
            {
                // the StringBuilder contains all serialized wall data, send it to the client
                StringBuilder wallData = new StringBuilder();
                foreach (Wall wall in theWorld.Walls.Values)
                {
                    wallData.Append(JsonConvert.SerializeObject(wall) + "\n");
                }
                Networking.Send(state.TheSocket, wallData.ToString());
            }
            Networking.GetData(state);
        }
        /// <summary>
        /// Method to check new tank spawning location collisions
        /// </summary>
        /// <param name="tank"></param>
        /// <returns></returns>
        private static bool CheckTankSpawnCollision(Tank tank)
        {
            // check this tank collides with walls
            foreach (Wall wall in theWorld.Walls.Values)
            {
                if (CheckTankWallCollision(tank, wall))
                {
                    return true;
                }
            }
            // check this tank collides with projectiles
            foreach (Projectile projectile in theWorld.Projectiles.Values)
            {
                if (CheckTankProjectileCollision(tank, projectile))
                {
                    return true;
                }
            }
            // check this tank collides with beams
            foreach (Beam beam in theWorld.Beams.Values)
            {
                if (CheckTankBeamCollision(tank, beam))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// This method checks collision between tank and wall
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="wall"></param>
        /// <returns></returns>
        private static bool CheckTankWallCollision(Tank tank, Wall wall)
        {
            double maxXOfExtendedWall = Math.Max(wall.Endpoint1.GetX(), wall.Endpoint2.GetX()) + WallSize / 2 + TankSize / 2;
            double minXOfExtendedWall = Math.Min(wall.Endpoint1.GetX(), wall.Endpoint2.GetX()) - WallSize / 2 - TankSize / 2;
            double maxYOfExtendedWall = Math.Max(wall.Endpoint1.GetY(), wall.Endpoint2.GetY()) + WallSize / 2 + TankSize / 2;
            double minYOfExtendedWall = Math.Min(wall.Endpoint1.GetY(), wall.Endpoint2.GetY()) - WallSize / 2 - TankSize / 2;
            return (tank.Location.GetX() >= minXOfExtendedWall && tank.Location.GetX() <= maxXOfExtendedWall && tank.Location.GetY() >= minYOfExtendedWall && tank.Location.GetY() <= maxYOfExtendedWall);
        }
        /// <summary>
        /// This method checks collision between tank and projectile
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="projectile"></param>
        /// <returns></returns>
        private static bool CheckTankProjectileCollision(Tank tank, Projectile projectile)
        {
            return (tank.Location - projectile.Location).Length() < TankSize / 2;
        }

        /// <summary>
        /// This method checks collision between tank and beam
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        private static bool CheckTankBeamCollision(Tank tank, Beam beam)
        {
            return Intersects(beam.Origin, beam.Direction, tank.Location, TankSize / 2);
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

        /// <summary>
        /// This method is to create the random location within the universe size
        /// </summary>
        /// <returns></returns>
        private static Vector2D CreateRandomLocation()
        {
            Random random = new Random();
            int xCoordinate = random.Next(-UniverseSize / 2, UniverseSize / 2);
            int yCoordinate = random.Next(-UniverseSize / 2, UniverseSize / 2);
            return new Vector2D(xCoordinate, yCoordinate);
        }

        /// <summary>
        /// Method to be invoked by the networking library
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveCommandData(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Console.WriteLine(state.ErrorMessage);
                return;
            }
            ProcessMessage(state);
            Networking.GetData(state);
        }

        /// <summary>
        /// Given the data that has arrived so far, 
        /// potentially from multiple receive operations, 
        /// determine if we have enough to make a complete message,
        /// and process it.
        /// </summary>
        /// <param name="sender">The SocketState that represents the client</param>
        private void ProcessMessage(SocketState state)
        {
            string totalData = state.GetData();

            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            string receivedCommand = null;

            // Loop until we have processed all messages.
            // We may have received more than one.
            foreach (string p in parts)
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;

                receivedCommand = p;
                // Remove it from the SocketState's growable buffer
                state.RemoveData(0, p.Length);
            }
            lock (theWorld)
            {
                if (receivedCommand != null)
                    ProcessCommand(state, receivedCommand);
            }
        }

        /// <summary>
        /// To process command data from the client
        /// </summary>
        /// <param name="state"></param>
        /// <param name="receivedCommand"></param>
        private void ProcessCommand(SocketState state, string receivedCommand)
        {
            JObject obj = JObject.Parse(receivedCommand);

            // This is the case that is a moving command.
            JToken token = obj["moving"];
            if (token != null && theWorld.Tanks[(int)state.ID].RespawnTime == -1)
            {
                ControlCommand cc = JsonConvert.DeserializeObject<ControlCommand>(receivedCommand);
                theWorld.ControlCommands[(int)state.ID] = cc;
            }
        }

        /// <summary>
        /// This method is to update the world data each time
        /// </summary>
        private static void UpdateWorld()
        {
            lock (theWorld)
            {
                // update all tanks
                foreach (Tank tank in theWorld.Tanks.Values)
                {
                    Vector2D preLocation = tank.Location;

                    if (tank.RespawnTime > 0)
                        tank.RespawnTime--;

                    switch (theWorld.ControlCommands[tank.ID].MovingDirection)
                    {
                        case "up":
                            tank.Orientation = new Vector2D(0, -1);
                            break;
                        case "left":
                            tank.Orientation = new Vector2D(-1, 0);
                            break;
                        case "down":
                            tank.Orientation = new Vector2D(0, 1);
                            break;
                        case "right":
                            tank.Orientation = new Vector2D(1, 0);
                            break;
                        case "none":
                            tank.Orientation = new Vector2D(0, 0);
                            break;
                    }
                    tank.Velocity = tank.Orientation * EngineStrength;
                    tank.Location += tank.Velocity;
                    // update turret direction
                    theWorld.Tanks[tank.ID].Aiming = theWorld.ControlCommands[tank.ID].turretDirection;

                    // check collision between tank and wall
                    foreach (Wall wall in theWorld.Walls.Values)
                    {
                        if (CheckTankWallCollision(tank, wall))
                        {
                            tank.Location = preLocation;
                        }
                    }

                    // When tanks reach one edge of the world they should be "teleported" to the opposite edge
                    WrapAround(tank);

                    //update respaw
                    if (tank.RespawnTime == 0)
                    {
                        RespawnTank(tank, CreateRandomLocation());
                        while (CheckTankSpawnCollision(tank) == true)
                        {
                            // find the respawn tank location with no collisions with others
                            tank.Location = CreateRandomLocation();
                        }
                    }

                    // update firing Delay
                    if (tank.ReloadingTime > 0)
                        tank.ReloadingTime--;
                    // updata the fire status
                    switch (theWorld.ControlCommands[tank.ID].FireType)
                    {
                        //if the fire type is projectile
                        case "main":
                            if (tank.ReloadingTime <= 0 && tank.RespawnTime == -1)
                            {
                                Projectile p = new Projectile(tank.ID, tank.Location, tank.Aiming);
                                p.MaxReboundTimes = MaxReboundTimes;
                                theWorld.ProjectileUpdate(p);
                                theWorld.Tanks[tank.ID].ReloadingTime = FiringDelay;
                            }
                            break;
                        //if the fire type is beam
                        case "alt":
                            if (tank.PowerUpCollect > 0)
                            {
                                Beam b = new Beam(tank.ID, tank.Location, tank.Aiming);
                                tank.PowerUpCollect--;
                                theWorld.BeamUpdate(b);
                                foreach (Tank t in theWorld.Tanks.Values)
                                {
                                    if (CheckTankBeamCollision(t, b) == true)
                                    {
                                        TankDead(t);
                                        theWorld.ControlCommands[t.ID].MovingDirection = "none";
                                        theWorld.Tanks[b.BeamOwner].Score++;
                                    }
                                }
                            }
                            break;
                    }
                }
                // update all projectiles
                foreach (Projectile projectile in theWorld.Projectiles.Values)
                {
                    if (projectile.IsDied)
                    {
                        theWorld.RemoveProjectile(projectile.ID);
                        continue;
                    }
                    projectile.Orientation.Normalize();
                    projectile.Location += projectile.Orientation * ProjectileSpeed;
                    // check collisions between this projectile with all tanks and walls
                    if (CheckProjectileCollisions(projectile) == true)
                    {
                        theWorld.Projectiles[projectile.ID].IsDied = true;
                    }
                }
                // create powerups
                if (FrameCount < MaxPowerupDelay)
                {
                    FrameCount++;
                }
                else if (FrameCount >= MaxPowerupDelay && theWorld.Powerups.Count < MaxPowerups)
                {
                    Powerup powerup = new Powerup(CreateRandomLocation(), PowerupCount);
                    while (CheckPowerupCollisions(powerup) == true)
                    {
                        powerup.Location = CreateRandomLocation();
                    }
                    theWorld.Powerups.Add(powerup.ID, powerup);
                    PowerupCount++;
                    FrameCount = 0;
                }
                // update powerups
                foreach (Powerup powerup in theWorld.Powerups.Values)
                {
                    if (powerup.IsDied)
                    {
                        theWorld.Powerups.Remove(powerup.ID);
                        continue;
                    }
                    foreach (Tank tank in theWorld.Tanks.Values)
                    {
                        if (CheckPowerupTankCollision(powerup, tank) == true)
                        {
                            theWorld.Powerups[powerup.ID].IsDied = true;
                            theWorld.Tanks[tank.ID].PowerUpCollect++;
                            // After spawning a powerup, the server should pick a random number of frames less than this
                            // number before trying to spawn another.
                            Random random = new Random();
                            MaxPowerupDelay = random.Next(1, MaxPowerupDelay);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The helper method to update the dead tank
        /// </summary>
        /// <param name="t"></param>
        private static void TankDead(Tank t)
        {
            if (t.RespawnTime == -1)
            {
                t.HitPoints = 0;
                t.Died = true;
                t.RespawnTime = RespawnRate;
            }
        }

        /// <summary>
        /// The helper method to update the respawn tank
        /// </summary>
        /// <param name="t"></param>
        /// <param name="location"></param>
        private static void RespawnTank(Tank t, Vector2D location)
        {
            t.Died = false;
            t.Location = location;
            t.HitPoints = 3;
            t.ReloadingTime = 0;
            t.RespawnTime = -1;
            t.PowerUpCollect = 0;
        }

        /// <summary>
        /// This helper method is to check collisions between the powerup with all walls and tanks
        /// </summary>
        /// <param name="powerup"></param>
        /// <returns></returns>
        private static bool CheckPowerupCollisions(Powerup powerup)
        {
            // check collision between powerup and walls
            foreach (Wall wall in theWorld.Walls.Values)
            {
                if (CheckPowerupWallCollision(powerup, wall) == true)
                {
                    return true;
                }
            }

            // check collision between powerup and tanks
            foreach (Tank tank in theWorld.Tanks.Values)
            {
                if (CheckPowerupTankCollision(powerup, tank) == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This helper method is to check collisions between the powerup and all tanks
        /// </summary>
        /// <param name="powerup"></param>
        /// <param name="tank"></param>
        /// <returns></returns>
        private static bool CheckPowerupTankCollision(Powerup powerup, Tank tank)
        {
            return (powerup.Location - tank.Location).Length() < TankSize / 2;
        }

        /// <summary>
        /// This helper method is to check collisions between the powerup and all walls
        /// </summary>
        /// <param name="powerup"></param>
        /// <param name="wall"></param>
        /// <returns></returns>
        private static bool CheckPowerupWallCollision(Powerup powerup, Wall wall)
        {
            double maxXOfWall = Math.Max(wall.Endpoint1.GetX(), wall.Endpoint2.GetX());
            double minXOfWall = Math.Min(wall.Endpoint1.GetX(), wall.Endpoint2.GetX());
            double maxYOfWall = Math.Max(wall.Endpoint1.GetY(), wall.Endpoint2.GetY());
            double minYOfWall = Math.Min(wall.Endpoint1.GetY(), wall.Endpoint2.GetY());
            return (powerup.Location.GetX() >= minXOfWall - WallSize / 2 && powerup.Location.GetX() <= maxXOfWall + WallSize / 2) && (powerup.Location.GetY() >= minYOfWall - WallSize / 2 && powerup.Location.GetY() <= maxYOfWall + WallSize / 2);
        }

        /// <summary>
        /// This helper method is to check all projectile collisions
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        private static bool CheckProjectileCollisions(Projectile projectile)
        {
            // check whether the projectile goes outside of the world
            if ((projectile.Location.GetX() > UniverseSize / 2) || (projectile.Location.GetX() < -UniverseSize / 2) || (projectile.Location.GetY() > UniverseSize / 2) || (projectile.Location.GetY() < -UniverseSize / 2))
            {
                return true;
            }

            // check the collisions between tanks
            foreach (Tank tank in theWorld.Tanks.Values)
            {
                if (CheckTankProjectileCollision(tank, projectile) == true && tank.ID != projectile.Owner)
                {
                    if (tank.HitPoints > 0)
                    {
                        tank.HitPoints--;
                        projectile.IsDied = true;
                    }
                    // if the tank is a dead tank, return false
                    if (tank.HitPoints == 0)
                    {
                        TankDead(tank);
                        theWorld.ControlCommands[tank.ID].MovingDirection = "none";
                        theWorld.Tanks[projectile.Owner].Score++;
                        return false;
                    }
                }
            }

            // check collisions between walls
            foreach (Wall wall in theWorld.Walls.Values)
            {
                if (CheckProjectileWallCollision(projectile, wall) == true)
                {
                    // When the additional mode opens, the projectile can reflect from the wall in random degrees
                    if (AdditionalGameMode && projectile.MaxReboundTimes > 0)
                    {
                        float angle = projectile.Orientation.ToAngle();
                        Random r = new Random();
                        projectile.Orientation.Rotate(r.Next(0, 90));
                        projectile.MaxReboundTimes--;
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The helper method is to check collision between the projectile and all walls
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static bool CheckProjectileWallCollision(Projectile projectile, Wall wall)
        {
            double maxXOfWall = Math.Max(wall.Endpoint1.GetX(), wall.Endpoint2.GetX());
            double minXOfWall = Math.Min(wall.Endpoint1.GetX(), wall.Endpoint2.GetX());
            double maxYOfWall = Math.Max(wall.Endpoint1.GetY(), wall.Endpoint2.GetY());
            double minYOfWall = Math.Min(wall.Endpoint1.GetY(), wall.Endpoint2.GetY());
            return (projectile.Location.GetX() > minXOfWall - WallSize / 2 && projectile.Location.GetX() < maxXOfWall + WallSize / 2 && projectile.Location.GetY() > minYOfWall - WallSize / 2 && projectile.Location.GetY() < maxYOfWall + WallSize / 2);
        }

        /// <summary>
        /// When tanks reach one edge of the world they should be "teleported" to the opposite edge
        /// </summary>
        /// <param name="tank"></param>
        private static void WrapAround(Tank tank)
        {
            if (Math.Abs(tank.Location.GetX()) + Constant.tankSize / 2 > UniverseSize / 2)
                theWorld.Setlocation(tank, new Vector2D(Math.Sign(tank.Location.GetX()) * (-UniverseSize / 2 + Constant.tankSize / 2), tank.Location.GetY()));
            if (Math.Abs(tank.Location.GetY()) + Constant.tankSize / 2 > UniverseSize / 2)
                theWorld.Setlocation(tank, new Vector2D(tank.Location.GetX(), Math.Sign(tank.Location.GetY()) * (-UniverseSize / 2 + Constant.tankSize / 2)));
        }
    }
}