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
    public class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        [JsonProperty(PropertyName = "died")]
        public bool IsDied { get; set; }

        public Powerup()
        {

        }

        public Powerup(Vector2D loc, int pId)
        {
            ID = pId;
            Location = loc;
        }
    }
}
