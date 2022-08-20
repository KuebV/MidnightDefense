using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MidnightDefense
{
    public class PlayerPositionData
    {
        public PlayerPositionData(Player player)
        {
            this.player = player;
            GameObject = player.GameObject;
            ReferenceHub = player.ReferenceHub;

            if (!player.IsAlive)
                LastPosition = Vector3.zero;

            Rigidbody = GameObject.GetComponent<Rigidbody>();
            Velocity = Rigidbody.velocity;
        }
        public Player player { get; }

        public Vector3 LastPosition { get; set; }

        public Vector3 Velocity { get; set; }

        public ReferenceHub ReferenceHub { get; }

        public GameObject GameObject { get; }

        public Rigidbody Rigidbody { get; }
    }
}
