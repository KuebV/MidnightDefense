using Exiled.API.Features;
using MEC;
using MidnightDefense.Objects;
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
        public override Version Version => new Version(1, 3, 0);

        public static Plugin Instance;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Hurting += EventHandlers.OnPlayerHurt;
            Exiled.Events.Handlers.Player.Shooting += EventHandlers.OnShooting;
            Exiled.Events.Handlers.Scp049.StartingRecall += EventHandlers.OnRecall;
            Exiled.Events.Handlers.Scp049.FinishingRecall += EventHandlers.OnRevive;
            Exiled.Events.Handlers.Player.Left += EventHandlers.OnLeave;
            Exiled.Events.Handlers.Player.Dying += EventHandlers.BeforePlayerDeath;
            Exiled.Events.Handlers.Player.Shot += EventHandlers.OnShot;
            Exiled.Events.Handlers.Player.ChangingRole += EventHandlers.ChangeClass;

            Instance = this;
            
            if (Instance.Config.SilentAimbotDetection)
                Timing.RunCoroutine(API.MonitorAimbotEnumerator());

            if (Instance.Config.NoclipDetection)
                Timing.RunCoroutine(API.MonitorPlayerSpeed());

            Timing.RunCoroutine(EventHandlers.CheckSuspectedPlayerPoints());

            _ = Webhook.RawSendWebhook(Translation.DiscordEnableMessage.Replace("%version%", Version.ToString()));

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Exiled.Events.Handlers.Player.Hurting -= EventHandlers.OnPlayerHurt;
            Exiled.Events.Handlers.Player.Shooting -= EventHandlers.OnShooting;
            Exiled.Events.Handlers.Scp049.StartingRecall -= EventHandlers.OnRecall;
            Exiled.Events.Handlers.Scp049.FinishingRecall -= EventHandlers.OnRevive;
            Exiled.Events.Handlers.Player.Left -= EventHandlers.OnLeave;
            Exiled.Events.Handlers.Player.Dying -= EventHandlers.BeforePlayerDeath;
            Exiled.Events.Handlers.Player.Shot -= EventHandlers.OnShot;
            Exiled.Events.Handlers.Player.ChangingRole -= EventHandlers.ChangeClass;

            base.OnDisabled();
        }

        /// <summary>
        /// Glorified String Builder for MDAC Player Command
        /// </summary>
        public static Dictionary<Player, string> PlayerLogs = new Dictionary<Player, string>();

        public static List<PlayerInfo> PlayerInfo = new List<PlayerInfo>();
    }

}
