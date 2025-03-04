using Serilog;
using Serilog.Core;

namespace UplayKit;

public class Logs
{

    public static LoggingLevelSwitch Log_Switch = new()
    {
        MinimumLevel = Serilog.Events.LogEventLevel.Information,
    };

    public static Logger CreateMixedLog()
    {
        var Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(Log_Switch)
            .WriteTo.Console()
            .WriteTo.File("uplaykit_logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        return Logger;
    }

    public static Logger CreateConsoleLog()
    {
        var Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(Log_Switch)
            .WriteTo.Console()
            .CreateLogger();
        return Logger;
    }

    public static Logger CreateFileLog()
    {
        var Logger = new LoggerConfiguration()
         .MinimumLevel.ControlledBy(Log_Switch)
         .WriteTo.File("uplaykit_logs.txt", rollingInterval: RollingInterval.Day)
         .CreateLogger();
        return Logger;
    }
}
