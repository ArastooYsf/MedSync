using Avalonia;
using System;
using System.IO;

namespace MedSync;

class Program
{
    
    [STAThread]
    public static void Main(string[] args)
    {
        
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            File.WriteAllText("crash.log", e.ExceptionObject.ToString());
        };
        
        var flagPath = Path.Combine(Directory.GetCurrentDirectory(), "reset.flag");

        if (File.Exists(flagPath))
        {
            File.Delete(flagPath);

            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "medsync.db");

            if (File.Exists(dbPath))
                File.Delete(dbPath);

            if (File.Exists(dbPath + "-shm"))
                File.Delete(dbPath + "-shm");

            if (File.Exists(dbPath + "-wal"))
                File.Delete(dbPath + "-wal");
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}