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
	public class SideTaskService : Service {
		SideTaskServiceBinder binder;

		public SideTaskService() {
			
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
			Log.Debug("SideTaskService", "SideTaskService started");

			DoWork();
			
			//return base.OnStartCommand(intent, flags, startId);
			return StartCommandResult.Sticky;
		}

		public override void OnDestroy() {
			base.OnDestroy();

			Log.Debug("SideTaskService", "SideTaskService destroyed");
		}

		public override IBinder OnBind(Intent intent) {
			binder = new SideTaskServiceBinder(this);
			return binder;
		}

		private void DoWork() {
			var t = new Thread(() => {
				//the work
				Log.Debug("work", "Doing work");

				int time = 0;
				int duration = 5000;
				int interval = 1000;

				while (time < duration) {
					Thread.Sleep(interval);
					time += interval;
					Log.Debug("time", (time/interval).ToString() + " seconds");

				}

				
				Log.Debug("work", "Work complete");
				//kill process once work is complete
				StopSelf();
			});
			//start doing the work
			t.Start();
		}
	}

	//Binder for the side task service
	public class SideTaskServiceBinder : Binder {
		SideTaskService service;

		public SideTaskServiceBinder(SideTaskService service) {
			this.service = service;
		}

		public SideTaskService GetSideTaskService() {
			return service;
		}
	}
}