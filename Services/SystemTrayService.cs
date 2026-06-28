using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MedSync.Services
{
    public class SystemTrayService
    {
        public async Task ShowNotificationAsync(string title, string message)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    await ShowWindowsNotificationAsync(title, message);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    await ShowLinuxNotificationAsync(title, message);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    await ShowMacNotificationAsync(title, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SystemTrayService] Error showing notification: {ex.Message}");
            }
        }

        private async Task ShowWindowsNotificationAsync(string title, string message)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Windows Toast Notification (requires Windows 10+)
                    var toastXml = $@"
                        <toast>
                            <visual>
                                <binding template='ToastText02'>
                                    <text id='1'>{EscapeXml(title)}</text>
                                    <text id='2'>{EscapeXml(message)}</text>
                                </binding>
                            </visual>
                        </toast>";

                    // Note: Full implementation requires Windows.UI.Notifications
                    // For now, we'll use a simple console log
                    Console.WriteLine($"[Windows Notification] {title}: {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SystemTrayService] Windows notification error: {ex.Message}");
                }
            });
        }

        private async Task ShowLinuxNotificationAsync(string title, string message)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // Use notify-send (libnotify)
                    var escapedTitle = EscapeShellArgument(title);
                    var escapedMessage = EscapeShellArgument(message);
                    
                    var processInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "notify-send",
                        Arguments = $"{escapedTitle} {escapedMessage}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = System.Diagnostics.Process.Start(processInfo);
                    if (process != null)
                    {
                        await process.WaitForExitAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SystemTrayService] Linux notification error: {ex.Message}");
                }
            });
        }

        private async Task ShowMacNotificationAsync(string title, string message)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // Use osascript (AppleScript)
                    var escapedTitle = EscapeShellArgument(title);
                    var escapedMessage = EscapeShellArgument(message);
                    
                    var script = $"display notification {escapedMessage} with title {escapedTitle}";
                    
                    var processInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "osascript",
                        Arguments = $"-e \"{script}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = System.Diagnostics.Process.Start(processInfo);
                    if (process != null)
                    {
                        await process.WaitForExitAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SystemTrayService] macOS notification error: {ex.Message}");
                }
            });
        }

        private string EscapeXml(string text)
        {
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        private string EscapeShellArgument(string argument)
        {
            return $"\"{argument.Replace("\"", "\\\"")}\"";
        }
    }
}
