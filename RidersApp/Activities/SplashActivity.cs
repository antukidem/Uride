using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;

namespace RidersApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            Thread.Sleep(5000);
        }
        protected override void OnResume()
        {
            base.OnResume(); 
            //FirebaseUser currentUser = AppDataHelper.GetCurrentUser();
            //if (currentUser == null)
            //{
                StartActivity(typeof(LoginActivity));
            //}
            //else
            //{
              //  StartActivity(typeof(MainActivity));
            //}
        }

    }
}