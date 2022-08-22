using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
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

namespace MidnightDefense
{
    public class API
    {

        private static System.Random random;

        public static Dictionary<Player, AntiAimbotPlayer> MonitoringAimbot = new Dictionary<Player, AntiAimbotPlayer>();
        public static IEnumerator<float> MonitorAimbotEnumerator()
        {
            random = new System.Random();
            for (; ; )
            {
                if (MonitoringAimbot.Count() > 0)
                {
                    for (int i = 0; i < MonitoringAimbot.Count(); i++)
                    {
                        KeyValuePair<Player, AntiAimbotPlayer> keyPair = MonitoringAimbot.ElementAt(i);
                        Player player = keyPair.Key;
                        Vector3 forward = player.CameraTransform.forward;
                        if (Physics.Raycast(player.CameraTransform.position, forward, out RaycastHit hit, 300f))
                        {
                            Vector3 rayCastHit = hit.point;
                            Vector3 midPoint = Vector3.Lerp(rayCastHit, player.Position, 0.5f);

                            midPoint.y += random.Next(-1, 5);
                            midPoint.x += random.Next(-2, 4);
                            midPoint.z += random.Next(-2, 4);

                            keyPair.Value.Player.Position = midPoint;
                        }

                    }
                }

                yield return Timing.WaitForSeconds(Plugin.Instance.Config.SilentAimbotTeleportTime);
                    
            }
        }

        public static IEnumerator<float> MonitorPlayerSpeed()
        {
            Config cfg = Plugin.Instance.Config;
            for (; ; )
            {
                List<PlayerInfo> playersList = Plugin.PlayerInfo;

                for (int i = 0; i < playersList.Count; i++)
                {
                    PlayerInfo playerInfo = playersList[i];

                    if (!playerInfo.Player.IsAlive)
                        continue;

                    PlayerPositionData posData = playerInfo.Position;
                    List<PlayerEffect> currentEffects = playerInfo.Player.ActiveEffects.ToList();

                    if (posData.DistanceBetweenPositions > cfg.NoclipMaximumVelocity)
                    {
                        if (playerInfo.Player.RemoteAdminAccess && !cfg.NoClipDetectionStaff)
                            continue;

                        if (cfg.NoClipAllowedRoles.Contains(playerInfo.Player.Role))
                            continue;

                        if (posData.LastPosition == new Vector3(0f, 0f, 0f))
                            continue;

                        if (currentEffects.Any(x => x.name == "Scp207"))
                            continue;

                        if (posData.DistanceBetweenPositions > 45f)
                            continue;

                        if (playerInfo.Player.Role == RoleType.Scp096 || playerInfo.Player.Role == RoleType.Scp079 || playerInfo.Player.Role == RoleType.Scp173)
                            continue;

                        if (Physics.Raycast(posData.Position, Vector3.down, out RaycastHit hit))
                            if (hit.transform.gameObject.tag == "LiftTarget")
                                continue;

                        Log.Info(playerInfo.Player.Nickname + " is moving faster than usual! " + " +" + posData.DistanceBetweenPositions);

                        if (cfg.NoclipRubberband)
                            playerInfo.Player.Position = posData.LastPosition;


                        PlayerLog playerLog = new PlayerLog(playerInfo.Player);
                        playerLog.Log(LogType.Detected, Plugin.Instance.Translation.NoclipDetectionMessage);

                        playerInfo.DetectionPoints += Plugin.Instance.Config.NoclipDetectionPoints;
                        playerInfo.DetectedCheats.NoDuplicateAdd(CheatsEnum.Noclip);


                    }

                    Plugin.PlayerInfo[i].Position.LastPosition = playerInfo.Player.Position;

                    
                }

                yield return Timing.WaitForSeconds(cfg.NoClipDetectionSpeed);
            }
        }

    }
}
