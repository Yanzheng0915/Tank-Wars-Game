//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;
using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        static int nextId = 0;
        [JsonProperty(PropertyName = "wall")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "p1")]
        public Vector2D Endpoint1 { get; set; }

        [JsonProperty(PropertyName = "p2")]
        public Vector2D Endpoint2 { get; set; }

        public Wall()
        {
            ID = nextId;
            nextId++;
        }

        public Wall(Vector2D ep1, Vector2D ep2)
        {
            Endpoint1 = ep1;
            Endpoint2 = ep2;
            ID = nextId;
            nextId++;
        }
    }
}
