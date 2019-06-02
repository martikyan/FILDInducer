using Android.App;
using Android.Content;
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
        private ProgressBar _progressBar;
        private TimeoutManager _timeoutManager = new TimeoutManager(TimeSpan.FromSeconds(5));

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            _timeoutManager.TimeoutReached += _timeoutManager_TimeoutReached;

            var layout = FindViewById<CoordinatorLayout>(Resource.Id.coordinatorLayout);
            layout.Touch += Layout_Touch;

            Task.Run(ProgressBarUpdater);
        }

        private void Layout_Touch(object sender, View.TouchEventArgs e)
        {
            _timeoutManager.Restart();
        }

        private void _timeoutManager_TimeoutReached(object sender, EventArgs e)
        {
            var toast = Toast.MakeText(Application.Context, "Hello", ToastLength.Long);
            toast.Show();
        }

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

        private async Task ProgressBarUpdater()
        {
            while (_timeoutManager.Running)
            {
                _progressBar.Progress = (int) _timeoutManager.ProgressPercentage;
                await Task.Delay(10);
            }
        }
    }
}

