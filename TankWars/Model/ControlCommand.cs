//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {
        [JsonProperty(PropertyName = "moving")]
        public string MovingDirection { get; set; } = "none";

        [JsonProperty(PropertyName = "fire")]
        public string FireType { get; set; } = "none";

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D turretDirection = new Vector2D(-1, -1);

        public ControlCommand()
        {

        }

        public ControlCommand(string direction, string fire, Vector2D aimingDirection)
        {
            MovingDirection = direction;
            FireType = fire;
            turretDirection = aimingDirection;
        }
    }
}
