using AuraServiceLib;
using System.Diagnostics;

const int DRAM = 0x00070000;
const int RED = 0x000000FF;
const int GREEN = 0x0000FF00;

var memoryCounter = new PerformanceCounter("Memory", "Available Bytes");
var totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
var sdk = new AuraSdk();
sdk.SwitchMode();

while (true)
{
    var value = (1 - (memoryCounter.NextValue() / totalMemory));
    var rounded = Math.Round(value * 8);
    foreach (IAuraSyncDevice device in sdk.Enumerate(DRAM))
    {
        var i = 8;
        foreach (IAuraRgbLight light in device.Lights)
        {
            if (i <= rounded)
            {
                light.Color = RED;
            } else
            {
                light.Color = GREEN;
            }
            i--;
        }
        device.Apply();
    }
    Thread.Sleep(1000);
}