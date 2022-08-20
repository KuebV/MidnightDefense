using Exiled.API.Features;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MidnightDefense
{
    public class AntiAimbotPlayer
    {
        public AntiAimbotPlayer(RoleType type, Vector3 scale, Player suspectedPlayer)
        {
            GameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);

            ReferenceHub = GameObject.GetComponent<ReferenceHub>();
            ReferenceHub.characterClassManager.CurClass = type;

            if (Plugin.Instance.Config.DisconnectOnAntiAimbotHit)
                ReferenceHub.characterClassManager.GodMode = false;
            else
                ReferenceHub.characterClassManager.GodMode = true;

            ReferenceHub.playerStats.StatModules[0].CurValue = 100;
            ReferenceHub.nicknameSync.Network_myNickSync = Plugin.Instance.Translation.SilentAimbotNPCName;
            ReferenceHub.characterClassManager.IsVerified = true;

            GameObject.transform.localScale = scale;
            //PlayerManager.AddPlayer(GameObject, CustomNetworkManager.slots);

            Player = new Player(GameObject);
            Player.SessionVariables.Add("IsNPC", true);

            // Feature is still experimental
            List<Player> playerList = Player.List.ToList();
            for(int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i] != suspectedPlayer)
                    Player.TargetGhostsHashSet.Add(playerList[i].Id);
            }
        }


        public GameObject GameObject { get; }
        public Player Player { get; }
        public ReferenceHub ReferenceHub { get; }
        public Vector3 Position
        {
            get => GameObject.transform.position;
            set => ReferenceHub.playerMovementSync.OverridePosition(value, null, false);
        }

        public string Name
        {
            get => ReferenceHub.nicknameSync.Network_myNickSync;
            set { ReferenceHub.nicknameSync.Network_myNickSync = value; }
        }

        public Vector3 Scale
        {
            get => Player.Scale;
            set => Player.Scale = value;
        }

        public void UnSpawn() => NetworkServer.UnSpawn(GameObject);
        public void Destroy()
        {
            PlayerManager.RemovePlayer(GameObject, CustomNetworkManager.slots);
            NetworkServer.Destroy(GameObject);
        }

        public void Spawn() => NetworkServer.Spawn(GameObject);

    }

}
