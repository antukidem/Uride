using Firebase;
using Android.App;
using Firebase.Auth;
using Android.Content;
using Firebase.Database;

namespace RidersApp.Helpers
{
    public static class AppDataHelper
    {
        static ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);

        private static string APPLICATION_ID = Application.Context.Resources.GetString(Resource.String.ApplicationId);
        private static string API_KEY = Application.Context.Resources.GetString(Resource.String.DatabaseApiKey);
        private static string DATABASE_URL = Application.Context.Resources.GetString(Resource.String.DatabaseUrl);
        private static string STORAGE_BUCKET = Application.Context.Resources.GetString(Resource.String.StorageBucket); 

        static FirebaseApp mApp;
        static FirebaseAuth mAuth;
        static FirebaseDatabase mDatabase;
        static FirebaseUser mCurrentUser;

        static FirebaseApp InitializeFirebase()
        {
            mApp = FirebaseApp.InitializeApp(Application.Context);
            if (mApp == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApplicationId(APPLICATION_ID).SetApiKey(API_KEY)
                    .SetStorageBucket(STORAGE_BUCKET).SetDatabaseUrl(DATABASE_URL)
                    .Build();
                mApp = FirebaseApp.InitializeApp(Application.Context, options);
            }
            mAuth = FirebaseAuth.Instance;
            mCurrentUser = mAuth.CurrentUser;
            mDatabase = FirebaseDatabase.GetInstance(mApp);
            return mApp;
        }

        public static FirebaseAuth GetFirebaseAuth()
        {
            if (mAuth == null)
            {
                InitializeFirebase();
                mAuth = FirebaseAuth.Instance;
            }
            return mAuth;
        }

        public static FirebaseDatabase GetDatabase()
        {
            if (mDatabase == null)
            {
                if (mApp == null)
                {
                    mApp = InitializeFirebase();
                    mDatabase = FirebaseDatabase.GetInstance(mApp);
                }
                else
                {
                    mDatabase = FirebaseDatabase.GetInstance(mApp);
                }
            }
            return mDatabase;
        }

        public static FirebaseUser GetCurrentUser()
        {
            if (mAuth == null)
            {
                if (mApp == null)
                {
                    mApp = InitializeFirebase();
                    mAuth = FirebaseAuth.Instance;
                }
                else
                {
                    mAuth = FirebaseAuth.Instance;
                }
            }
            mCurrentUser = mAuth.CurrentUser;
            return mCurrentUser;
        }

        public static string GetFullName()
        {
            string fullname = preferences.GetString("fullname", "");
            return fullname;
        }

        public static string GetEmail()
        {
            string email = preferences.GetString("email", "");
            return email;
        }

        public static string GetPhone()
        {
            string phone = preferences.GetString("phone", "");
            return phone;
        }
    }
}