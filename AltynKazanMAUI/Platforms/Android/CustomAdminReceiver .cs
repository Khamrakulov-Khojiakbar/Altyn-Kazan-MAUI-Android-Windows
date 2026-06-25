using Android.App;
using Android.App.Admin;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltynKazanMAUI.Platforms.Android
{
    [BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", Exported = true)]
    public class CustomAdminReceiver : DeviceAdminReceiver
    {
        public override void OnEnabled(Context context, Intent intent)
        {
            base.OnEnabled(context, intent);
        }
    }
}
