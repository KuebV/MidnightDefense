using Exiled.API.Interfaces;
using MidnightDefense.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightDefense
{
    public class Translation : ITranslation
    {
        [Description("Friendly-Fire Detection Message")]
        public string FFDetectedMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Friendly-Fire</color> despite it being disabled";

        [Description("SCP-049 Movement Detection Message")]
        public string SCP049DetectedMessage { get; set; } = "Player has been suspected for using <color=#2ea339>SCP-049 Movement Cheats</color>";

        [Description("Infinite Range Detection Message")]
        public string InfiniteRangeDetectionMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Infinite Range</color>";

        [Description("Speedhack Detection Message")]
        public string SpeedhackDetectionMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Speedhack</color>";

        [Description("Silent Aimbot Detection Message")]
        public string SilentAimbotDetectionMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Silent Aimbot</color>";

        [Description("Noclip Detection Message")]
        public string NoclipDetectionMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Noclip</color>";

        [Description("Alert Message")]
        public string PointThresholdMessage { get; set; } = "<color=#f0311f>%player% has been detected for cheating!</color>";

        [Description("Discord Alert Message")]
        public string DiscordAlertMessage { get; set; } = "%player% has been detected for cheating! Detected Cheats [%cheats%]";

        [Description("When the plugin is enabled, the webhook will send this message")]
        public string DiscordEnableMessage { get; set; } = "Midnight Defense (Version: %version%) will send alerts to this channel!";

        [Description("Anti-Aimbot NPC Name")]
        public string SilentAimbotNPCName { get; set; } = "MD-AC";


    }
}
