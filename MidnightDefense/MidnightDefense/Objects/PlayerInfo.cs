using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MidnightDefense.Objects
{
    public class PlayerInfo
    {
        public Player Player { get; set; }

        /// <summary>
        /// How many points does the player currently have for detection
        /// </summary>
        public int DetectionPoints { get; set; }

        /// <summary>
        /// Cheats that the player has been detected using
        /// </summary>
        public List<CheatsEnum> DetectedCheats { get; set; }

        /// <summary>
        /// How many times has the player hit the Silent Aimbot NPC
        /// </summary>
        public int SilentAimbotHitCounter { get; set; }

        /// <summary>
        /// Custom Position Data to detect Speedhack / Noclip
        /// </summary>
        public PlayerPositionData Position { get; set; }

        /// <summary>
        /// UNIX Timestamp of the last time the player shot their gun
        /// </summary>
        public long LastBulletFired { get; set; }

        /// <summary>
        /// Whether the player is being monitored for aimbot
        /// </summary>
        public bool MonitorForAimbot { get; set; }

        /// <summary>
        /// GameObject for Player
        /// </summary>
        public GameObject GameObject;

        /// <summary>
        /// Get the amount of times that staff / discord have been alerted of the player
        /// </summary>
        public int AlertCount { get; set; }


        /// <summary>
        /// How many times has the player been reported as cheating?
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        /// Get the players who have reported the targetted player
        /// </summary>
        public List<Player> ReporterPlayers { get; set; } = new List<Player>();
    }
}
