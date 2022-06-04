using System.Diagnostics;

using AuraServiceLib;

namespace RamMonitor.WindowsService;
public class RamMonitorService
{
    const int DRAM = 0x00070000;
    const int RED = 0x000000FF;
    const int GREEN = 0x0000FF00;

    private readonly PerformanceCounter memoryCounter;
    private readonly long totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
    private readonly AuraSdk sdk;

    public RamMonitorService(ILogger<RamMonitorService> logger)
    {
        memoryCounter = new PerformanceCounter("Memory", "Available Bytes");
        totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        sdk = new AuraSdk();
        sdk.SwitchMode();
        logger.LogInformation("Ram Monitor Started");
    }

    /**
     * This function needs to be called every second.
     */
    public void UpdateLeds()
    {
        var value = Math.Round((1 - (memoryCounter.NextValue() / totalMemory)) * 8);
        foreach (IAuraSyncDevice device in sdk.Enumerate(DRAM))
        {
            var i = 8;
            foreach (IAuraRgbLight light in device.Lights)
            {
                if (i <= value)
                {
                    light.Color = RED;
                }
                else
                {
                    light.Color = GREEN;
                }
                i--;
            }
            device.Apply();
        }
    }
}

public record Joke(string Setup, string Punchline);