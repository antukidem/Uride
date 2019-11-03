using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPartnerDataModels
{
    public class JOMap<T>
    {
        private string responseText;

        public JOMap()
        {
        }

        public JOMap(string responseText)
        {
            this.responseText = responseText;
        } 
        public T Load(string response)
        {
             return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response ); 
        } 
    }

    #region Twitter models
    public class Url2
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public List<int> indices { get; set; }
    }

    public class Entities
    {
        public Url url { get; set; }
        public Description description { get; set; }
    }

    public class Url
    {
        public List<Url2> urls { get; set; }
    }

    public class Description
    {
        public List<object> urls { get; set; }
    }

    public class TwitterUser
    {
        public int id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public object profile_location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public Entities entities { get; set; }
        public bool @protected { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public string created_at { get; set; }
        public int favourites_count { get; set; }
        public object utc_offset { get; set; }
        public object time_zone { get; set; }
        public object geo_enabled { get; set; }
        public bool verified { get; set; }
        public int statuses_count { get; set; }
        public object lang { get; set; }
        public object contributors_enabled { get; set; }
        public object is_translator { get; set; }
        public object is_translation_enabled { get; set; }
        public object profile_background_color { get; set; }
        public object profile_background_image_url { get; set; }
        public object profile_background_image_url_https { get; set; }
        public object profile_background_tile { get; set; }
        public object profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public object profile_banner_url { get; set; }
        public object profile_link_color { get; set; }
        public object profile_sidebar_border_color { get; set; }
        public object profile_sidebar_fill_color { get; set; }
        public object profile_text_color { get; set; }
        public object profile_use_background_image { get; set; }
        public object has_extended_profile { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        public object following { get; set; }
        public object follow_request_sent { get; set; }
        public object notifications { get; set; }
        public object translator_type { get; set; }
    }
    #endregion

    #region  Google Model
    public class PlacesBounds
    {
        public PlacesBounds()
        {
            this.pickUp = new LocationDetail();
            this.destination = new LocationDetail();
            this.lineDrawn = false;
        }
        public bool lineDrawn { get; set; }
        public LocationDetail pickUp { get; set; }
        public LocationDetail destination { get; set; }
    }

    public class LocationDetail
    {
        public LocationDetail()
        {
            Id = string.Empty;
            Name = string.Empty;
            LatLng = new Location();
            Address = string.Empty; ;
            State = string.Empty;
        }
        public LocationDetail(string name, string state, Location latLng)
        {
            Id = name;
            Name = name;
            LatLng = latLng;
            Address = name;
            State = state;
        }
        public LocationDetail(string id, string name, string address, string state, Location latLng)
        {
            Id = id;
            Name = name;
            LatLng = latLng;
            Address = address;
            State = state;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Location LatLng { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
    }

    public class MatchedSubString
    {
        public int length { get; set; }
        public int offset { get; set; }
    }

    public class MainTextMatchedSubstring
    {
        public int length { get; set; }
        public int offset { get; set; }
    }

    public class StructuredFormatting
    {
        public string main_text { get; set; }
        public List<MainTextMatchedSubstring> main_text_matched_substrings { get; set; }
        public string secondary_text { get; set; }
    }

    public class Term
    {
        public int offset { get; set; }
        public string value { get; set; }
    }

    public class Prediction
    {
        public string description { get; set; }
        public string id { get; set; }
        public List<MatchedSubString> matched_substrings { get; set; }
        public string place_id { get; set; }
        public string reference { get; set; }
        public StructuredFormatting structured_formatting { get; set; }
        public List<Term> terms { get; set; }
        public List<string> types { get; set; }
    }

    public class GoogleMapPlaceClass
    {
        public List<Prediction> predictions { get; set; }
        public string status { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    //public class Location
    //{
    //    public Location(double v1, double v2)
    //    {
    //    }
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}

    public class Northeast2
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest2
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast2 northeast { get; set; }
        public Southwest2 southwest { get; set; }
    }

    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }


    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }
    public enum ResultStatus
    {
        OK,
        ZERO_RESULTS

    }
    public class GeoCodeJSONClass
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

    public class GoogleException
    {
        public string error_message { get; set; }
        public List<object> predictions { get; set; }
        public string status { get; set; }
    }

    public class PlusCode
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }
    }

    public class ReverseGeoCodeRootObject
    {
        public PlusCode plus_code { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

    public class PasswordChangeModel
    {
        public string NewPassword { get; set; }
        public List<PasswordHistoryItem> PasswordHistoryItems { get; set; }
        public string Username { get; set; }
    }

    public class PasswordHistoryItem
    {
        public string PasswordText { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public interface IPasswordValidator
    {
        bool IsValid(PasswordChangeModel passwordChangeModel);
    }

    public interface IPasswordValidationRule
    {
        bool IsValid(PasswordChangeModel passwordChangeModel);
    }

    public class PasswordMinLengthRule : IPasswordValidationRule
    {
        private const int _minLength = 8;

        public bool IsValid(PasswordChangeModel passwordChangeModel)
        {
            return passwordChangeModel.NewPassword.Length >= _minLength;
        }
    }

    //public  class   PasswordMinLengthRule : IPasswordValidationRule
    //{
    //    private readonly int _minLength;
    //    public PasswordMinLengthRule(int minLength)
    //    {
    //        _minLength = minLength;
    //    }
    //    public bool IsValid(Password password)
    //    {
    //        return password.NewPassword.Length >= _minLength;
    //    }
    //}

    public class GeocodedWaypoint
    {
        public string geocoder_status { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }



    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class EndLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class StartLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Distance2
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration2
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class EndLocation2
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Polyline
    {
        public string points { get; set; }
    }

    public class StartLocation2
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Step
    {
        public Distance2 distance { get; set; }
        public Duration2 duration { get; set; }
        public EndLocation2 end_location { get; set; }
        public string html_instructions { get; set; }
        public Polyline polyline { get; set; }
        public StartLocation2 start_location { get; set; }
        public string travel_mode { get; set; }
    }

    public class Leg
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string end_address { get; set; }
        public EndLocation end_location { get; set; }
        public string start_address { get; set; }
        public StartLocation start_location { get; set; }
        public List<Step> steps { get; set; }
        public List<object> traffic_speed_entry { get; set; }
        public List<object> via_waypoint { get; set; }
    }

    public class OverviewPolyline
    {
        public string points { get; set; }
    }

    public class Route
    {
        public Bounds bounds { get; set; }
        public string copyrights { get; set; }
        public List<Leg> legs { get; set; }
        public OverviewPolyline overview_polyline { get; set; }
        public string summary { get; set; }
        public List<object> warnings { get; set; }
        public List<object> waypoint_order { get; set; }
    }

    public class WaypointRootObject
    {
        public List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        public List<Route> routes { get; set; }
        public string status { get; set; }
    }


    public class Bounds2
    {
        public Bounds2()
        {
            northeast = new Northeast();
            southwest = new Southwest();
        }
        public string northeastLocationName { get; set; }
        public Northeast northeast { get; set; }
        public string southwestLocationName { get; set; }
        public Southwest southwest { get; set; }
    }

    //public class AddressComponent
    //{
    //    public string long_name { get; set; }
    //    public string short_name { get; set; }
    //    public List<string> types { get; set; }
    //}

    //public class Northeast
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    //public class Southwest
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    //public class Bounds
    //{
    //    public Northeast northeast { get; set; }
    //    public Southwest southwest { get; set; }
    //}

    public class Location
    {
        public Location()
        {
        }

        public Location(string v1, string v2)
        {
            lat = double.Parse(v1);
            lng = double.Parse(v2);
        }

        public Location(string fullName, string lattitude, string longitude)
        {
            lat = double.Parse(lattitude);
            lng = double.Parse(longitude);
        }

        public Location(double v1, double v2)
        {
            lat = v1;
            lng = v2;
        }
        public string fullName { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }

        public Location(Northeast v)
        {
            lat = v.lat;
            lng = v.lng;
        }

        public Location(Southwest v)
        {
            lat = v.lat;
            lng = v.lng;
        }
    }

    //************************************  START DRIECTION **********************************************
    //public class GeocodedWaypoint
    //{
    //    public string geocoder_status { get; set; }
    //    public string place_id { get; set; }
    //    public List<string> types { get; set; }
    //}
    //public class Northeast
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    //public class Southwest
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}

    //public class Bounds
    //{
    //    public Northeast northeast { get; set; }
    //    public Southwest southwest { get; set; }
    //}


    //public class Distance
    //{
    //    public string text { get; set; }
    //    public int value { get; set; }
    //}

    //public class Duration
    //{
    //    public string text { get; set; }
    //    public int value { get; set; }
    //}
    //public class EndLocation
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}

    //public class StartLocation
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    //public class Distance2
    //{
    //    public string text { get; set; }
    //    public int value { get; set; }
    //}
    //public class Duration2
    //{
    //    public string text { get; set; }
    //    public int value { get; set; }
    //}

    //public class EndLocation2
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    //public class Polyline
    //{
    //    public string points { get; set; }
    //}
    //public class StartLocation2
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    public class Traffic
    {
        public List<object> users { get; set; }
        public int usercount { get; set; }
        public double avgspeed { get; set; }
        public int status { get; set; }
    }
    //public class Step
    //{
    //    public Distance2 distance { get; set; }
    //    public Duration2 duration { get; set; }
    //    public EndLocation2 end_location { get; set; }
    //    public string html_instructions { get; set; }
    //    public Polyline polyline { get; set; }
    //    public StartLocation2 start_location { get; set; }
    //    public string travel_mode { get; set; }
    //    public Traffic traffic { get; set; }
    //    public string maneuver { get; set; }
    //}
    //public class Leg
    //{
    //    public Distance distance { get; set; }
    //    public Duration duration { get; set; }
    //    public string end_address { get; set; }
    //    public EndLocation end_location { get; set; }
    //    public string start_address { get; set; }
    //    public StartLocation start_location { get; set; }
    //    public List<Step> steps { get; set; }
    //    public List<object> via_waypoint { get; set; }
    //    public int priority { get; set; }
    //}
    //public class OverviewPolyline
    //{
    //    public string points { get; set; }
    //}
    //public class Route
    //{
    //    public Bounds bounds { get; set; }
    //    public string copyrights { get; set; }
    //    public List<Leg> legs { get; set; }
    //    public OverviewPolyline overview_polyline { get; set; }
    //    public string summary { get; set; }
    //    public List<object> warnings { get; set; }
    //    public List<object> waypoint_order { get; set; }
    //}
    public class GoogleDirectionClass
    {
        public List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        public List<Route> routes { get; set; }
        public string status { get; set; }
    }
    //************************************   END DIRECTION  ***********************************************

    //public class Northeast2
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    //public class Southwest2
    //{
    //    public double lat { get; set; }
    //    public double lng { get; set; }
    //}
    //public class Viewport
    //{
    //    public Northeast2 northeast { get; set; }
    //    public Southwest2 southwest { get; set; }
    //}
    //public class Geometry
    //{
    //    public Bounds bounds { get; set; }
    //    public Location location { get; set; }
    //    public string location_type { get; set; }
    //    public Viewport viewport { get; set; }
    //}
    //public class Result
    //{
    //    public List<AddressComponent> address_components { get; set; }
    //    public string formatted_address { get; set; }
    //    public Geometry geometry { get; set; }
    //    public string place_id { get; set; }
    //    public List<string> types { get; set; }
    //}
    //public class GeoCodeJSONClass
    //{
    //    public List<Result> results { get; set; }
    //    public string status { get; set; }
    //}
    #endregion
}
