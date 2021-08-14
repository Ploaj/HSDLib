using System;
using System.Diagnostics;

namespace HSDRawViewer.Tools
{
    public class RawTimer
    {
        long count;
        const long nsfactor = 10000000L;

        internal RawTimer()
        {
            Reset();
        }

        public bool IsRunning
        {
            get
            {
                return isrunning;
            }
        }

        bool isrunning = false;
        internal long Frequency
        {
            get
            {
                return Stopwatch.Frequency;
            }
        }

        internal bool isHighResolution
        {
            get
            {
                return Stopwatch.IsHighResolution;
            }
        }

        internal static long Timestamp
        {
            get
            {
                return Stopwatch.GetTimestamp();
            }
        }

        internal TimeSpan Elapsed
        {
            get
            {
                return elapsed;
            }
        }
        TimeSpan elapsed;

        internal TimeSpan Total
        {
            get
            {
                return total;
            }
        }
        TimeSpan total; 
        
        internal void Start()
        {
            isrunning = true;
            count = Timestamp;
        }

        internal void Stop()
        {
            isrunning = false;
        }

        internal void Reset()
        {
            count = Timestamp;
            elapsed = TimeSpan.Zero;
            total = TimeSpan.Zero;
            isrunning = false;
        }

        internal void Restart()
        {
            Reset();
            Start();
        }

        internal void Step()
        {
            if (isrunning)
            {
                long last = count;
                count = Timestamp;
                long offset = count - last;
                elapsed = DeltaToTimeSpan(offset);
                total += elapsed;
            }
        }

        private TimeSpan DeltaToTimeSpan(long delta)
        {
            return TimeSpan.FromTicks((delta * nsfactor) / Frequency);
        }
    }
}
