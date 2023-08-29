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
    public class Projectile
    {
        static int nextId = 0;
        [JsonProperty(PropertyName = "proj")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        public Vector2D Direction { get; }


        [JsonProperty(PropertyName = "dir")]
        public Vector2D Orientation { get; set; }

        [JsonProperty(PropertyName = "died")]
        public bool IsDied { get; set; } = false;

        [JsonProperty(PropertyName = "owner")]
        public int Owner { get; set; }

        public int MaxReboundTimes;
        public Projectile()
        {
            ID = nextId;
            nextId++;
        }

        public Projectile(int own, Vector2D location, Vector2D direction)
        {
            Owner = own;
            Location = location;
            Orientation = direction;
            ID = nextId;
            nextId++;
        }
    }
}
