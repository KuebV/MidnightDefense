using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MidnightDefense
{
    public class Plugin : Plugin<Config, Translation>
    {

        public override string Author => "KuebV";
        public override string Name => "MidnightDefense";
        public override Version Version => new Version(1, 1, 1);

        public static Dictionary<Player, string> PlayerLogs = new Dictionary<Player, string>();
        public static Dictionary<Player, int> SuspectedPlayers = new Dictionary<Player, int>();
        public static Dictionary<Player, AntiAimbotPlayer> MonitoringAimbot = new Dictionary<Player, AntiAimbotPlayer>();
        public static Dictionary<Player, long> CheckingSpeedhack = new Dictionary<Player, long>();
        public static Dictionary<Player, int> SilentAimbotHitCounter = new Dictionary<Player, int>();
        public static List<PlayerPositionData> PlayerPositions = new List<PlayerPositionData>();

        public static Plugin Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += EventHandlers.OnPlayerHurt;
            Exiled.Events.Handlers.Player.Shooting += EventHandlers.OnShooting;
            Exiled.Events.Handlers.Scp049.StartingRecall += EventHandlers.OnRecall;
            Exiled.Events.Handlers.Scp049.FinishingRecall += EventHandlers.OnRevive;
            Exiled.Events.Handlers.Player.Joined += EventHandlers.OnJoin;
            Exiled.Events.Handlers.Player.Left += EventHandlers.OnLeave;

            Instance = this;
            Timing.RunCoroutine(API.MonitorAimbotEnumerator());
            Timing.RunCoroutine(EventHandlers.CheckSuspectedPlayerPoints());
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Exiled.Events.Handlers.Player.Hurting -= EventHandlers.OnPlayerHurt;
            Exiled.Events.Handlers.Player.Shooting -= EventHandlers.OnShooting;
            Exiled.Events.Handlers.Scp049.StartingRecall -= EventHandlers.OnRecall;
            Exiled.Events.Handlers.Scp049.FinishingRecall -= EventHandlers.OnRevive;
            Exiled.Events.Handlers.Player.Joined -= EventHandlers.OnJoin;
            Exiled.Events.Handlers.Player.Left -= EventHandlers.OnLeave;

            base.OnDisabled();
        }
    }
}
