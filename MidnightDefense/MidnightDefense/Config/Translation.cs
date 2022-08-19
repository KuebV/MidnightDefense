using Exiled.API.Interfaces;
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
        [Description("Friendly-Fire Protection Detection Message")]
        public string FFDetectedMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Friendly-Fire</color> despite it being disabled";

        [Description("SCP-049 Movement Detection Message")]
        public string SCP049DetectedMessage { get; set; } = "Player has been suspected for using <color=#2ea339>SCP-049 Movement Cheats</color>";

        [Description("Infinite Range Detection Message")]
        public string InfiniteRangeDetectionMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Infinite Range</color>";

        [Description("Speedhack Detection Message")]
        public string SpeedhackDetectionMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Speedhack</color>";

        [Description("Silent Aimbot Detection Message")]
        public string SilentAimbotDetectionMessage { get; set; } = "Player has been suspected for using <color=#2ea339>Silent Aimbot</color>";

        [Description("Alert Message")]
        public string PointThresholdMessage { get; set; } = "<color=#f0311f>%player% has been detected for cheating!</color>";

        [Description("Anti-Aimbot NPC Name")]
        public string SilentAimbotNPCName { get; set; } = "MD-AC";

    }
}
