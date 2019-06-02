using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FILDInducer
{
    internal sealed class TimeoutManager
    {
        public TimeSpan Timeout { get; }
        public bool Running { get; private set; } = true;
        public float ProgressPercentage => (float) (100.0 * _stopwatch.ElapsedMilliseconds / Timeout.TotalMilliseconds);
        public event EventHandler TimeoutReached;

        private readonly Timer _timer;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TimeoutManager(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                throw new ArgumentNullException(nameof(timeout));
            }

            Timeout = timeout;
            _timer = new Timer(Timeout.TotalMilliseconds)
            {
                AutoReset = false,
            };
            _stopwatch.Start();
            _timer.Elapsed += TimeoutReachedHandler;
        }

        public void Restart()
        {
            _timer.Stop();
            _timer.Start();
            _stopwatch.Restart();
        }

        private void TimeoutReachedHandler(object sender, ElapsedEventArgs e)
        {
            Running = false;
            _stopwatch.Stop();
            _timer.Stop();
            TimeoutReached?.Invoke(sender, e);
        }
    }
}