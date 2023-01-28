using Newtonsoft.Json;

namespace UplayKit
{
    internal class InternalEx
    {
        public static void WriteEx(Exception ex)
        {
            string ToWrite = "\n";
            ToWrite += DateTime.UtcNow.ToString("yyyy.MM.dd hh:mm:ss") + " (UTC)";
            ToWrite += $"\n{ex.InnerException}";
            ToWrite += $"\n{ex.StackTrace}";
            ToWrite += $"\n{ex.Message}";
            ToWrite += $"\n{ex.Source}";
            ToWrite += $"\n{ex.TargetSite}";
            ToWrite += $"\n{JsonConvert.SerializeObject(ex.Data)}";
            ToWrite += $"\n{ex.ToString()}";
            File.AppendAllText("UplayKit_Ex.txt", ToWrite);
        }
    }
}
