using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Threading.Tasks;

namespace FILDInducer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button _mainButton;
        private CancellationSignal _progressBarUpdaterCancellation;
        private ProgressBar _progressBar;
        private Task _progressBarUpdater;
        private TimeoutManager _timeoutManager = new TimeoutManager(TimeSpan.FromSeconds(5));

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Init();
        }

        private void Init()
        {
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            _timeoutManager.TimeoutReached += OnTimeoutReached;

            var layout = FindViewById<CoordinatorLayout>(Resource.Id.coordinatorLayout);
            layout.Touch += Layout_Touch;

            _mainButton = FindViewById<Button>(Resource.Id.mainButton);
            _mainButton.Click += OnMainButtonTouch;
        }

        private void OnMainButtonTouch(object sender, EventArgs e)
        {
            if (_timeoutManager.Running)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void Stop()
        {
            _progressBarUpdaterCancellation.Cancel();
            _mainButton.Text = "Start";
            _timeoutManager.Stop();
            _progressBar.Visibility = ViewStates.Invisible;
        }

        private void Start()
        {
            _progressBarUpdaterCancellation = new CancellationSignal();
            _mainButton.Text = "Stop";
            _timeoutManager.Start();
            _progressBar.Visibility = ViewStates.Visible;

            _progressBarUpdater = ProgressBarUpdater(_progressBarUpdaterCancellation);
            _progressBarUpdater.ConfigureAwait(true);
        }

        private void Layout_Touch(object sender, View.TouchEventArgs e)
        {
            // Restart the timeout manager.
            _timeoutManager.Stop();
            _timeoutManager.Start();
        }

        private void OnTimeoutReached(object sender, EventArgs e)
        {
            Stop();
            // Vibrate here.
        }

        private async Task ProgressBarUpdater(CancellationSignal cancellationSignal)
        {
            while (_timeoutManager.Running || !cancellationSignal.IsCanceled)
            {
                _progressBar.Progress = (int)_timeoutManager.ProgressPercentage;
                await Task.Delay(10);
            }
        }
    }
}