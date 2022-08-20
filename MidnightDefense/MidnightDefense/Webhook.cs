using Exiled.API.Features;
using MidnightDefense.Objects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MidnightDefense
{
    public class Webhook
    {
        public Player detectedPlayer { get; set; }
        public CheatsEnum[] detectedCheats { get; set; }
        public string Message { get; set; }

        private readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendWebhook(Webhook webhookArgs)
        {
            List<string> cheatsListed = new List<string>();
            for (int i = 0; i < webhookArgs.detectedCheats.Length; i++)
                cheatsListed.Add(Enum.GetName(typeof(CheatsEnum), webhookArgs.detectedCheats[i]));

            string sb = "";
            sb += string.Format("[<t:{0}:f>] ", DateTimeOffset.Now.ToUnixTimeSeconds());
            sb += Plugin.Instance.Translation.DiscordAlertMessage
                .Replace("%player%", webhookArgs.detectedPlayer.Nickname)
                .Replace("%cheats%", string.Join(", ", cheatsListed));

            Post(Plugin.Instance.Config.DiscordWebhookURL, new NameValueCollection()
            {
                { "username", Plugin.Instance.Config.DiscordWebhookUsername },
                { "content", sb }
            });


        }

        private static byte[] Post(string uri, NameValueCollection pair)
        {
            using (WebClient wc = new WebClient())
                return wc.UploadValues(uri, pair);
        }
    }
}
