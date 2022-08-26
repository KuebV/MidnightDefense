using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using MidnightDefense.Objects;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MidnightDefense.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class MDAC : ICommand
    {
        public string Command => "midnightdefenseanticheat";

        public string[] Aliases => new string[] { "mdac" };

        public string Description => "Monitor, Analyize, and Configure the Midnight Defense Anti-Cheat";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("md.general")) 
            {
                response = "Invalid Permissions to use this command!";
                return false;
            }

            if (arguments.Count < 1)
            {
                string sb = "\n\n";
                sb += "<color=#edba42>[Midnight Defense]</color>\n";
                sb += "<color=#9c519c>Specialized in detecting and preventing Midnight Client cheaters</color>\n";
                sb += "<color=#bfbfbf>------------HELP MENU------------\n";
                sb += "<color=#eb4034>Usage : mdac (Subcommand) (Extra Arguments)</color>\n";
                sb += "<color=#eb4034><u>[Subcommands]</u></color>\n";
                sb += "<color=#407abd>config </color><color=#bfbfbf>\t\t\tView the current confiugration for Midnight Defense</color>\n";
                sb += "<color=#407abd>player (Player Name / ID)</color><color=#bfbfbf>\tView the anti-cheat log for that specific player</color>\n";
                sb += "<color=#407abd>monitor (Player Name / ID)</color><color=#bfbfbf>\tManually enable Anti-Aimbot feature for that player</color>\n";
                sb += "<color=#407abd>dismiss (Player Name / ID)</color><color=#bfbfbf>\tDismiss players actions, resetting their points</color>\n";

                response = sb;
                return true;
            }

            Config config = Plugin.Instance.Config;


            string helpString = "Usage: mdac (player / config / monitor / info)";
            switch (arguments.At(0).ToLower())
            {
                #region ANTI-CHEAT INFO 
                case "info":
                    string sb = "\n\n";
                    sb += "<color=#edba42>[Midnight Defense]</color>\n";
                    sb += "<color=#9c519c>Specialized in detecting and preventing Midnight Client cheaters</color>\n";
                    sb += "<color=#bfbfbf>-----------------------------------\n";

                    int averageLatency = 0;
                    int highestLatency = 0;
                    Player[] playerList = Player.List.ToArray();
                    for (int i = 0; i < playerList.Length; i++)
                    {
                        averageLatency += playerList[i].Ping;
                        if (playerList[i].Ping > highestLatency)
                            highestLatency = playerList[i].Ping;
                    }

                    averageLatency = averageLatency / Player.List.Count();

                    sb += "<color=#0073ff>Average Ping: </color><color=#bfbfbf>" + averageLatency + "ms</color>\n";
                    sb += "<color=#0073ff>Highest Ping: </color><color=#bfbfbf>" + highestLatency + "ms</color>\n";

                    int suspectedPlayers = Plugin.PlayerInfo.Count(p => p.DetectionPoints > 0);

                    sb += "<color=#0073ff># of players suspected for cheating: </color><color=#bfbfbf>" + suspectedPlayers + "</color>\n";
                    sb += "<color=#0073ff>Midnight Defense Version: </color><color=#bfbfbf>" + Plugin.Instance.Version.ToString() + "</color>\n";
                    sb += "<color=#bfbfbf>-----------------------------------\n";

                    response = sb;
                    return true;
                #endregion
                #region ANTI-CHEAT PLAYER LOGS
                case "player":
                    Player player = Player.Get(arguments.At(1));
                    if (player == null)
                    {
                        response = "That player does not exist!";
                        return false;
                    }

                    PlayerInfo playerInfo = Plugin.PlayerInfo.Find(p => p.Player == player);

                    string playerSB = "\n";
                    playerSB += "<color=#edba42>[Midnight Defense]</color>\n";
                    playerSB += "<color=#0073ff>Currently Suspected: </color><color=#bfbfbf>" + (playerInfo.DetectionPoints >= 1 ? true : false).ToString() + "</color>\n";
                    playerSB += "<color=#bfbfbf>---------------LOGS-----------------\n";
                    playerSB += Plugin.PlayerLogs[player];

                    response = playerSB;
                    return true;
                #endregion
                #region ANTI-CHEAT CONFIG
                case "config":

                    if (!(sender is PlayerCommandSender pSender))
                    {
                        string configSB = "\n\n";
                        configSB += "[Midnight Defense]\n";
                        configSB += "Specialized in detecting and preventing Midnight Client cheaters\n";
                        configSB += "-----------FRIENDLY-FIRE-------------\n";
                        configSB += "Friendly-Fire Detection: " + config.FFDetection + "\n";
                        configSB += "Negate all Friendly-Fire Damage: " + config.NegateFFDamage + "\n";
                        configSB += "Friendly-Fire Detection Points: " + config.FFDetectionPoints + "\n";
                        configSB += "-----------SCP-049 MOVEMENT-------------\n";
                        configSB += "SCP-049 Detection: " + config.SCP049Detection + "\n";
                        configSB += "SCP-049 Detection Points: " + config.SCP049DetectedMovementPoints + "\n";
                        configSB += "-----------INFINITE RANGE-------------\n";
                        configSB += "Infinite Range Detection: " + config.InfiniteRangeDetection + "\n";
                        configSB += "Negate all Infinite Range Damage: " + config.NegateInfiniteRangeDamage + "\n";
                        configSB += "Infinite Range Detection Points: " + config.InfiniteRangeDetectionPoints + "\n";
                        configSB += "Maxiumum Range before Detection: " + config.RangeDistance + "\n";
                        configSB += "-----------SPEEDHACK-------------\n";
                        configSB += "Speedhack Detection: " + BooleanStringBuilder(config.SpeedhackDetection) + "\n";
                        configSB += "Speedhack Maxiumum Speed: " + config.NoclipMaximumVelocity + "\n";
                        configSB += "Speedhack Detection Points: " + config.SpeedhackDetectionPoints + "\n";
                        configSB += "-----------SILENT AIMBOT-------------\n";
                        configSB += "Silent Aimbot Detection: " + config.SilentAimbotDetection + "\n";
                        configSB += "Silent Aimbot Detection Points: " + config.SilentAimbotDetectionPoints + "\n";
                        configSB += "Silent Aimbot Player: [" + string.Join(", ", config.SilentAimbotPlayerSize) + "]\n";
                        configSB += "-----------NO CLIP-------------\n";
                        configSB += "NoClip Detection: " + BooleanStringBuilder(config.NoclipDetection) + "\n";
                        configSB += "NoClip Detection Points: " + config.NoclipDetectionPoints + "\n";
                        configSB += "NoClip Detection Rubberband: " + BooleanStringBuilder(config.NoclipRubberband) + "\n";

                        response = configSB;
                        return true;
                    }
                    else
                    {
                        string configSB = "\n\n";
                        configSB += "<color=#edba42>[Midnight Defense]</color>\n";
                        configSB += "<color=#9c519c>Specialized in detecting and preventing Midnight Client cheaters</color>\n";
                        configSB += "<color=#bfbfbf>-----------FRIENDLY-FIRE-------------\n";
                        configSB += "<color=#bfbfbf>Friendly-Fire Detection: </color>" + BooleanStringBuilder(config.FFDetection) + "\n";
                        configSB += "<color=#bfbfbf>Negate all Friendly-Fire Damage: </color>" + BooleanStringBuilder(config.NegateFFDamage) + "\n";
                        configSB += "<color=#bfbfbf>Friendly-Fire Detection Points: </color><color=#2ea339>" + config.FFDetectionPoints + "</color>\n";
                        configSB += "<color=#bfbfbf>-----------SCP-049 MOVEMENT-------------\n";
                        configSB += "<color=#bfbfbf>SCP-049 Detection: </color>" + BooleanStringBuilder(config.SCP049Detection) + "\n";
                        configSB += "<color=#bfbfbf>SCP-049 Detection Points: </color><color=#2ea339>" + config.SCP049DetectedMovementPoints + "\n";
                        configSB += "<color=#bfbfbf>-----------INFINITE RANGE-------------\n";
                        configSB += "<color=#bfbfbf>Infinite Range Detection: </color>" + BooleanStringBuilder(config.InfiniteRangeDetection) + "\n";
                        configSB += "<color=#bfbfbf>Negate all Infinite Range Damage: </color>" + BooleanStringBuilder(config.NegateInfiniteRangeDamage) + "\n";
                        configSB += "<color=#bfbfbf>Infinite Range Detection Points: </color><color=#2ea339>" + config.InfiniteRangeDetectionPoints + "\n";
                        configSB += "<color=#bfbfbf>Maxiumum Range before Detection: </color><color=#2ea339>" + config.RangeDistance + "\n";
                        configSB += "<color=#bfbfbf>-----------SPEEDHACK-------------\n";
                        configSB += "<color=#bfbfbf>Speedhack Detection: </color>" + BooleanStringBuilder(config.SpeedhackDetection) + "\n";
                        configSB += "<color=#bfbfbf>Speedhack Maxiumum Speed: </color>" + config.NoclipMaximumVelocity + "\n";
                        configSB += "<color=#bfbfbf>Speedhack Detection Points: </color><color=#2ea339>" + config.SpeedhackDetectionPoints + "\n";
                        configSB += "<color=#bfbfbf>-----------SILENT AIMBOT-------------\n";
                        configSB += "<color=#bfbfbf>Silent Aimbot Detection: </color>" + BooleanStringBuilder(config.SilentAimbotDetection) + "\n";
                        configSB += "<color=#bfbfbf>Silent Aimbot Detection Points: </color><color=#2ea339>" + config.SilentAimbotDetectionPoints + "\n";
                        configSB += "<color=#bfbfbf>Silent Aimbot Player Size: </color><color=#2ea339>[" + string.Join(", ", config.SilentAimbotPlayerSize) + "]\n";
                        configSB += "<color=#bfbfbf>-----------NO CLIP-------------\n";
                        configSB += "<color=#bfbfbf>NoClip Detection: </color>" + BooleanStringBuilder(config.NoclipDetection) + "\n";
                        configSB += "<color=#bfbfbf>NoClip Detection Points: </color><color=#2ea339>" + config.NoclipDetectionPoints + "\n";
                        configSB += "<color=#bfbfbf>NoClip Detection Rubberband: </color>" + BooleanStringBuilder(config.NoclipRubberband) + "\n";

                        response = configSB;
                        return true;
                    }
                    
                #endregion
                #region ANTI-CHEAT TRIGGER
                case "monitor":
                    Player triggerPlayer = Player.Get(arguments.At(1));
                    if (triggerPlayer == null)
                    {
                        response = "Invalid Player";
                        return false;
                    }

                    PlayerInfo playerinfo = Plugin.PlayerInfo.Find(p => p.Player == triggerPlayer);

                    if (playerinfo.MonitorForAimbot)
                    {
                        response = "MD-AC is no longer monitoring for Aimbot";
                        AntiAimbotPlayer aimbotPlayer = API.MonitoringAimbot[playerinfo.Player];
                        aimbotPlayer.Destroy();

                        API.MonitoringAimbot.Remove(playerinfo.Player);
                        playerinfo.MonitorForAimbot = false;
                        return true;
                    }
                    else
                    {
                        response = "MD-AC will now monitor that player for Aimbot";

                        float[] arr = config.SilentAimbotPlayerSize;
                        AntiAimbotPlayer antiAimbotPlayer = new AntiAimbotPlayer(RoleType.NtfPrivate, new Vector3(arr[0], arr[1], arr[2]), triggerPlayer);
                        antiAimbotPlayer.Spawn();

                        API.MonitoringAimbot.Add(playerinfo.Player, antiAimbotPlayer);
                        playerinfo.MonitorForAimbot = true;
                        return true;
                    }
                #endregion
                #region ANTI-CHEAT DISMISSAL
                case "dismiss":
                    Player dismissPlayer = Player.Get(arguments.At(1));

                    PlayerInfo playerDismissalInfo = Plugin.PlayerInfo.Find(p => p.Player == dismissPlayer);
                    playerDismissalInfo.DetectionPoints = 0;
                    playerDismissalInfo.DetectedCheats.Clear();
                    playerDismissalInfo.MonitorForAimbot = false;
                    playerDismissalInfo.ReporterPlayers.Clear();
                    playerDismissalInfo.ReportCount = 0;

                    if (API.MonitoringAimbot.ContainsKey(dismissPlayer))
                    {
                        API.MonitoringAimbot[dismissPlayer].Destroy();
                        API.MonitoringAimbot.Remove(dismissPlayer);
                    }
 
                    PlayerLog dismissalLog = new PlayerLog(dismissPlayer);
                    dismissalLog.Log(LogType.Informational, "Player has been dismissed by " + Player.Get(sender).Nickname);

                    response = "Player has been dismissed!";
                    return true;
                #endregion
                default:
                    response = helpString;
                    return false;
            }
        }

        private string BooleanStringBuilder(bool configOption)
        {
            if (configOption)
                return "<color=#2ea339>True</color>";
            else
                return "<color=#ab423a>False</color>";
        }
    }
}
