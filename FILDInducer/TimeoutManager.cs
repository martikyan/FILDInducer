using System;
using System.Diagnostics;
using System.Timers;

namespace FILDInducer
{
    internal sealed class TimeoutManager
    {
        public TimeSpan Timeout { get; }
        public bool Running { get; private set; } = false;
        public float ProgressPercentage => (float)(100.0 * _stopwatch.ElapsedMilliseconds / Timeout.TotalMilliseconds);

        public event EventHandler TimeoutReached;

        private readonly Timer _timer = new Timer();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TimeoutManager(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                throw new ArgumentNullException(nameof(timeout));
            }

            Timeout = timeout;
            _timer.AutoReset = false;
            _timer.Elapsed += TimeoutReachedHandler;
        }

        public void Start()
        {
            _timer.Interval = Timeout.TotalMilliseconds;
            _timer.Start();

            _stopwatch.Start();
            Running = true;
        }

        public void Stop()
        {
            _timer.Stop();
            _stopwatch.Reset();
            Running = false;
        }

        private void TimeoutReachedHandler(object sender, ElapsedEventArgs e)
        {
            Stop();
            TimeoutReached?.Invoke(sender, e);
        }
    }
}