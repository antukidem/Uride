using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Text;
//using Android.Gms.Maps.Model;
using Disruption;
using System.IO;
using System.Net.Cache;
using System.Net.Http;
using SharedPartnerDataModels;
using DisruptionLibraries;
using System.Threading.Tasks;

public class Directions : IPartnerServiceApi
{
    const string GoogleDiectionApiUri = "https://maps.googleapis.com/maps/api/directions/";
    private string _Key;
    //private IPerformPartnerServiceRequestProcessor _performPartnerServerQuery;

    private string _originLat;
    private string _originLng;
    private string _destinationLat;
    private string _destinationLng;

    public Directions(string Key)
    {
        _Key = Key;
    }
    public Directions(string Key, string originLat, string originLng, string destinationLat, string destinationLng)
    {
        _Key = Key;
        _originLat = originLat;
        _originLng = originLng;
        _destinationLat = destinationLat;
        _destinationLng = destinationLng;

    }

    public Directions(string Key, Location origin, Location destination)
    {
        _Key = Key;
        _Key = Key;
        _originLat = origin.lat.ToString();
        _originLng = origin.lng.ToString();
        _destinationLat = destination.lat.ToString();
        _destinationLng = destination.lng.ToString();
    }

    private string OriginLat { get; set; }
    private string OriginLng { get; set; }
    private string DestinationLat { get; set; }
    private string DestinationLng { get; set; }

    public string GetRequestUrl()
    {
        string str_org = "origin=" + _originLat + "," + _originLng;
        string str_dest = "destination=" + _destinationLat + "," + _destinationLng;
        string sensor = "sensor=false";
        string mode = "mode=driving";
        string key = "key=" + _Key;
        string param = str_org + "&" + str_dest + "&" + sensor + "&" + mode + "&" + key;
        string output = "json";
        string url = GoogleDiectionApiUri + output + "?" + param;
        return url;
    }

    public object ProcessPartnerServiceQueryResults(string result)
    {
        GoogleDirectionClass JsonObject = JsonConvert.DeserializeObject<GoogleDirectionClass>(result);
        return JsonObject;
    }

}

public class SearchAutoSuggestions : IPartnerServiceApi
{
    const string AutoCompleteGoogleApiUri = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=";
    private string _Key;
    //private IPerformPartnerServiceRequestProcessor _performPartnerServerQuery;

    public SearchAutoSuggestions(string key, string searchText)
    {
        _Key = key;
        SearchInput = searchText;
    }

    private string SearchInput { get; set; }

    public void Dispose()
    {
        _Key = null;
        SearchInput = null;
    }

    public string GetRequestUrl()
    {
        return AutoCompleteGoogleApiUri + SearchInput + "&key=" + _Key;
    }

    public object ProcessPartnerServiceQueryResults(string result)
    {
        GoogleMapPlaceClass objMapClass = JsonConvert.DeserializeObject<GoogleMapPlaceClass>(result.ToString());// (autoCompleteOptions);
        //string[] strPredictiveText = new string[objMapClass.predictions.Count];
        //int index = 0;
        //foreach (Prediction objPred in objMapClass.predictions)
        //{
        //    strPredictiveText[index] = objPred.description;
        //    index++;
        //} 

        List<KeyValuePair<string, string>> strPredictiveText = new List<KeyValuePair<string, string>>();
        foreach (Prediction objPred in objMapClass.predictions)
        {
            strPredictiveText.Add(new KeyValuePair<string, string>(objPred.description, objPred.place_id));
        }
        return strPredictiveText;
    }
}

public class ReverseGeocodeByPlaceId : IPartnerServiceApi
{
    const string PlacesApiUri = "https://maps.googleapis.com/maps/api/geocode/";
    private string _Key;
    private string _placeId;
    //private IPerformPartnerServiceRequestProcessor _performPartnerServerQuery;

    public ReverseGeocodeByPlaceId(string key, string placeId)
    {
        _Key = key;
        _placeId = placeId;
    }

    private string SearchInput { get; set; }

    public void Dispose()
    {
        _Key = null;
        _placeId = null;
    }

    public string GetRequestUrl()
    {
        string place = "place_id=" + _placeId;
        string key = "key=" + _Key;
        string param = place + "&" + key;
        string output = "json";
        string url = PlacesApiUri + output + "?" + param;
        return url;
    }

    public object ProcessPartnerServiceQueryResults(string result)
    {
        GeoCodeJSONClass objMapClass = JsonConvert.DeserializeObject<GeoCodeJSONClass>(result.ToString());// (autoCompleteOptions);

        return objMapClass.results[0].geometry.location;
    }
}

public class GeoCode : IPartnerServiceApi
{
    const string ResourceUri = "https://maps.googleapis.com/maps/api/geocode/json";

    private string _key;

    //private IPerformPartnerServiceRequestProcessor _performPartnerServerQuery;

    public GeoCode(string key, string phisicalAddress)
    {
        _key = key;
        PlaceName = phisicalAddress;
    }

    private string PlaceName { get; set; }

    public string GetRequestUrl()
    {
        var sb = new StringBuilder();
        sb.Append(ResourceUri);
        sb.Append("?address=").Append(PlaceName);
        sb.Append("&key=").Append(_key);
        return sb.ToString();
    }

    public object ProcessPartnerServiceQueryResults(string result)
    {

        GeoCodeJSONClass objGeoCodeJSONClass = JsonConvert.DeserializeObject<GeoCodeJSONClass>(result);
        if (objGeoCodeJSONClass.status == "ZERO_RESULTS")
        { return null; }
        else
        { return objGeoCodeJSONClass.results[0].geometry.location; }

    }

}

public class ReverseGeoCode : IPartnerServiceApi
{
    const string ResourceUri = "https://maps.googleapis.com/maps/api/geocode/json";
    //"https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=YOUR_API_KEY" 
    private string _key;

    //private IPerformPartnerServiceRequestProcessor _performPartnerServerQuery;

    public ReverseGeoCode(string key, string latlng)
    {
        _key = key;
        LatLng = latlng;
    }

    public ReverseGeoCode(string key, Location latlng)
    {
        _key = key;
        LatLng = latlng.lat + "," + latlng.lng;
    }

    public ReverseGeoCode(string key, double lattitude, double longitude)
    {
        _key = key;
        LatLng = lattitude.ToString() + "," + longitude.ToString();
    }
    public ReverseGeoCode(string key, string lattitude, string longitude)
    {
        _key = key;
        LatLng = lattitude + "," + longitude;
    }

    private string LatLng { get; set; }

    public string GetRequestUrl()
    {
        return new StringBuilder().Append(ResourceUri).Append("?latlng=").Append(LatLng).Append("&key=").Append(_key).ToString();
    }

    public object ProcessPartnerServiceQueryResults(string result)
    {
        ReverseGeoCodeRootObject objGeoCodeJSONClass = JsonConvert.DeserializeObject<ReverseGeoCodeRootObject>(result);
        return (objGeoCodeJSONClass);
    }
}

//Responsibility of this classs asynchroneously make a request to server and get response back to the calling  module
public class PartnerServiceRequestProcessor : IPerformPartnerServiceRequestProcessor
{
    private IPartnerServiceApi _partnerServiceApi;

    public PartnerServiceRequestProcessor(IPartnerServiceApi partnerServiceApi)
    {
        this._partnerServiceApi = partnerServiceApi;
        PerformPartnerServerQueryAsync();
    }

    private string QueryResult { get; set; }

    private Exception QueryException { get; set; } = null;

    //private string FetchLocalCacheData()
    //{
    //    string res = string.Empty;
    //    string querParam = string.Empty;

    //    if (_partnerServiceApi.GetRequestUrl().IndexOf("autocomplete") > 0)
    //    {
    //        querParam = HttpUtility.ParseQueryString(new Uri(_partnerServiceApi.GetRequestUrl()).Query).Get("input");
    //        res = new LocalDataCahe().LookUpNames(querParam.Trim());
    //    }
    //    if (_partnerServiceApi.GetRequestUrl().IndexOf("address") > 0)
    //    {
    //        querParam = HttpUtility.ParseQueryString(new Uri(_partnerServiceApi.GetRequestUrl()).Query).Get("address");
    //        res = new LocalDataCahe().GetCordinates(querParam.Trim());
    //    }
    //    if (_partnerServiceApi.GetRequestUrl().IndexOf("latlng") > 0)
    //    {
    //        querParam = HttpUtility.ParseQueryString(new Uri(_partnerServiceApi.GetRequestUrl()).Query).Get("latlng");
    //        res = new LocalDataCahe().GetReverseName(querParam.Trim());
    //    }
    //    return res;
    //}

    public async void PerformPartnerServerQueryAsync()
    {
        try
        {
            #region  Query cache Implementation
            //implement logic to check cahe data by checking url parameters before  
            //FetchLocalCacheData(); if (response == .Empty) { response =   _webClient.DownloadString (new Uri(_partnerServiceApi.GetRequestUrl())); }
            #endregion

            //response = UltraDownloadString(_partnerServiceApi.GetRequestUrl());
            UltraWebClient.DownloadData(_partnerServiceApi.GetRequestUrl());
            SetPartnerServiceQueryResult(UltraWebClient.GetQueryResult());
        }
        catch (Exception exception)
        {
            SetPatnerSrviceQueryException(UltraWebClient.GetQueryException());

            #region obsolete
            //using (WebClient _webClient = new WebClient())
            //{
            //    try
            //    {
            //        response = _webClient.DownloadString(new Uri(_partnerServiceApi.GetRequestUrl()));
            //        SetPartnerServiceQueryResult(response);
            //    }
            //    catch (Exception ex)
            //    {
            //        SetPatnerSrviceQueryException(ex);

            //        try
            //        {
            //            using (HttpClient httpc = new HttpClient())
            //            {
            //                response = await httpc.GetStringAsync(_partnerServiceApi.GetRequestUrl());
            //                SetPartnerServiceQueryResult(response);
            //            }
            //        }
            //        catch (Exception exc)
            //        {
            //            SetPatnerSrviceQueryException(exc);
            //        }
            //    }
            //    finally
            //    {
            //        if (_webClient != null)
            //        {
            //            _webClient.Dispose();
            //        }
            //    }
            //}
            #endregion
        }
    }

    public void SetPartnerServiceQueryResult(string response)
    {
        QueryResult = response;
    }

    public void SetPatnerSrviceQueryException(Exception exception)
    {
        QueryException = exception;
    }

    public string GetPartnerServiceQueryResult()
    {
        return QueryResult;
    }

    public Exception GetPatnerServiceQueryException()
    {
        return QueryException;
    }

    public object ProcessPartnerServiceQueryResult()
    {
        if (QueryResult == null)
        {
            return null;
        }
        else
        {
            return _partnerServiceApi.ProcessPartnerServiceQueryResults(QueryResult);
        }
    }


    [Obsolete]
    public async Task<string> GetGeoJsonAsync(string url)
    {
        var handler = new HttpClientHandler();
        using (HttpClient client = new HttpClient(handler))
        {
            string result = string.Empty;
            try
            {
                result = await client.GetStringAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("**********************************************************************************************************************");
                Console.WriteLine("URL: " + url);
                Console.WriteLine("ERROR :" + ex.Message);
                Console.WriteLine("**********************************************************************************************************************");

                return string.Empty;
            }
            finally
            {
                handler = null;
                result = null;
            }
        }
    }

    [Obsolete]
    public async void PerformPartnerServerQueryAsync3()
    {
        string response = "";
        try
        {
            string url = _partnerServiceApi.GetRequestUrl();
            response = await GetGeoJsonAsync(url);
            SetPartnerServiceQueryResult(response);
        }
        catch (Exception exc)
        {
            SetPatnerSrviceQueryException(exc);
        }
    }

    [Obsolete]
    public object ProcessPartnerServiceQueryResult2()
    {
        if (QueryResult == null)
        {
            return null;
        }
        else
        {
            object o = null;
            try
            {
                o = _partnerServiceApi.ProcessPartnerServiceQueryResults(QueryResult);
            }
            catch (Exception exception)
            {
                SetPatnerSrviceQueryException(exception);
            }
            return o;
        }
    }

    private static string UltraDownloadString(string url)
    {
        RidersApp.Logger.Log(url, "QUERYING PARTNER");
         
        // Create a policy that allows items in the cache to be used if they have been cached 7 days or less. 
        HttpRequestCachePolicy requestPolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, TimeSpan.FromDays(7));

        string responseFromServer = string.Empty;
        // await Task.Run(() =>{ });
        // Create a request for the URL.   
        HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
        // Set the policy for this request only.  
        //request.CachePolicy = requestPolicy;
        // If required by the server, set the credentials.  
        //request.Credentials = CredentialCache.DefaultCredentials;
        //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0";
        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
        //request.Method = "GET"; 
        //request.Timeout = System.Threading.Timeout.Infinite;
        //request.KeepAlive = true;
        //request.ServicePoint.Expect100Continue = false;
        //request.ProtocolVersion = HttpVersion.Version11;
        //request.PreAuthenticate = true;
        //request.AllowWriteStreamBuffering = true;
        // request.Timeout = 5000;
        //request.ReadWriteTimeout = 5000;
        // Get the response.  
        HttpWebResponse response = (HttpWebResponse)request.GetResponse() as HttpWebResponse;
        // Display the status.   
        Console.WriteLine(response.StatusDescription);
        // Get the stream containing content returned by the server. 
        using (Stream dataStream = response.GetResponseStream())
        {
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            responseFromServer = reader.ReadToEnd();
            // Clean up the streams and the response.  
            reader.Close();
            dataStream.Close();
            response.Close();
            // Display the content.  
            Console.WriteLine(responseFromServer);
        }
        return responseFromServer;
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this._partnerServiceApi = null;
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~PartnerServiceRequestProcessor() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion


    public class Duration
    {
        private string measurementUnit { get; set; }
    }

    public class Distance
    {
        private string measurementUnit { get; set; }

        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        { 
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}

namespace DisruptionLibraries
{
    public enum TripType
    {
        Taxi_Service,
        Interstate
    }
}
