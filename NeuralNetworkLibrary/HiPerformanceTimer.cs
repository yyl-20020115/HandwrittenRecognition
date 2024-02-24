using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace NeuralNetworkLibrary;

public class HiPerformanceTimer
{
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(
        out long lpPerformanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(
        out long lpFrequency);

    private long StartTime, StopTime;
    private readonly long freq;

    public bool Started { get; private set; }

    public bool Stoped { get; private set; }
    // Constructor

    public HiPerformanceTimer()
    {
        StartTime = 0;
        StopTime = 0;
        Started = false;
        Stoped = true;

        if (!QueryPerformanceFrequency(out freq))
        {
            // high-performance counter not supported

            throw new Win32Exception();
        }
    }

    // Start the timer

    public void Start()
    {
        // lets do the waiting threads there work

        Thread.Sleep(0);
        QueryPerformanceCounter(out StartTime);
        Started = true;
        Stoped = false;
    }

    // Stop the timer

    public void Stop()
    {
        QueryPerformanceCounter(out StopTime);
        Started = false;
        Stoped = true;
    }

    public override bool Equals(object obj) 
        => obj is HiPerformanceTimer timer &&
               freq == timer.freq;
    public override int GetHashCode() => freq.GetHashCode();
    // Returns the duration of the timer (in seconds)

    public double Duration => (StopTime - StartTime) / (double)freq;
}
