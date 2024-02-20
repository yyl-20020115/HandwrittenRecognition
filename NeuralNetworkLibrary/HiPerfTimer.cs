using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace NeuralNetworkLibrary;

public class HiPerfTimer
{
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(
        out long lpPerformanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(
        out long lpFrequency);

    private long startTime, stopTime;
    private readonly long freq;

    public bool BStarted { get; private set; }

    public bool BStoped { get; private set; }
    // Constructor

    public HiPerfTimer()
    {
        startTime = 0;
        stopTime = 0;
        BStarted = false;
        BStoped = true;

        if (QueryPerformanceFrequency(out freq) == false)
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
        QueryPerformanceCounter(out startTime);
        BStarted = true;
        BStoped = false;
    }

    // Stop the timer

    public void Stop()
    {
        QueryPerformanceCounter(out stopTime);
        BStarted = false;
        BStoped = true;
    }

    public override bool Equals(object obj) 
        => obj is HiPerfTimer timer &&
               freq == timer.freq;
    public override int GetHashCode() => freq.GetHashCode();
    // Returns the duration of the timer (in seconds)

    public double Duration => (stopTime - startTime) / (double)freq;
}
