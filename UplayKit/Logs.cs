using Serilog;
using Serilog.Core;

namespace UplayKit;

public class Logs
{
    public static LoggingLevelSwitch Console_Log_Switch = new()
    { 
        MinimumLevel = Serilog.Events.LogEventLevel.Information,
    };

    public static LoggingLevelSwitch File_Log_Switch = new()
    {
        MinimumLevel = Serilog.Events.LogEventLevel.Information,
    };

    public static LoggingLevelSwitch Mixed_Log_Switch = new()
    {
        MinimumLevel = Serilog.Events.LogEventLevel.Information,
    };

    public static Logger ConsoleLogger { get; set; } = CreateConsoleLog();
    public static Logger FileLogger { get; set; } = CreateFileLog();
    public static Logger MixedLogger { get; set; } = CreateMixedLog();

    public static Logger CreateMixedLog()
    {
        var Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(Mixed_Log_Switch)
            .WriteTo.Console()
            .WriteTo.File("uplaykit_mixed_logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        return Logger;
    }

    public static Logger CreateConsoleLog()
    {
        var Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(Console_Log_Switch)
            .WriteTo.Console()
            .CreateLogger();
        return Logger;
    }

    public static Logger CreateFileLog()
    {
        var Logger = new LoggerConfiguration()
         .MinimumLevel.ControlledBy(File_Log_Switch)
         .WriteTo.File("uplaykit_logs.txt", rollingInterval: RollingInterval.Day)
         .CreateLogger();
        return Logger;
    }
}
