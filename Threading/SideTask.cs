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
	[IntentFilter(new String[] { "Threading.Threading.SideTaskService" })]
	public class SideTaskService : Service {
		SideTaskServiceBinder binder;

		public SideTaskService() {
			
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
			Log.Debug("SideTaskService", "SideTaskService started");
			
			StartServiceInForeground();

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

		private void StartServiceInForeground() {
			var ongoing = new Notification(Resource.Drawable.Icon, "SideTaskService in foreground");
			var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);
			ongoing.SetLatestEventInfo(this, "SideTaskService", "SideTaskService running in foreground", pendingIntent);

			StartForeground((int)NotificationFlags.ForegroundService, ongoing);
		}

		private void DoWork() {
			var t = new Thread(() => {
				//the work
				Log.Debug("work", "Doing work");
				Thread.Sleep(5000);
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