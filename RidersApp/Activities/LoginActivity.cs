using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget; 
using RidersApp.EventListeners;

namespace RidersApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class LoginActivity : AppCompatActivity
    {
        TextInputLayout emailText;
        TextInputLayout passwordText;
        TextView clickToRegisterText;
        Button loginButton;
        CoordinatorLayout rootView;
 
        Android.Support.V7.App.AlertDialog.Builder alert;
        Android.Support.V7.App.AlertDialog alertDialog; 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);

            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            clickToRegisterText = (TextView)FindViewById(Resource.Id.clickToRegisterText);

            clickToRegisterText.Click += ClickToRegisterText_Click;
            loginButton.Click += LoginButton_Click; 
         }

        private void LoginButton_Click(object sender, EventArgs e)
        { 
            if (!DisruptionLibraries.UltraWebClient.IsConnected())
            {
                Snackbar.Make(rootView, "No internet connection", Snackbar.LengthLong).Show();
                StartActivity(typeof(MainActivity));
            }

            string email, password;

            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;

            if (!email.Contains("@"))
            {
                Snackbar.Make(rootView, "Please provide a valid email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootView, "Please provide a valid password", Snackbar.LengthShort).Show();
                return;
            }

            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            ShowProgressDialogue();

            //consider having different authentication  providers.  
            //use strategy pattern to decouple the current code 
            Helpers.AppDataHelper.GetFirebaseAuth().SignInWithEmailAndPassword(email, password).AddOnSuccessListener(taskCompletionListener).AddOnFailureListener(taskCompletionListener);
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            StartActivity(typeof(MainActivity));
        }
        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            StartActivity(typeof(MainActivity));
            //Snackbar.Make(rootView, "User Registration failed", Snackbar.LengthShort).Show();

            //Logger.Log(e.ToString(), "LOGIN FAILED");
        }

        private void ClickToRegisterText_Click(object sender, EventArgs e)
        {
              StartActivity(typeof(RegisterationActivity));
        } 
 
        void ShowProgressDialogue()
        {
            alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);
            alertDialog = alert.Show();
        }

        void CloseProgressDialogue()
        {
            if (alert != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }

    }
}