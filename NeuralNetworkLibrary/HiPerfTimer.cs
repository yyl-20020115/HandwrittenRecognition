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
    private long freq;

    private bool _bStarted;
    public bool BStarted => _bStarted;

    public bool BStoped { get; private set; }
    // Constructor

    public HiPerfTimer()
    {
        startTime = 0;
        stopTime = 0;
        _bStarted = false;
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
        _bStarted = true;
        BStoped = false;
    }

    // Stop the timer

    public void Stop()
    {
        QueryPerformanceCounter(out stopTime);
        _bStarted = false;
        BStoped = true;
    }

    // Returns the duration of the timer (in seconds)

    public double Duration => (stopTime - startTime) / (double)freq;
}
