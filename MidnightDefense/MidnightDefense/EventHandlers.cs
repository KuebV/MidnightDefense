using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace MidnightDefense
{
    public class EventHandlers
    {

        public static void OnPlayerHurt(HurtingEventArgs ev)
        {

            if (ev.Attacker == null)
                return;

            if (ev.Target == null)
                return;


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

                        if (Plugin.Instance.Config.NegateFFDamage)
                            ev.IsAllowed = false;

                        Plugin.SuspectedPlayers[ev.Attacker]++;
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
                        Plugin.SuspectedPlayers[ev.Attacker]++;

                        if (Plugin.Instance.Config.NegateInfiniteRangeDamage)
                            ev.IsAllowed = false;
                    }
                }
            }

            

        }

        /// <summary>
        /// Contains Speedhack Protection & Anti-Aimbot Protection
        /// Speedhack protection is very primitive since not much is known how it actually works
        /// 
        /// </summary>
        /// <param name="ev"></param>
        public static void OnShooting(ShootingEventArgs ev)
        {
            if (Plugin.Instance.Config.SpeedhackDetection)
            {
                long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (!Plugin.CheckingSpeedhack.ContainsKey(ev.Shooter))
                {
                    Plugin.CheckingSpeedhack.Add(ev.Shooter, milliseconds);
                    return;
                }

                long lastShot = milliseconds - Plugin.CheckingSpeedhack[ev.Shooter];
                if (lastShot < Plugin.Instance.Config.SpeedhackDetectionThreshold)
                {
                    PlayerLog log = new PlayerLog(ev.Shooter);
                    log.Log(LogType.Detected, Plugin.Instance.Translation.SpeedhackDetectionMessage);
                    Plugin.SuspectedPlayers[ev.Shooter]++;

                    if (Plugin.Instance.Config.SpeedhackDetectionCancelEvent)
                        ev.IsAllowed = false;
                }

                Plugin.CheckingSpeedhack[ev.Shooter] = milliseconds;
            }

            if (Plugin.Instance.Config.SilentAimbotDetection)
            {
                if (!Plugin.MonitoringAimbot.ContainsKey(ev.Shooter))
                    return;

                Vector3 forward = ev.Shooter.CameraTransform.forward;
                if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out RaycastHit hit, 300f))
                {
                    if (hit.transform.gameObject.tag == "Player")
                    {
                        Player player = Player.Get(hit.transform.gameObject);
                        if (player == null)
                            return;

                        if (player.SessionVariables.ContainsKey("IsNPC"))
                        {
                            int currentHits = Plugin.SilentAimbotHitCounter[ev.Shooter];
                            if (currentHits >= Plugin.Instance.Config.SilentAimbotHitThreshold)
                            {
                                PlayerLog log = new PlayerLog(ev.Shooter);
                                log.Log(LogType.Detected, Plugin.Instance.Translation.SilentAimbotDetectionMessage);
                            }

                            Plugin.SilentAimbotHitCounter[ev.Shooter]++;
                        }
                    }
                }
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
                if (!Plugin.SuspectedPlayers.ContainsKey(ev.Player))
                    Plugin.SuspectedPlayers.Add(ev.Player, 0);

                PlayerLog playerLog = new PlayerLog(ev.Player);
                playerLog.Log(LogType.Informational, "<color=#6ff263>--START OF LOG--</color>");
            });
            
        }

        public static void OnLeave(LeftEventArgs ev)
        {
            if (Plugin.MonitoringAimbot.ContainsKey(ev.Player))
            {
                AntiAimbotPlayer aimbotPlayer = Plugin.MonitoringAimbot[ev.Player];
                aimbotPlayer.Destroy();

                Plugin.MonitoringAimbot.Remove(ev.Player);
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
                    player.ShowHint(sb, 10);
                }

            }
        }

        public static IEnumerator<float> CheckSuspectedPlayerPoints()
        {
            for(; ; )
            {
                if (Plugin.SuspectedPlayers.Count() > 0)
                {
                    for (int i = 0; i < Plugin.SuspectedPlayers.Count(); i++)
                    {
                        KeyValuePair<Player, int> suspectedPlayer = Plugin.SuspectedPlayers.ElementAt(i);
                        if (suspectedPlayer.Value >= Plugin.Instance.Config.PointThreshold)
                            AlertOnlineStaff(suspectedPlayer.Key);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
