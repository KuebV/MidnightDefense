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
            else
                LastPosition = player.Position;
            

            Position = player.Position;
        }
        public Player player { get; }

        public Vector3 LastPosition { get; set; }

        public Vector3 Position
        {
            get => player.Position;
            set => ReferenceHub.playerMovementSync.OverridePosition(value);
        }

        public ReferenceHub ReferenceHub { get; }

        public GameObject GameObject { get; }
        public float Velocity { get => ReferenceHub.playerMovementSync.PlayerVelocity.magnitude; }

        public float DistanceBetweenPositions
        {
            get => (LastPosition - Position).MagnitudeIgnoreY();
        }

        public float MaxiumumSpeed { get; set; } = 0;
    }
}
