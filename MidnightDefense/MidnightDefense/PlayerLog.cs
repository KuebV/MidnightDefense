using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightDefense
{
    public class PlayerLog
    {

        public PlayerLog(Player player)
        {
            this.player = player;
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            PlayerFile = Path.Combine(LogDirectory, player.RawUserId + ".txt");

            if (!Plugin.PlayerLogs.ContainsKey(player))
                Plugin.PlayerLogs.Add(player, "");

        }
        private Player player;
        private string LogDirectory = Path.Combine(Paths.IndividualConfigs, "Logs");
        private string PlayerFile;

        public void Log(LogType logType, string message)
        {
            switch (logType)
            {
                case LogType.Informational:
                    UpdateDictionaryPlayer("\n<color=#407abd>[Info]</color>: <color=#b5b5b5>" + message + "</color>");
                    using (StreamWriter sw = new StreamWriter(PlayerFile, append: true))
                    {
                        sw.WriteLine("[INFO]: " + message);
                        sw.Close();
                    }
                    break;
                case LogType.Alert:
                    UpdateDictionaryPlayer("\n<color=#edbd4c>[⚠]</color>: <color=#b5b5b5>" + message + "</color>");
                    using (StreamWriter sw = new StreamWriter(PlayerFile, append: true))
                    {
                        sw.WriteLine("[ALERT]: " + message);
                        sw.Close();
                    }
                    break;
                case LogType.Detected:
                    UpdateDictionaryPlayer("\n<color=#f0311f>[⚠]</color>: <color=#b5b5b5>" + message + "</color>");
                    using (StreamWriter sw = new StreamWriter(PlayerFile, append: true))
                    {
                        sw.WriteLine("[DETECTED]: " + message);
                        sw.Close();
                    }
                    break;
            }
        }

        private void UpdateDictionaryPlayer(string message)
        {
            if (!Plugin.PlayerLogs.ContainsKey(player))
                Plugin.PlayerLogs.Add(player, message);
            else
                Plugin.PlayerLogs[player] += message;
        }

    }

    public enum LogType
    {
        Informational,
        Alert,
        Detected
    }
}
