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
using Java.Util;
using Android.Gms.Tasks;
using Java.Lang;
using RidersApp.EventListeners;

namespace RidersApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/UberTheme", MainLauncher = false)]
    public class RegisterationActivity : AppCompatActivity
    {
        TextInputLayout fullNameText;
        TextInputLayout phoneText;
        TextInputLayout emailText;
        TextInputLayout passwordText;
        Button registerButton;
        CoordinatorLayout rootView;
        TextView clickToLoginText;

        TaskCompletionListener TaskCompletionListener = new TaskCompletionListener();
        string fullname, phone, email, password;

        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register);

            ConnectControl();
        }

        void ConnectControl()
        {
            fullNameText = (TextInputLayout)FindViewById(Resource.Id.fullNameText);
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            registerButton = (Button)FindViewById(Resource.Id.registerButton);
            clickToLoginText = (TextView)FindViewById(Resource.Id.clickToLogin);

            clickToLoginText.Click += ClickToLoginText_Click;
            registerButton.Click += RegisterButton_Click;
        }

        private void ClickToLoginText_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
            Finish();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {


            fullname = fullNameText.EditText.Text;
            phone = phoneText.EditText.Text;
            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;

            if (fullname.Length < 3)
            {
                Snackbar.Make(rootView, "Please enter a valid name", Snackbar.LengthShort).Show();
                return;
            }
            else if (phone.Length < 9)
            {
                Snackbar.Make(rootView, "Please enter a valid phone number", Snackbar.LengthShort).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Snackbar.Make(rootView, "Please enter a valid email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootView, "Please enter a password upto 8 characters", Snackbar.LengthShort).Show();
                return;
            }
            RegisterUser(fullname, phone, email, password);
        }


        void RegisterUser(string name, string phone, string email, string password)
        {
            if (!DisruptionLibraries.UltraWebClient.IsConnected())
            {
                Snackbar.Make(rootView, "No internet connection", Snackbar.LengthShort).Show();
                return;
            }

            TaskCompletionListener.Success += TaskCompletionListener_Success;
            TaskCompletionListener.Failure += TaskCompletionListener_Failure;
            try
            {
                Helpers.AppDataHelper.GetFirebaseAuth().CreateUserWithEmailAndPassword(email, password)
                    .AddOnSuccessListener(this, TaskCompletionListener)
                    .AddOnFailureListener(this, TaskCompletionListener);
            }
            catch (System.Exception e)
            {
                Logger.Log(e.Message, " User Registration  ");
            }
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "User Registration failed", Snackbar.LengthShort).Show();
            Logger.Log(" User Registration failed", "Failure");
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            Logger.Log( " User Registration  Succeeded ", "Success");

            Snackbar.Make(rootView, "User Registration was Successful", Snackbar.LengthShort).Show();

            HashMap userMap = new HashMap();
            userMap.Put("email", email);
            userMap.Put("phone", phone);
            userMap.Put("fullname", fullname);

            Firebase.Database.DatabaseReference userReference = Helpers.AppDataHelper.GetDatabase().GetReference("users/" + Helpers.AppDataHelper.GetCurrentUser().Uid);
            userReference.SetValue(userMap);
            StartActivity(typeof(MainActivity));
        }

        void SaveToSharedPreference()
        {
            editor = preferences.Edit();
            editor.PutString("email", email);
            editor.PutString("fullname", fullname);
            editor.PutString("phone", phone);
            editor.Apply();
        }

        void RetriveData()
        {
            string email = preferences.GetString("email", "");
        }
    }

}
