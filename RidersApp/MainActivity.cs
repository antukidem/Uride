using System;
using Android;
using Android.OS;
using System.Linq;
using Android.App;
using System.Text;
using Android.Views;
using Google.Places;
using Android.Widget;
using Android.Runtime;
using Android.Content;
using Android.Gms.Maps;
using Android.Locations;
using Android.Telephony;
using Android.Graphics;
using Android.Content.PM;
using Android.Gms.Maps.Model;
using Android.Support.V4.App;
using Android.Support.V7.App;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using Disruption;
using RidersApp.Helpers;

//using Android.Gms.Location;
//using Android.Support.Design.Widget;
//using Android.Media; 
namespace RidersApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback //, ILocationListener
    {
        //Initialization 
        private GoogleMap mMap;
        List<Place.Field> fields;
        MapFunctionHelper mapHelper;
        readonly string[] permissionGroupLocation = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        const int requestLocationId = 0;

        SharedPartnerDataModels.PlacesBounds placeBounds = new SharedPartnerDataModels.PlacesBounds();

        //Trip Detail  
        LatLng pickUpLocationLatLng;
        LatLng destinationLocationLatLng;

        //private string locationProvider;
        private Location currentLocation;
        private Location lastKnownGpsLocation;

        Android.Support.V7.Widget.Toolbar mainToolbar;
        Android.Support.V4.Widget.DrawerLayout drawerLayout;

        #region  Location Layout Refrence
        RelativeLayout layoutPickUp;
        RelativeLayout layoutDestination;
        #endregion

        #region  Location TextView Refrences
        TextView PickUpLocationText;
        TextView DestinationLocationText;
        #endregion

        #region  Radio TextView Refrences
        RadioButton PickUpRadio;
        RadioButton DestinationRadio;
        #endregion

        #region Location Request Parameters
        Android.Gms.Location.LocationRequest locationRequest;
        Android.Gms.Location.FusedLocationProviderClient locationClient;

        LocationCallbackHelper mLocationCallback;

        static int UPDATE_INTERVAL = 5;// 5 seconds
        static int FASTEST_INTERVAL = 5;
        static int DISPLACEMENT = 3; // 3 meters
        #endregion

        //Flags
        int addressRequest = 1;
        bool takeAddressFromSearch = false;

        //image view
        //ImageView centerMarker;
        CoordinatorLayout rootView;

        public MainActivity()
        {
        } 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            try
            {
                CheckAndInitializePlacesAPI();
                ConnectControl();
                MapSetUpMap(); //Setup map
                CheckLocationPermission();
                CreateLocationRequest();
                GetMyLocation();
                StartLocationUpdates();
                SendSMS();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, "EXCEPTION");
            }
        }
        //check and initialize places API key
        private void CheckAndInitializePlacesAPI()
        {
            if (!PlacesApi.IsInitialized)
            {
                PlacesApi.Initialize(this, Application.Context.Resources.GetString(Resource.String.PlacesAPIKey));
            }
            fields = new List<Place.Field>();
            fields.Add(Place.Field.Id); fields.Add(Place.Field.Name); fields.Add(Place.Field.LatLng); fields.Add(Place.Field.Address);
        }

        private string GetDevicePhoneNumber()
        {
            TelephonyManager mManager = (TelephonyManager)GetSystemService(Context.TelephonyService);
            return mManager.Line1Number;
        }

        async void SendSMS()
        {
            // List<string> recipients = new List<string>();
            //recipients.Add("08033802239");

            // await UiServices.SendSms("This is my test SMS", recipients);
            //Intent sInt = new Intent(Intent.ActionView);
            //sInt.PutExtra("address", new String[] { GetDevicePhoneNumber() });
            //sInt.PutExtra("sms_body", "This is my test SMS");
            //sInt.SetType("vnd.android-dir/mms-sms");

            //StartActivity(sInt);
        }

        #region connect control

        void ConnectControl()
        {
            //Image View
            //centerMarker = (ImageView)FindViewById(Resource.Id.centerMarker);

            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            drawerLayout = (Android.Support.V4.Widget.DrawerLayout)FindViewById(Resource.Id.drawerLayout);
            mainToolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.mainToolbar);

            //TextView
            PickUpLocationText = (TextView)FindViewById(Resource.Id.pickUpLocationText);
            DestinationLocationText = (TextView)FindViewById(Resource.Id.destinationLocationText);

            //Radio buttons
            PickUpRadio = (RadioButton)FindViewById(Resource.Id.pickUpRadio);
            DestinationRadio = (RadioButton)FindViewById(Resource.Id.destinationRadio);

            PickUpRadio.Click += PickUpRadio_Click;
            DestinationRadio.Click += DestinationRadio_Click;

            //Layouts
            layoutPickUp = (RelativeLayout)FindViewById(Resource.Id.layoutPickUp);
            layoutDestination = (RelativeLayout)FindViewById(Resource.Id.layoutDestination);

            //attache events
            layoutPickUp.Click += layoutPickUp_Click;
            layoutDestination.Click += layoutDestination_Click;

            SetSupportActionBar(mainToolbar);
            SupportActionBar.Title = "";
            Android.Support.V7.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_menu_action);
            actionBar.SetDisplayHomeAsUpEnabled(true);
        }
        #endregion

        #region Event Handlers
        private void PickUpRadio_Click(object sender, EventArgs e)
        {
            addressRequest = 1;
            PickUpRadio.Checked = true;
            DestinationRadio.Checked = false;
            takeAddressFromSearch = false;
            //centerMarker.SetColorFilter(Color.DarkGreen);
        }

        private void DestinationRadio_Click(object sender, EventArgs e)
        {
            addressRequest = 2;
            PickUpRadio.Checked = false;
            DestinationRadio.Checked = true;
            takeAddressFromSearch = false;
            //centerMarker.SetColorFilter(Color.Red);
        }

        private void layoutPickUp_Click(object sender, EventArgs e)
        {
            //Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields).Build(this);
            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields).SetCountry("NG").Build(this);
            //Intent intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeOverlay).Build(this);
            StartActivityForResult(intent, 1);
        }
        private void layoutDestination_Click(object sender, EventArgs e)
        {
            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields).SetCountry("NG").Build(this);
            //Intent intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeOverlay).Build(this);
            StartActivityForResult(intent, 2);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Processing intent datat with option " + requestCode + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            try
            {
                if (requestCode == 1)
                {
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>> Processing Pickup <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    if (resultCode == Android.App.Result.Ok)
                    {
                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>> Checking result " + resultCode + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        takeAddressFromSearch = true;
                        PickUpRadio.Checked = false;
                        DestinationRadio.Checked = false;

                        var place = Autocomplete.GetPlaceFromIntent(data);
                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>> Place IDs " + placeBounds.destination.Id + " = " + place.Id + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>> Place Name  = " + place.Name + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                        if (placeBounds.pickUp.Id == place.Id)
                        {
                            PickUpLocationText.Text = place.Address;
                            placeBounds.pickUp = new SharedPartnerDataModels.LocationDetail(place.Id, place.Name, place.Address, GetStateFromGooglePlace(place),
                            new SharedPartnerDataModels.Location(place.LatLng.Latitude, place.LatLng.Longitude));

                            MoveCameraToDestination(mMap, place.LatLng, 15);
                            //centerMarker.SetColorFilter(Color.DarkGreen);
                            placeBounds.lineDrawn = false;
                        }
                    }
                }

                if (requestCode == 2)
                {
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>> Processing Pickup <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    if (resultCode == Android.App.Result.Ok)
                    {
                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>> Checking result " + resultCode + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        takeAddressFromSearch = true;
                        PickUpRadio.Checked = false;
                        DestinationRadio.Checked = false;

                        var place = Autocomplete.GetPlaceFromIntent(data);
                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>> Place IDs " + placeBounds.destination.Id + " = " + place.Id + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>> Place Name  = " + place.Name + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                        if (placeBounds.destination.Id == place.Id)
                        {
                            DestinationLocationText.Text = place.Name;
                            placeBounds.destination = new SharedPartnerDataModels.LocationDetail(place.Id, place.Name, place.Address, GetStateFromGooglePlace(place),
                                new SharedPartnerDataModels.Location(place.LatLng.Latitude, place.LatLng.Longitude));

                            MoveCameraToDestination(mMap, place.LatLng, 15);
                            //centerMarker.SetColorFilter(Color.Red);
                            placeBounds.lineDrawn = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Activity result error <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>   " + e.Message + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
            CheckIfSrcDestSatisfied(placeBounds);
        }

        string GetStateFromGooglePlace(Place pl)
        {
            var b = pl.AddressComponents != null ? pl.AddressComponents.AsList().FirstOrDefault(x => x.Types.Contains("administrative_area_level_1")).Name : GetStateFromAddress(pl.Address);
            return b;
        }
        string GetStateFromAddress(string address)
        {
            var addArray = address.Split(",");
            string result = addArray.Length > 3 ? addArray[addArray.Length - 2] : "";
            return result;
        }

        private void CheckIfSrcDestSatisfied(SharedPartnerDataModels.PlacesBounds sourceDestinationInfo)
        {
            if ((sourceDestinationInfo.pickUp.Id != string.Empty) && (sourceDestinationInfo.destination.Id != string.Empty))
            {
                SharedPartnerDataModels.Bounds2 bounds2 = new SharedPartnerDataModels.Bounds2();
                bounds2.northeastLocationName = sourceDestinationInfo.pickUp.Address;
                bounds2.northeast.lat = sourceDestinationInfo.pickUp.LatLng.lat;
                bounds2.northeast.lng = sourceDestinationInfo.pickUp.LatLng.lng;

                bounds2.southwestLocationName = sourceDestinationInfo.destination.Address;
                bounds2.southwest.lat = sourceDestinationInfo.destination.LatLng.lat;
                bounds2.southwest.lng = sourceDestinationInfo.destination.LatLng.lng;

                if (sourceDestinationInfo.pickUp.State == sourceDestinationInfo.destination.State)
                {
                    Logger.Log("METROPOLITAN TRIP", "TRIP DETECTION");
                }
                else
                {
                    Logger.Log("INTER STATE   TRIP", "TRIP DETECTION");
                }
                FnGetDirectionsData(bounds2);
                // await Task.Run(() => FnGetDirectionsData(bounds2));
            }
        }

        bool CheckLocationPermission()
        {
            bool permissionGranted = false;

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                permissionGranted = false;
                RequestPermissions(permissionGroupLocation, requestLocationId);
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (grantResults == null)
            {
                return;
            }
            if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
            {
                Toast.MakeText(this, "Permission was granted", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Permission was denied", ToastLength.Short).Show();
            }
        }
        #endregion

        void CreateLocationRequest()
        {
            locationRequest = new Android.Gms.Location.LocationRequest();
            locationRequest.SetInterval(UPDATE_INTERVAL);
            locationRequest.SetFastestInterval(FASTEST_INTERVAL);
            locationRequest.SetPriority(Android.Gms.Location.LocationRequest.PriorityHighAccuracy);
            locationRequest.SetSmallestDisplacement(DISPLACEMENT);
            locationClient = Android.Gms.Location.LocationServices.GetFusedLocationProviderClient(this);
            mLocationCallback = new LocationCallbackHelper();
            mLocationCallback.MyLocation += MLocationCallback_MyLocation;

            Logger.Log("Location mamager initialized. ", "locationProvider");
        }

        private void MLocationCallback_MyLocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            lastKnownGpsLocation = e.Location;
            LatLng myPosition = new LatLng(lastKnownGpsLocation.Latitude, lastKnownGpsLocation.Longitude);
            mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myPosition, 12));
            //MoveCameraToDestination(mMap, myPosition, 12);
        }

        void StartLocationUpdates()
        {
            if (!DisruptionLibraries.UltraWebClient.IsConnected())
            {
                Snackbar.Make(rootView, "No internet connection", Snackbar.LengthShort).Show();
                return;
            }
            //Check location permission
            if (CheckLocationPermission())
            {
                locationClient.RequestLocationUpdates(locationRequest, mLocationCallback, null);
            }
        }

        void StopLocationUpdates()
        {
            if (locationClient != null && mLocationCallback != null)
            {
                locationClient.RemoveLocationUpdates(mLocationCallback);
            }
        }

        async void GetMyLocation()
        {
            if (!DisruptionLibraries.UltraWebClient.IsConnected())
            {
                Snackbar.Make(rootView, "No internet connection", Snackbar.LengthShort).Show();
                return;
            }
            if (!CheckLocationPermission())
            {
                return;
            }
            Logger.Log("Starting Location Updates ", "Location Updates");

            lastKnownGpsLocation = await locationClient.GetLastLocationAsync();
            if (lastKnownGpsLocation != null)
            {
                LatLng myPosition = new LatLng(lastKnownGpsLocation.Latitude, lastKnownGpsLocation.Longitude);
                mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myPosition, 17));
                //MoveCameraToDestination(mMap, myPosition, 17);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void MapSetUpMap()
        {
            if (!DisruptionLibraries.UltraWebClient.IsConnected())
            {
                Snackbar.Make(rootView, "No internet connection", Snackbar.LengthShort).Show();
                return;
            }

            if (mMap == null)
            {
                SupportMapFragment mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
                mapFragment.GetMapAsync(this);
            }
        }

        private void mMap_CameraIdle(object sender, System.EventArgs e)
        {

            SharedPartnerDataModels.ReverseGeoCodeRootObject rootAddress = null;

            if (!takeAddressFromSearch)
            {
                if (addressRequest == 1)
                {
                    pickUpLocationLatLng = mMap.CameraPosition.Target;
                    try
                    {
                        rootAddress = (SharedPartnerDataModels.ReverseGeoCodeRootObject)mapHelper.FindCordinateAddress(pickUpLocationLatLng, true);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                    if (rootAddress != null)
                    {
                        if (rootAddress.results.Count == 0)
                            return;
                        PickUpLocationText.Text = rootAddress.results[0].formatted_address;

                        placeBounds.pickUp = new SharedPartnerDataModels.LocationDetail(rootAddress.results[0].place_id, rootAddress.results[0].formatted_address, rootAddress.results[0].formatted_address,
                        rootAddress.results[0].address_components.FirstOrDefault(x => x.types.Contains("administrative_area_level_1")).long_name,
                        rootAddress.results[0].geometry.location);
                        //new SharedGoogleDataModels.Location(pickUpLocationLatLng.Latitude, pickUpLocationLatLng.Longitude);
                    }

                }
                else if (addressRequest == 2)
                {
                    destinationLocationLatLng = mMap.CameraPosition.Target;
                    try
                    {
                        rootAddress = (SharedPartnerDataModels.ReverseGeoCodeRootObject)mapHelper.FindCordinateAddress(destinationLocationLatLng, true);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                    if ((rootAddress != null) && (rootAddress.results.Count > 0))
                    {
                        DestinationLocationText.Text = rootAddress.results[0].formatted_address; ;

                        placeBounds.destination = new SharedPartnerDataModels.LocationDetail(rootAddress.results[0].place_id, rootAddress.results[0].formatted_address, rootAddress.results[0].formatted_address,
                            rootAddress.results[0].address_components.FirstOrDefault(x => x.types.Contains("administrative_area_level_1")).long_name,
                        rootAddress.results[0].geometry.location);
                        //new SharedGoogleDataModels.Location(destinationLocationLatLng.Latitude, destinationLocationLatLng.Longitude));
                    }
                    // DestinationLocationText.Text = mapHelper.FindCordinateAddress(destinationLocationLatLng);
                }
            }
            CheckIfSrcDestSatisfied(placeBounds);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            mMap.CameraIdle += mMap_CameraIdle;
            string mapkey = Resources.GetString(Resource.String.APIkey);

            mMap.MapType = GoogleMap.MapTypeNormal;
            mMap.MyLocationEnabled = true;

            mapHelper = new MapFunctionHelper(mapkey, mMap);
        }

        private void UpdateUserLocationOnApp(Address addr)
        {
            // set camera to display  user location  
            if (addr != null)
            {
                //var addr = await new UiServices().ReverseGeoCode(this, currentLocation.Latitude, currentLocation.Longitude);//sourceLocationName =sourceLocationName ;
                settings.locationSource = addr.GetAddressLine(0);
                //txtMycurrentLocation.Text = string.Format("FROM : {0} ", settings.locationSource);  
                RunOnUiThread(() =>
                {
                    new MarkLocationOnMap(mMap, settings.locationSource, new LatLng(addr.Longitude, addr.Longitude));
                });
                MoveCameraToDestination(mMap, new LatLng(addr.Latitude, addr.Longitude));
                lastKnownGpsLocation = currentLocation;
            }
        }

        void FnGetDirectionsData(SharedPartnerDataModels.Bounds2 bounds)
        {
            SharedPartnerDataModels.GoogleDirectionClass directions = new UiServices().GetDirectionsData(bounds);

            if (directions == null)
                return;
            //objRoutes.routes.Count  --may be more then one 
            if (directions.routes.Count > 0)
            {
                string encodedPoints = directions.routes[0].overview_polyline.points;

                List<SharedPartnerDataModels.Location> lstDecodedPoints = new List<SharedPartnerDataModels.Location>();
                try
                {
                    lstDecodedPoints = UiServices.FnDecodePolylinePoints(encodedPoints);
                    //lstDecodedPoints = UiServices.locDecodePoly (encodedPoints);

                    //convert list of location point to array of latlng type
                    var latLngPoints = new LatLng[lstDecodedPoints.Count];
                    int index = 0;
                    foreach (SharedPartnerDataModels.Location loc in lstDecodedPoints)
                    {
                        latLngPoints[index++] = new LatLng(loc.lat, loc.lng);
                    }

                    //Draw Polylines on Map
                    PolylineOptions polylineOptions = new PolylineOptions().Add(latLngPoints).InvokeWidth(10).InvokeColor(Color.Teal)
                        .InvokeStartCap(new SquareCap()).InvokeEndCap(new SquareCap()).InvokeJointType(JointType.Round).Geodesic(true);

                    if (mMap != null)
                    {
                        mMap.Clear();
                        RunOnUiThread(() => mMap.AddPolyline(polylineOptions));

                        string snippet = "Duration :" + directions.routes.FirstOrDefault().legs.FirstOrDefault().duration.text;
                        snippet += "\n Distance :" + directions.routes.FirstOrDefault().legs.FirstOrDefault().distance.text;

                        RunOnUiThread(() =>
                        {
                            new MarkLocationOnMap(mMap, bounds.northeastLocationName, new LatLng(bounds.northeast.lat, bounds.northeast.lng));// Resource.Drawable.MarkerSource);
                            new MarkLocationOnMap(mMap, bounds.southwestLocationName, snippet, new LatLng(bounds.southwest.lat, bounds.southwest.lng), BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));// Resource.Drawable.MarkerDest);
                        });

                        PickUpRadio.Checked = false;
                        DestinationRadio.Checked = false;

                        //Get Trip Bounds
                        //LatLng southwest = new LatLng(bounds.southwest.lng, bounds.southwest.lng);
                        //LatLng northeast = new LatLng(bounds.northeast.lng, bounds.northeast.lng);
                        //LatLngBounds tripBound = new LatLngBounds(southwest, northeast);

                        //mMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripBound, 100));
                        //mMap.SetPadding(40, 40, 40, 40);
                    }
                    //var polylineOptions = new PolylineOptions().Visible(true).InvokeColor(Android.Graphics.Color.Red).Geodesic(true);
                    //polylineoption.InvokeWidth(5).InvokeZIndex(30).Add(latLngPoints); 
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.StackTrace,ex.Message);
 
                    RunOnUiThread(() =>
                      Toast.MakeText(this, "Please Wait....", ToastLength.Short).Show());
                }
            }
        }

        void MoveCameraToDestination(GoogleMap map, LatLng pos, int zoom = 12, int bearing = 45, int tilt = 10)
        {
            RunOnUiThread(() =>
            {
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(pos);
                builder.Zoom(zoom);
                builder.Bearing(bearing);
                builder.Tilt(tilt);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                map.AnimateCamera(cameraUpdate);
                //map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(pos, 10));
            });
        }
    }

    public static class settings
    {
        static settings()
        {
        }
        public static string locationSource { get; set; }
        public static string locationDestination { get; set; }
        public static string lastSuccessfullSearchQuery { get; set; }
    }

    class MarkLocationOnMap
    {
        public MarkLocationOnMap(GoogleMap mMap, string title, LatLng pos)
        {
            var marker = new MarkerOptions();
            marker.SetTitle(title);
            marker.SetPosition(pos);
            mMap.AddMarker(marker);
        }

        public MarkLocationOnMap(GoogleMap mMap, string title, string snippet, LatLng pos)
        {
            var marker = new MarkerOptions();
            marker.SetTitle(title);
            marker.SetSnippet(snippet);
            marker.Draggable(true);
            marker.SetPosition(pos);
            mMap.AddMarker(marker);
        }

        public MarkLocationOnMap(GoogleMap mMap, string title, string snippet, LatLng pos, BitmapDescriptor markerIcon = null)
        {
            var marker = new MarkerOptions();
            marker.SetTitle(title);
            marker.SetSnippet(snippet);
            marker.Draggable(true);
            marker.SetPosition(pos);
            marker.SetIcon(markerIcon);
            mMap.AddMarker(marker);
        }
    }

    public static class Logger
    {
        private static int mScreenWidth;
        private static char mChar;
        private static string mTitle;
        private static string mMessage;

        public static void Log(string message, string title = "", char defaultChar = '*', int screenWidth = 80)
        {
            mScreenWidth = screenWidth;
            mMessage = message;
            mTitle = title;
            mChar = defaultChar;

            using (IDisposable disposable = null)
            {
                Console.WriteLine(BuildLine(null, '_'));
                Console.WriteLine(BuildLine(title, '_', 80));
                Console.WriteLine(BuildLine(null, '_'));
                Console.WriteLine(message);
                Console.WriteLine(BuildLine(null));
            }
        }

        private static int GetMid(int limit)
        {
            return limit == 0 ? 0 : limit / 2;
        }

        private static int GetLimit(string text, int screenWidth)
        {
            if (text.Length > screenWidth) return 0;
            return Math.Abs(screenWidth - text.Length);
        }

        public static string BuildLine(string text, char defaultChar = '*', int screenWidth = 80)
        {
            try
            {
                if (text == null) { text = string.Empty; }
                StringBuilder lout = new StringBuilder();
                int limit = GetLimit(text, screenWidth);
                int mid = GetMid(limit);

                for (int a = 0; a <= limit; a++)
                {
                    if (a == mid) lout.Append(text);
                    lout.Append(defaultChar);
                }
                return lout.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while loggging this message "+ text); 
                return string.Empty;
            }
        }

    }
}