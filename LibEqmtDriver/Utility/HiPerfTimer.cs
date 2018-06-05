using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System;

namespace LibEqmtDriver.Utility
{
    public class HiPerfTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long _startTime, _stopTime;

        private readonly long _freq;

        public HiPerfTimer()
        {
            _startTime = 0;
            _stopTime = 0;

            if (QueryPerformanceFrequency(out _freq) == false)
            {
                // high-performance counter not supported

                throw new Win32Exception();
            }
        }

        public void Start()
        {
            QueryPerformanceCounter(out _startTime);
        }
        public void Stop()
        {
            QueryPerformanceCounter(out _stopTime);
        }

        public double Duration
        {
            get
            {
                return (double)(_stopTime - _startTime) / (double)_freq;
            }
        }

        public void Wait(double sleepMs)
        {
            long intstartTime, intstopTime;
            QueryPerformanceCounter(out intstartTime);
            float time;
            do
            {
                QueryPerformanceCounter(out intstopTime);
                time = ((float)(intstopTime - intstartTime) / (float)_freq) * 1000;
            } while (time < (float)sleepMs);
        }

        public void wait_us(double sleepUs)
        {
            long intstartTime, intstopTime;
            QueryPerformanceCounter(out intstartTime);
            float time;
            do
            {
                QueryPerformanceCounter(out intstopTime);
                time = ((float)(intstopTime - intstartTime) / (float)_freq) * 1000000;
            } while (time < (float)sleepUs);
        }
    }
}
