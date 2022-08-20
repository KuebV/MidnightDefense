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
        public static IEnumerator<float> MonitorAimbotEnumerator()
        {
            random = new System.Random();
            for (; ; )
            {
                List<PlayerInfo> players = Plugin.PlayerInfo.FindAll(x => x.MonitorForAimbot == true);
                for (int i = 0; i < players.Count; i++)
                {
                    PlayerInfo player = players[i];

                    Vector3 forward = player.Player.CameraTransform.forward;
                    if (Physics.Raycast(player.Player.CameraTransform.position, forward, out RaycastHit hit, 300f))
                    {
                        Vector3 rayCastHit = hit.point;
                        Vector3 midPoint = Vector3.Lerp(rayCastHit, player.Player.Position, 0.5f);

                        midPoint.y += random.Next(-1, 5);
                        midPoint.x += random.Next(-2, 4);
                        midPoint.z += random.Next(-2, 4);

                        player.AimbotPlayer.Position = midPoint;
                    }
                }

                yield return Timing.WaitForSeconds(Plugin.Instance.Config.SilentAimbotTeleportTime);
                    
            }
        }


        //This will be implemented on the Beta Branch at a later date
        /*public static IEnumerator<float> MonitorPlayerSpeed()
        {
            for (; ;)
            {
                
            }
        }*/
    }
}
