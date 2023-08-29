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
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "org")]
        public Vector2D Origin { get; set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D Direction{ get; set; }

        [JsonProperty(PropertyName = "owner")]
        public int BeamOwner { get; set; }

        public Beam()
        {

        }

        public Beam(int owner, Vector2D o, Vector2D d)
        {
            BeamOwner = owner;
            Origin = o;
            Direction = d;
        }    
    }
}
