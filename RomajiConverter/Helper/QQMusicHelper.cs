using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Opportunity.LrcParser;

namespace RomajiConverter.Helper
{
    public static class QQMusicHelper
    {
        [DllImport("user32")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lptrString, int nMaxCount);

        public static string GetCurrentSongmid()
        {
            var hwnd = Process.GetProcessesByName("QQMusic").Select(p => p.MainWindowHandle).FirstOrDefault();
            var stringBuilder = new StringBuilder(512);
            GetWindowText(hwnd, stringBuilder, stringBuilder.Capacity);
            var songName = stringBuilder.ToString().Trim();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36");
            var result = httpClient.GetStringAsync(new Uri($"https://api.qq.jsososo.com/search/quick?key={songName}")).Result;
            var jObject = JObject.Parse(result);
            var list = jObject["data"]["song"]["itemlist"];
            if (list.Any()) return (string) list[0]["mid"];
            return "";
        }

        public static List<ReturnLrc> GetLrc(string mid)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36");
            var result = httpClient.GetStringAsync(new Uri($"https://api.qq.jsososo.com/lyric?songmid={mid}")).Result;
            var jObject = JObject.Parse(result);
            var jpnLrc = Lyrics.Parse((string) jObject["data"]["lyric"]);
            var chnLrc = Lyrics.Parse((string) jObject["data"]["trans"]);
            var lrcList = jpnLrc.Lyrics.Lines.Select(line => new ReturnLrc { Time = line.Timestamp.ToString("mm:ss.fff"), JLrc = line.Content }).ToList();
            foreach (var line in chnLrc.Lyrics.Lines)
            {
                foreach (var lrc in lrcList.Where(lrc => lrc.Time == line.Timestamp.ToString("mm:ss.fff")))
                {
                    lrc.CLrc = line.Content;
                }
            }
            return lrcList;
        }
    }
}
