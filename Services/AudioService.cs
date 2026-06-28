using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MedSync.Services
{
    public class AudioService
    {
        private readonly string _audioBasePath;

        public AudioService()
        {
            _audioBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Audios", "Notifications");
        }

        public async Task PlayNotificationSoundAsync(string? soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName))
                return;

            try
            {
                var fullPath = Path.Combine(_audioBasePath, soundFileName);
                
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[AudioService] Sound file not found: {fullPath}");
                    return;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    await PlayWindowsSoundAsync(fullPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    await PlayLinuxSoundAsync(fullPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    await PlayMacSoundAsync(fullPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AudioService] Error playing sound: {ex.Message}");
            }
        }

        private async Task PlayWindowsSoundAsync(string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Using System.Media.SoundPlayer for .wav files
                    using var player = new System.Media.SoundPlayer(filePath);
                    player.PlaySync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AudioService] Windows playback error: {ex.Message}");
                }
            });
        }

        private async Task PlayLinuxSoundAsync(string filePath)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // Try paplay (PulseAudio)
                    var paplayResult = await RunCommandAsync("paplay", $"\"{filePath}\"");
                    if (paplayResult) return;

                    // Fallback: aplay (ALSA)
                    var aplayResult = await RunCommandAsync("aplay", $"\"{filePath}\"");
                    if (aplayResult) return;

                    Console.WriteLine("[AudioService] No audio player found on Linux (paplay/aplay)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AudioService] Linux playback error: {ex.Message}");
                }
            });
        }

        private async Task PlayMacSoundAsync(string filePath)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await RunCommandAsync("afplay", $"\"{filePath}\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AudioService] macOS playback error: {ex.Message}");
                }
            });
        }

        private async Task<bool> RunCommandAsync(string command, string arguments)
        {
            try
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = System.Diagnostics.Process.Start(processInfo);
                if (process == null) return false;

                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
