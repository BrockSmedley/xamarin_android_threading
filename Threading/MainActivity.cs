using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace Threading {
	[Activity(Label = "Threading", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity {
		bool isBound = false;
		CounterServiceBinder binder;
		CounterServiceConnection counterServiceConnection;
		TimeReceiver timeReceiver;
		Intent counterServiceIntent;


		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			//set up service vars
			counterServiceIntent = new Intent("Threading.Threading.CounterService");
			timeReceiver = new TimeReceiver();

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.MyButton);

			button.Click += delegate { 
				// run task on another thread
				GetTime();
			};

		}

		protected override void OnStart() {
			base.OnStart();

			var intentFilter = new IntentFilter(CounterService.TimeUpdatedAction) { Priority = (int)IntentFilterPriority.HighPriority };
			RegisterReceiver(timeReceiver, intentFilter);

			counterServiceConnection = new CounterServiceConnection(this);
			BindService(counterServiceIntent, counterServiceConnection, Bind.AutoCreate);

			//schedule updates...?
		}

		protected override void OnStop() {
			base.OnStop();

			if (isBound) {
				UnbindService(counterServiceConnection);
				isBound = false;
			}
		}

		void GetTime() {
			if (isBound) {
				RunOnUiThread(() => {
					//get time from service
					var time = binder.GetCounterService().GetTime();
					
					if (time != null) {
						TextView timerTextView = FindViewById<TextView>(Resource.Id.textView1);
						timerTextView.SetText(time.ToString(), TextView.BufferType.Normal);
					}
					else {
						Log.Debug("time", "time is null in GetTime()");
					}
				});
			}
		}


		class TimeReceiver : BroadcastReceiver {
			public override void OnReceive(Context context, Intent intent) {
				//when message is received, update the time
				((MainActivity)context).GetTime();

				InvokeAbortBroadcast();
			}
		}

		class CounterServiceConnection : Java.Lang.Object, IServiceConnection {
			MainActivity activity;

			public CounterServiceConnection(MainActivity activity) {
				this.activity = activity;
			}

			public void OnServiceConnected(ComponentName name, IBinder service) {
				var counterServiceBinder = service as CounterServiceBinder;
				if (counterServiceBinder != null) {
					var binder = (CounterServiceBinder)service;
					activity.binder = binder;
					activity.isBound = true;
				}
			}

			public void OnServiceDisconnected(ComponentName name) {
				activity.isBound = false;
			}
		}
	}//end activity class
}//end namespace

