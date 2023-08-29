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
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        public int ID{ get; set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        [JsonProperty(PropertyName = "bdir")]
        public Vector2D Orientation { get; set; }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D Aiming { get; set; } = new Vector2D(0, -1);

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "hp")]
        public int HitPoints { get; set; } = 3;

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; } = 0;

        [JsonProperty(PropertyName = "died")]
        public bool Died { get; set; } = false;

        [JsonProperty(PropertyName = "dc")]
        public bool Disconnected { get; set; } = false;

        [JsonProperty(PropertyName = "join")]
        public bool Joined { get; set; } = false;

        public int ColorId { get; set; } = 1;

        public int PowerUpCollect { get; set; } = 0;

        public Vector2D Velocity { get; set; }

        public int ReloadingTime { get; set; }
        public int RespawnTime { get; set; }
        public Tank(string name, int id)
        {
            this.Name = name;
            ID = id;
        }

        public Tank(string name, int id, Vector2D randomLocation)
        {
            this.Name = name;
            ID = id;
            this.Location = randomLocation;
        }

        public Tank()
        {
        }
    }
}

