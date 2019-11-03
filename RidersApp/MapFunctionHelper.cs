using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Disruption
{
    class MapFunctionHelper
    {
        string mapKey;
        Android.Gms.Maps.GoogleMap map;
        string mapkey;
        public double distance;
        public double duration;
        public string distanceString;
        public string durationstring;
        Marker pickupMarker;
        Marker driverLocationMarker;
        bool isRequestingDirection;

        public MapFunctionHelper(string mapKey)
        {
            this.mapKey = mapKey;
        }

        public MapFunctionHelper(string mMapkey, Android.Gms.Maps.GoogleMap mMap)
        {
            this.mapKey = mMapkey;
            this.map = mMap;
        }

        public object FindCordinateAddress(Android.Gms.Maps.Model.LatLng position, bool returnFullAddress=false)
        {
            var result= UiServices.ReverseGeoCode(position.Latitude.ToString() + "," + position.Longitude.ToString(),returnFullAddress);
            return result;
        } 

        public string GetGeoCodeUrl(double lat, double lng)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&key=" + mapkey;
            return url;
        }

        public async Task<string> GetGeoJsonAsync(string url)
        {
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        public async Task<string> FindCordinateAddressAsync(LatLng position)
        {
            string url = GetGeoCodeUrl(position.Latitude, position.Longitude);
            string json = "";
            string placeAddress = "";

            //Check for Internet connection
            json = await GetGeoJsonAsync(url);

            if (!string.IsNullOrEmpty(json))
            {
                var geoCodeData = JsonConvert.DeserializeObject<GeocodingParser>(json);
                if (!geoCodeData.status.Contains("ZERO"))
                {
                    if (geoCodeData.results[0] != null)
                    {
                        placeAddress = geoCodeData.results[0].formatted_address;
                    }
                }
            }

            return placeAddress;
        }

        public async Task<string> GetDirectionJsonAsync(LatLng location, LatLng destination)
        {
            //Origin of route
            string str_origin = "origin=" + location.Latitude + "," + location.Longitude;

            //Destination of route
            string str_destination = "destination=" + destination.Latitude + "," + destination.Longitude;

            //mode
            string mode = "mode=driving";

            //Buidling the parameters to the webservice
            string parameters = str_origin + "&" + str_destination + "&" + "&" + mode + "&key=";

            //Output format
            string output = "json";

            string key = mapkey;

            //Building the final url string
            string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" + parameters + key;

            string json = "";
            json = await GetGeoJsonAsync(url);

            return json;

        }
    }
     

    public class GeocodingParser
    {
        public IList<SharedPartnerDataModels.Result> results { get; set; }
        public string status { get; set; }
    }

}