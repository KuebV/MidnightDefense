using Exiled.API.Features;
using Exiled.Events.EventArgs;
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

        /// <summary>
        /// Anti Human-Teamkilling when FF is OFF
        /// </summary>
        /// <param name="ev"></param>

        public static void BeforePlayerDeath(DyingEventArgs ev)
        {
            if (ev.Killer == null) return;
            if (ev.Target == null) return;
            if (ev.Target.IsScp) return;

            if (Plugin.Instance.Config.FFDetection)
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
                        pInfo.DetectionPoints += Plugin.Instance.Config.FFDetectionPoints;
                        pInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.FriendlyFire);

                        if (Plugin.Instance.Config.NegateFFDamage)
                            ev.IsAllowed = false;

                    }
                }
            }
        }

        /// <summary>
        /// Prevents SCP Friendly-Fire & SCP Teamkilling
        /// </summary>
        /// <param name="ev"></param>
        public static void OnPlayerHurt(HurtingEventArgs ev)
        {

            if (ev.Attacker == null) return;
            if (ev.Target == null) return;
            if (ev.Attacker.IsHuman) return;
            if (ev.Target.IsHuman) return;

            if (Plugin.Instance.Config.FFDetection)
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
                        pInfo.DetectionPoints += Plugin.Instance.Config.FFDetectionPoints;
                        pInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.FriendlyFire);

                        if (Plugin.Instance.Config.NegateFFDamage)
                            ev.IsAllowed = false;

                    }

                }
            }

            if (Plugin.Instance.Config.InfiniteRangeDetection)
            {
                if (ev.Attacker.IsScp)
                {
                    Vector3 attackerPosition = ev.Attacker.Position;
                    Vector3 targetPosition = ev.Target.Position;
                    float distance = Vector3.Distance(targetPosition, attackerPosition);
                    if (distance > Plugin.Instance.Config.RangeDistance)
                    {
                        PlayerLog log = new PlayerLog(ev.Attacker);
                        log.Log(LogType.Detected, Plugin.Instance.Translation.InfiniteRangeDetectionMessage);

                        PlayerInfo pInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Attacker);
                        pInfo.DetectionPoints += Plugin.Instance.Config.InfiniteRangeDetectionPoints;
                        pInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.InfiniteRange);

                        if (Plugin.Instance.Config.NegateInfiniteRangeDamage)
                            ev.IsAllowed = false;
                    }
                }
            }

            

        }

        /// <summary>
        /// Speedhack protection is very primitive since not much is known how it actually works
        /// 
        /// </summary>
        /// <param name="ev"></param>
        public static void OnShooting(ShootingEventArgs ev)
        {

            PlayerInfo playerInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Shooter);
            if (Plugin.Instance.Config.SpeedhackDetection)
            {
                long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                long lastShot = milliseconds - playerInfo.LastBulletFired;
                if (lastShot < Plugin.Instance.Config.SpeedhackDetectionThreshold)
                {

                    PlayerLog log = new PlayerLog(ev.Shooter);
                    log.Log(LogType.Detected, Plugin.Instance.Translation.SpeedhackDetectionMessage);

                    playerInfo.DetectionPoints += Plugin.Instance.Config.SpeedhackDetectionPoints;
                    playerInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.Speedhack);

                    if (Plugin.Instance.Config.SpeedhackDetectionCancelEvent)
                        ev.IsAllowed = false;
                }

                playerInfo.LastBulletFired = milliseconds;
            }

            
        }


        /// <summary>
        /// Anti-Aimbot Protections
        /// </summary>
        /// <param name="ev"></param>
        public static void OnShot(ShotEventArgs ev)
        {
            if (!Plugin.Instance.Config.SilentAimbotDetection)
                return;

            PlayerInfo playerInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Shooter);
            if (!playerInfo.MonitorForAimbot)
                return;

            Vector3 forward = ev.Shooter.CameraTransform.forward;
            if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out RaycastHit hit, 300f))
            {
                Log.Info(hit.transform.gameObject.tag);
                if (hit.transform.gameObject.tag == "Player")
                {

                    if (API.MonitoringAimbot.Any(x => x.Value.GameObject == hit.transform.gameObject)) 
                    {
                        int currentHits = playerInfo.SilentAimbotHitCounter;
                        if (currentHits >= Plugin.Instance.Config.SilentAimbotHitThreshold)
                        {

                            PlayerLog log = new PlayerLog(ev.Shooter);
                            log.Log(LogType.Detected, Plugin.Instance.Translation.SilentAimbotDetectionMessage);

                            playerInfo.DetectionPoints += Plugin.Instance.Config.SilentAimbotDetectionPoints;
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
        public static void OnRecall(StartingRecallEventArgs ev)
        {
            if (!Plugin.Instance.Config.SCP049Detection)
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

        public static void OnRevive(FinishingRecallEventArgs ev)
        {
            if (!Plugin.Instance.Config.SCP049Detection)
                return;

            // Uhh, if you didn't initaite the recall then don't allow it to even happen
            if (!scp049recallPosition.ContainsKey(ev.Scp049))
                ev.IsAllowed = false;

            Vector3 previousPosition = scp049recallPosition[ev.Scp049];
            if (previousPosition != ev.Scp049.Position)
            {
                if (Plugin.Instance.Config.Debug)
                    Log.Info("Player has been detected for SCP049 Movement Cheats");

                PlayerInfo playerInfo = Plugin.PlayerInfo.Find(p => p.Player == ev.Scp049);
                playerInfo.DetectionPoints += Plugin.Instance.Config.SCP049DetectedMovementPoints;
                playerInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.SCP049Movement);

                PlayerLog playerLog = new PlayerLog(ev.Scp049);
                playerLog.Log(LogType.Detected, Plugin.Instance.Translation.SCP049DetectedMessage);
                ev.IsAllowed = false;
            }
        }
        #endregion

        public static void OnJoin(JoinedEventArgs ev)
        {

            Timing.CallDelayed(3f, () =>
            {
                float[] scaleArr = Plugin.Instance.Config.SilentAimbotPlayerSize;
                Plugin.PlayerInfo.Add(new PlayerInfo
                {
                    Player = ev.Player,
                    Position = new PlayerPositionData(ev.Player),
                    MonitorForAimbot = false,
                    SilentAimbotHitCounter = 0,
                    LastBulletFired = 0,
                    DetectedCheats = new List<CheatsEnum> { }
                });

                PlayerLog playerLog = new PlayerLog(ev.Player);
                playerLog.Log(LogType.Informational, "<color=#6ff263>--START OF LOG--</color>");


                //This will be implemented on the Beta Branch at a later date
                /*if (!Plugin.PlayerPositions.Any(x => x.player == ev.Player))
                    Plugin.PlayerPositions.Add(new PlayerPositionData(ev.Player));*/

            });
            
        }

        public static void OnLeave(LeftEventArgs ev) => Plugin.PlayerInfo.RemoveAll(p => p.Player == ev.Player);

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

                    suspectedPlayers[i].AlertCount++;
                }

                yield return Timing.WaitForSeconds(Plugin.Instance.Config.AlertTimeframe);
            }
        }
    }
}
