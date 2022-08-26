using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Permissions.Extensions;
using MEC;
using MidnightDefense.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MidnightDefense
{
    public class EventHandlers
    {

        private Config config;
        public EventHandlers(Config cfg) => config = cfg;
        /// <summary>
        /// Anti Human-Teamkilling when FF is OFF
        /// </summary>
        /// <param name="ev"></param>

        public void BeforePlayerDeath(DyingEventArgs ev)
        {
            if (ev.Killer == null) return;
            if (ev.Target == null) return;
            if (ev.Target.IsScp) return;

            // Suicide Checking
            if (ev.Killer == ev.Target) return;

            if (ev.Killer.CheckPermission("md.ffbypass"))
                return;

            if (config.FFDetection)
            {
                if (!Server.FriendlyFire)
                {
                    Team attackerTeam = ev.Killer.Role.Team;
                    Team targetTeam = ev.Target.Role.Team;

                    string ffMessage = Plugin.Instance.Translation.FFDetectedMessage;
                    if (attackerTeam == targetTeam)
                    {
                        PlayerLog playerLog = new PlayerLog(ev.Killer);
                        playerLog.Log(LogType.Detected, ffMessage);

                        PlayerInfo pInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Killer);
                        pInfo.DetectionPoints += config.FFDetectionPoints;
                        pInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.FriendlyFire);

                        if (config.NegateFFDamage)
                            ev.IsAllowed = false;

                    }
                }
            }
        }

        /// <summary>
        /// Prevents SCP Friendly-Fire & SCP Teamkilling
        /// </summary>
        /// <param name="ev"></param>
        public void OnPlayerHurt(HurtingEventArgs ev)
        {

            if (ev.Attacker == null) return;
            if (ev.Target == null) return;
            if (ev.Attacker.IsHuman) return;
            if (ev.Target.IsHuman) return;

            if (ev.Attacker.CheckPermission("md.ffbypass"))
                return;

            if (config.FFDetection)
            {
                if (!Server.FriendlyFire)
                {

                    Team attackerTeam = ev.Attacker.Role.Team;
                    Team targetTeam = ev.Target.Role.Team;

                    string ffMessage = Plugin.Instance.Translation.FFDetectedMessage;
                    if (attackerTeam == targetTeam)
                    {
                        PlayerLog playerLog = new PlayerLog(ev.Attacker);
                        playerLog.Log(LogType.Detected, ffMessage);

                        PlayerInfo pInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Attacker);
                        pInfo.DetectionPoints += config.FFDetectionPoints;
                        pInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.FriendlyFire);

                        if (config.NegateFFDamage)
                            ev.IsAllowed = false;

                    }

                }
            }

            if (config.InfiniteRangeDetection)
            {
                if (ev.Attacker.IsScp)
                {
                    Vector3 attackerPosition = ev.Attacker.Position;
                    Vector3 targetPosition = ev.Target.Position;
                    float distance = Vector3.Distance(targetPosition, attackerPosition);
                    if (distance > config.RangeDistance)
                    {
                        PlayerLog log = new PlayerLog(ev.Attacker);
                        log.Log(LogType.Detected, Plugin.Instance.Translation.InfiniteRangeDetectionMessage);

                        PlayerInfo pInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Attacker);
                        pInfo.DetectionPoints += config.InfiniteRangeDetectionPoints;
                        pInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.InfiniteRange);

                        if (config.NegateInfiniteRangeDamage)
                            ev.IsAllowed = false;
                    }
                }
            }

        }

        /// <summary>
        /// Anti-Aimbot Protections
        /// </summary>
        /// <param name="ev"></param>
        public void OnShot(ShotEventArgs ev)
        {
            if (!config.SilentAimbotDetection)
                return;

            PlayerInfo playerInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Shooter);
            if (!playerInfo.MonitorForAimbot)
                return;

            if (ev.Shooter.CheckPermission("md.aimbotbypass"))
                return;

            Vector3 forward = ev.Shooter.CameraTransform.forward;
            if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out RaycastHit hit, 300f))
            {
                if (hit.transform.gameObject.tag == "Player")
                {

                    if (API.MonitoringAimbot.Any(x => x.Value.GameObject == hit.transform.gameObject)) 
                    {
                        int currentHits = playerInfo.SilentAimbotHitCounter;
                        if (currentHits >= config.SilentAimbotHitThreshold)
                        {

                            PlayerLog log = new PlayerLog(ev.Shooter);
                            log.Log(LogType.Detected, Plugin.Instance.Translation.SilentAimbotDetectionMessage);

                            playerInfo.DetectionPoints += config.SilentAimbotDetectionPoints;
                            playerInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.Aimbot);
                        }

                        playerInfo.SilentAimbotHitCounter++;
                    }
                }
                else
                    playerInfo.SilentAimbotHitCounter = 0;
            }

        }



        private static Dictionary<Player, Vector3> scp049recallPosition = new Dictionary<Player, Vector3>();

        #region SCP-049 Reviving while moving patch
        /// <summary>
        /// Player 049 starts to revive player
        /// Store their location in a Vector3 in a dictionary, and check it when they are revived
        /// </summary>
        /// <param name="ev"></param>
        public void OnRecall(StartingRecallEventArgs ev)
        {
            if (!config.SCP049Detection)
                return;

            if (!scp049recallPosition.ContainsKey(ev.Scp049))
                scp049recallPosition.Add(ev.Scp049, ev.Scp049.Position);
            else
            {
                // Incase it gets cancelled?
                scp049recallPosition.Remove(ev.Scp049);
                scp049recallPosition.Add(ev.Scp049, ev.Scp049.Position);
            }
        }

        public void OnRevive(FinishingRecallEventArgs ev)
        {
            if (!config.SCP049Detection)
                return;

            if (ev.Scp049.CheckPermission("md.049bypass"))
                return;

            // Uhh, if you didn't initaite the recall then don't allow it to even happen
            if (!scp049recallPosition.ContainsKey(ev.Scp049))
                ev.IsAllowed = false;

            Vector3 previousPosition = scp049recallPosition[ev.Scp049];
            if (previousPosition != ev.Scp049.Position)
            {
                PlayerInfo playerInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Scp049);
                playerInfo.DetectionPoints += config.SCP049DetectedMovementPoints;
                playerInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.SCP049Movement);

                PlayerLog playerLog = new PlayerLog(ev.Scp049);
                playerLog.Log(LogType.Detected, Plugin.Instance.Translation.SCP049DetectedMessage);
                ev.IsAllowed = false;
            }
        }
        #endregion

        public void OnLeave(LeftEventArgs ev) => Plugin.PlayerInfo.RemoveAll(p => p.Player == ev.Player);

        public void ChangeClass(ChangingRoleEventArgs ev)
        {
            try
            {
                if (!Plugin.PlayerInfo.Any(x => x.Player == ev.Player))
                {
                    Timing.CallDelayed(3f, () =>
                    {
                        float[] scaleArr = config.SilentAimbotPlayerSize;
                        Plugin.PlayerInfo.Add(new PlayerInfo
                        {
                            Player = ev.Player,
                            Position = new PlayerPositionData(ev.Player),
                            MonitorForAimbot = false,
                            SilentAimbotHitCounter = 0,
                            LastBulletFired = 0,
                            DetectedCheats = new List<CheatsEnum> { },
                            DetectionPoints = 0,
                        });

                        PlayerLog playerLog = new PlayerLog(ev.Player);
                        playerLog.Log(LogType.Informational, "<color=#6ff263>--START OF LOG--</color>");

                        Log.Info(ev.Player.Nickname + " has been initalized!");
                    });
                }

                if (ev.NewRole == RoleType.Spectator || ev.NewRole == RoleType.Tutorial)
                {
                    Plugin.PlayerInfo.Find(x => x.Player == ev.Player).Position.LastPosition = new Vector3(0f, 0f, 0f);
                    return;
                }

                if (Plugin.PlayerInfo.Any(x => x.Player == ev.Player))
                {
                    Timing.CallDelayed(3f, () =>
                    {
                        Plugin.PlayerInfo.Find(x => x.Player == ev.Player).Position.LastPosition = ev.Player.Position;
                    });
                }
            }

            // Player just left, but plugin freaks out
            catch (NullReferenceException ex) { return; }


        }

        public void OnCheaterReport(ReportingCheaterEventArgs ev)
        {
            if (config.SilentAimbotTriggerReportAmount == -1)
                return;

            PlayerInfo playerInfo = Plugin.PlayerInfo.Find(x => x.Player == ev.Target);
            if (playerInfo == null)
            {
                Log.Error("OnCheaterReport: Targetted Player is null!");
                return;
            }

            // Players who have already reported the player shouldn't be able to spam report them and trigger the aimbot
            if (playerInfo.ReporterPlayers.Contains(ev.Issuer))
                return;

            // If the player is already being monitored for aimbot
            if (API.MonitoringAimbot.ContainsKey(ev.Target))
                return;

            playerInfo.ReportCount++;
            playerInfo.ReporterPlayers.Add(ev.Issuer);

            if (playerInfo.ReportCount >= config.SilentAimbotTriggerReportAmount)
            {

                float[] arr = config.SilentAimbotPlayerSize;
                AntiAimbotPlayer antiAimbotPlayer = new AntiAimbotPlayer(RoleType.NtfPrivate, new Vector3(arr[0], arr[1], arr[2]), ev.Target);
                antiAimbotPlayer.Spawn();

                API.MonitoringAimbot.Add(ev.Target, antiAimbotPlayer);
                playerInfo.MonitorForAimbot = true;
            }

            
        }


        public static void AlertOnlineStaff(Player suspectedPlayer)
        {
            for (int i = 0; i < Player.List.Count(); i++)
            {
                Player player = Player.List.ElementAt(i);
                if (player.ReferenceHub.serverRoles.RemoteAdmin)
                {
                    string sb = "";
                    for (int x = 0; x < 14; x++)
                        sb += "\n";

                    sb += Plugin.Instance.Translation.PointThresholdMessage.Replace("%player%", suspectedPlayer.Nickname);
                    player.ShowHint(sb, 30);
                }

            }
        }

        public static IEnumerator<float> CheckSuspectedPlayerPoints()
        {
            for(; ; )
            {
                List<PlayerInfo> suspectedPlayers = Plugin.PlayerInfo.FindAll(x=> x.DetectionPoints >= Plugin.Instance.Config.PointThreshold);

                for (int i = 0; i < suspectedPlayers.Count; i++)
                {
                    try
                    {
                        if (suspectedPlayers[i].AlertCount <= Plugin.Instance.Config.AlertMaxTimes)
                        {
                            if (Plugin.Instance.Config.AlertOnlineStaff)
                                AlertOnlineStaff(suspectedPlayers[i].Player);

                            if (Plugin.Instance.Config.DiscordWebhookEnabled)
                            {
                                _ = Webhook.SendWebhook(new Webhook
                                {
                                    detectedPlayer = suspectedPlayers[i].Player,
                                    Message = Plugin.Instance.Translation.DiscordAlertMessage,
                                    detectedCheats = suspectedPlayers[i].DetectedCheats.ToArray()
                                });
                            }
                        }
                    }
                    catch (Exception ex) { Log.Error(ex); }

                    suspectedPlayers[i].AlertCount++;
                }

                yield return Timing.WaitForSeconds(Plugin.Instance.Config.AlertTimeframe);
            }
        }
    }
}
