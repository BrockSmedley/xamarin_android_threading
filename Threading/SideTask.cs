using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;


namespace Threading {
	[Service]
	[IntentFilter(new String[] { "Threading.Threading.CounterService" })]
	public class CounterService : IntentService {
		IBinder binder;
		int time = 12;
		public const string TimeUpdatedAction = "TimeUpdated";

		protected override void OnHandleIntent(Intent intent) {
			var timeIntent = new Intent(TimeUpdatedAction);
			StartCount(timeIntent);
		}

		//counts and sends an OrderedBroadcast every second
		private void StartCount(Intent timeIntent) {
				Log.Debug("work", "Counting");

				int duration = 5000;
				time = duration;
				int interval = 1000;


				while (time > 0) {
					Thread.Sleep(interval);
					Log.Debug("time", (time / interval).ToString() + " seconds");
					time -= interval;

					SendBroadcast(timeIntent, null);
				}

				Log.Debug("work", "Work complete");			
		}

		public int GetTime() {
			OnHandleIntent(new Intent(TimeUpdatedAction));
			return time;
		}

		public override IBinder OnBind(Intent intent) {
			binder = new CounterServiceBinder(this);
			return binder;
		}		
	}//end class

	public class CounterServiceBinder : Binder {
		CounterService service;

		public CounterServiceBinder(CounterService service) {
			this.service = service;
		}

		public CounterService GetCounterService() {
			return service;
		}
	}//end class
}//end namespace