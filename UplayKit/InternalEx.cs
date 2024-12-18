﻿namespace UplayKit;

internal class InternalEx
{
    public static void WriteEx(Exception ex)
    {
        string ToWrite = "\n";
        ToWrite += DateTime.UtcNow.ToString("yyyy.MM.dd hh:mm:ss") + " (UTC) | " + DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + " (LOCAL)";
        ToWrite += $"\nMethod: {ex.TargetSite?.Name}";
        ToWrite += $"\nInner: {ex.InnerException}";
        ToWrite += $"\nStackTrace: {ex.StackTrace}";
        ToWrite += $"\nMessage: {ex.Message}";
        ToWrite += $"\nSource: {ex.Source}";
        ToWrite += $"\nHResult: {ex.HResult}";
        ToWrite += $"\nHelpLink: {ex.HelpLink}";
        File.AppendAllText("UplayKit_Ex.txt", ToWrite);
    }
}
