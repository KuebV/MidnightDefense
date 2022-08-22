using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MidnightDefense
{
    public class Config : IConfig
    {
        [Description("Whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should Friendly-Fire Detection be enabled")]
        public bool FFDetection { get; set; } = true;

        [Description("Disable Friendly-Fire Damage")]
        public bool NegateFFDamage { get; set; } = true;

        [Description("Points given when friendly-fire event is triggered")]
        public int FFDetectionPoints { get; set; } = 1;

        [Description("Should SCP-049 Movement Detection be enabled")]
        public bool SCP049Detection { get; set; } = true;

        [Description("Points given when SCP-049 Movement event is triggered")]
        public int SCP049DetectedMovementPoints { get; set; } = 1;

        [Description("Infinite Range Detection")]
        public bool InfiniteRangeDetection { get; set; } = true;

        [Description("Distance that SCP's are allowed to attack players")]
        public float RangeDistance { get; set; } = 5f;

        [Description("Should negate all damage if a player is being attacked from a range that is set outside of the range distance")]
        public bool NegateInfiniteRangeDamage { get; set; } = true;

        [Description("Points given when infinite range event is triggered")]
        public int InfiniteRangeDetectionPoints { get; set; } = 1;

        [Description("Should Silent Aimbot Detection be enabled? (Places Dummy Player near the suspected players crosshair and teleports around every so often to detect if they are aimbotting)")]
        public bool SilentAimbotDetection { get; set; } = true;

        [Description("Points given when suspected player triggers the anti-aimbot")]
        public int SilentAimbotDetectionPoints { get; set; } = 1;

        [Description("Size of Anti-Aimbot Player")]
        public float[] SilentAimbotPlayerSize { get; set; } = { 1f, 1f, 1f };

        [Description("If anyone shoots the anti-aimbot player, it will automatically kick them (Default : False)")]
        public bool DisconnectOnAntiAimbotHit { get; set; } = false;

        [Description("How many hits to the anti-aimbot player it takes until the cheat is detected")]
        public int SilentAimbotHitThreshold { get; set; } = 8;

        [Description("How often should the Anti-Aimbot NPC teleport?")]
        public float SilentAimbotTeleportTime { get; set; } = 0.15f;

        [Description("How many cheater reports does it take for the Anti-Aimbot to trigger on the suspected player")]
        public int SilentAimbotTriggerReportAmount { get; set; } = 3;

        [Description("Speedhack Detection")]
        public bool SpeedhackDetection { get; set; } = true;

        [Description("Points given when suspected player triggers Speedhack")]
        public int SpeedhackDetectionPoints { get; set; } = 1;

        [Description("Speedhack Millisecond Threshold (DO NOT GO OVER 80)")]
        public int SpeedhackDetectionThreshold { get; set; } = 75;

        [Description("If Speedhack is detected, should the current action be cancelled?")]
        public bool SpeedhackDetectionCancelEvent { get; set; } = false;

        [Description("Noclip Detection")]
        public bool NoclipDetection { get; set; } = true;

        [Description("Points given when suspected player attempts to NoClip")]
        public int NoclipDetectionPoints { get; set; } = 1;

        [Description("Rubberband to last recorded position when suspected player attempts to noclip")]
        public bool NoclipRubberband { get; set; } = true;

        [Description("Maximum Velocity before Noclip is detected (Don't change this to anything lower than the default!)")]
        public float NoclipMaximumVelocity { get; set; } = 7.571895f;

        [Description("How often does the plugin check the player velocity for noclip?")]
        public float NoClipDetectionSpeed { get; set; } = 0.5f;

        [Description("Should NoClip Detection activate even on staff?")]
        public bool NoClipDetectionStaff { get; set; } = false;

        [Description("What roles should be allowed to use NoClip even if they aren't staff")]
        public RoleType[] NoClipAllowedRoles { get; set; } = new RoleType[] { RoleType.Tutorial };

        [Description("Alert Threshold (Points until any online staff are notified)")]
        public int PointThreshold { get; set; } = 5;

        [Description("When a player reaches a certain amount of points, should staff be alerted?")]
        public bool AlertOnlineStaff { get; set; } = true;

        [Description("Maximum times an alert will notify staff / discord about the cheater")]
        public int AlertMaxTimes { get; set; } = 3;

        [Description("How often does the plugin check if anyone has reached the point threshold to alert staff")]
        public float AlertTimeframe { get; set; } = 10f;

        [Description("Enable Discord Webhook Alerts")]
        public bool DiscordWebhookEnabled { get; set; } = false;

        [Description("Discord Webhook Username")]
        public string DiscordWebhookUsername { get; set; } = "Midnight Defense Alerts";

        [Description("Discord Webhook URL")]
        public string DiscordWebhookURL { get; set; } = "discordURLhere";

        
    }
}
