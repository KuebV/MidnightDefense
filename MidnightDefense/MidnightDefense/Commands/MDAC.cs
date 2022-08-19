using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Mirror;
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

                    int suspectedPlayers = 0;
                    for (int i = 0; i < Plugin.SuspectedPlayers.Count; i++)
                    {
                        if (Plugin.SuspectedPlayers.ElementAt(i).Value >= 1)
                            suspectedPlayers++;
                    }

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

                    string playerSB = "\n";
                    playerSB += "<color=#edba42>[Midnight Defense]</color>\n";
                    playerSB += "<color=#0073ff>Currently Suspected: </color><color=#bfbfbf>" + (Plugin.SuspectedPlayers[player] >= 1 ? true : false).ToString() + "</color>\n";
                    playerSB += "<color=#bfbfbf>---------------LOGS-----------------\n";
                    playerSB += Plugin.PlayerLogs[player];

                    response = playerSB;
                    return true;
                #endregion
                #region ANTI-CHEAT CONFIG
                case "config":
                    string configSB = "\n\n";
                    configSB += "<color=#edba42>[Midnight Defense]</color>\n";
                    configSB += "<color=#9c519c>Specialized in detecting and preventing Midnight Client cheaters</color>\n";
                    configSB += "<color=#bfbfbf>-----------FRIENDLY-FIRE-------------\n";
                    configSB += "<color=#bfbfbf>Friendly-Fire Detection: </color>" + BooleanStringBuilder(Plugin.Instance.Config.FFDetection) + "\n";
                    configSB += "<color=#bfbfbf>Negate all Friendly-Fire Damage: </color>" + BooleanStringBuilder(Plugin.Instance.Config.NegateFFDamage) + "\n";
                    configSB += "<color=#bfbfbf>Friendly-Fire Detection Points: </color><color=#2ea339>" + Plugin.Instance.Config.FFDetectionPoints + "</color>\n";
                    configSB += "<color=#bfbfbf>-----------SCP-049 MOVEMENT-------------\n";
                    configSB += "<color=#bfbfbf>SCP-049 Detection: </color>" + BooleanStringBuilder(Plugin.Instance.Config.SCP049Detection) + "\n";
                    configSB += "<color=#bfbfbf>SCP-049 Detection Points: </color><color=#2ea339>" + Plugin.Instance.Config.SCP049DetectedMovementPoints + "\n";
                    configSB += "<color=#bfbfbf>-----------INFINITE RANGE-------------\n";
                    configSB += "<color=#bfbfbf>Infinite Range Detection: </color>" + BooleanStringBuilder(Plugin.Instance.Config.InfiniteRangeDetection) + "\n";
                    configSB += "<color=#bfbfbf>Negate all Infinite Range Damage: </color>" + BooleanStringBuilder(Plugin.Instance.Config.NegateInfiniteRangeDamage) + "\n";
                    configSB += "<color=#bfbfbf>Infinite Range Detection Points: </color><color=#2ea339>" + Plugin.Instance.Config.InfiniteRangeDetectionPoints + "\n";
                    configSB += "<color=#bfbfbf>Maxiumum Range before Detection: </color><color=#2ea339>" + Plugin.Instance.Config.RangeDistance + "\n";
                    configSB += "<color=#bfbfbf>-----------SPEEDHACK-------------\n";
                    configSB += "<color=#bfbfbf>Speedhack Detection: </color>" + BooleanStringBuilder(Plugin.Instance.Config.SpeedhackDetection) + "\n";
                    configSB += "<color=#bfbfbf>Cancel Event if trigger Speedhack: </color>" + BooleanStringBuilder(Plugin.Instance.Config.SpeedhackDetectionCancelEvent) + "\n";
                    configSB += "<color=#bfbfbf>Speedhack Detection Points: </color><color=#2ea339>" + Plugin.Instance.Config.SpeedhackDetectionPoints + "\n";
                    configSB += "<color=#bfbfbf>Speedhack Milliseconds Threshold: </color><color=#2ea339>" + Plugin.Instance.Config.SpeedhackDetectionThreshold + "\n";
                    configSB += "<color=#bfbfbf>-----------SILENT AIMBOT-------------\n";
                    configSB += "<color=#bfbfbf>Silent Aimbot Detection: </color>" + BooleanStringBuilder(Plugin.Instance.Config.SilentAimbotDetection) + "\n";
                    configSB += "<color=#bfbfbf>Silent Aimbot Detection Points: </color><color=#2ea339>" + Plugin.Instance.Config.SilentAimbotDetectionPoints + "\n";
                    configSB += "<color=#bfbfbf>Silent Aimbot Player: </color><color=#2ea339>[" + string.Join(", ", Plugin.Instance.Config.SilentAimbotPlayerSize) + "]\n";


                    response = configSB;
                    return true;
                #endregion
                #region ANTI-CHEAT TRIGGER
                case "monitor":
                    Player triggerPlayer = Player.Get(arguments.At(1));
                    if (triggerPlayer == null)
                    {
                        response = "Invalid Player";
                        return false;
                    }

                    if (Plugin.MonitoringAimbot.ContainsKey(triggerPlayer))
                    {
                        response = "MD-AC is no longer monitoring for Aimbot";
                        AntiAimbotPlayer aimbotPlayer = Plugin.MonitoringAimbot[triggerPlayer];
                        aimbotPlayer.Destroy();

                        Plugin.MonitoringAimbot.Remove(triggerPlayer);

                        return true;
                    }
                    else
                    {
                        response = "MD-AC will now monitor that player for Aimbot";

                        float[] arr = Plugin.Instance.Config.SilentAimbotPlayerSize;
                        AntiAimbotPlayer antiAimbotPlayer = new AntiAimbotPlayer(RoleType.ClassD, "MD-AC", new Vector3(arr[0], arr[1], arr[2]));
                        antiAimbotPlayer.Spawn();

                        antiAimbotPlayer.Player.TargetGhostsHashSet.Remove(triggerPlayer.Id);
                        Plugin.MonitoringAimbot.Add(triggerPlayer, antiAimbotPlayer);
                        Plugin.SilentAimbotHitCounter.Add(triggerPlayer, 0);
                        return true;
                    }
                #endregion
                #region ANTI-CHEAT DISMISSAL
                case "dismiss":
                    Player dismissPlayer = Player.Get(arguments.At(1));
                    Plugin.SuspectedPlayers[dismissPlayer] = 0;

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
