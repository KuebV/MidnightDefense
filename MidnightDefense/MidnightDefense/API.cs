using Exiled.API.Features;
using MEC;
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
        public static IEnumerator<float> MonitorAimbotEnumerator()
        {
            random = new System.Random();
            for (; ; )
            {
                if (Plugin.MonitoringAimbot.Count() > 0)
                {
                    for (int i = 0; i < Plugin.MonitoringAimbot.Count(); i++)
                    {
                        KeyValuePair<Player, AntiAimbotPlayer> keyPair = Plugin.MonitoringAimbot.ElementAt(i);
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
    }
}
