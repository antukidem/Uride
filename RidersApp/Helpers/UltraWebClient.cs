using Plugin.Connectivity;
using RidersApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Threading.Tasks;

namespace DisruptionLibraries
{
    public enum DownloadType
    {
        String,
        ByteArray,
        Stream
    }
    public static class UltraWebClient
    {
        private static string URL = string.Empty;
        public static DownloadType DownloadType = DownloadType.String;

        //  public static UltraWebClient( )
        //  {
        //      URL = url;
        //  }

        public static bool IsSuccess()
        {
            if ((GetQueryException() == null) && (GetQueryResult() != string.Empty))
            {
                return true;
            }
            return false;
        }

        public static bool IsConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        private static string QueryResult { get; set; }

        private static Stream QueryStreamResult { get; set; }

        private static byte[] QueryBytesResult { get; set; }

        private static WebException QueryException { get; set; }

        public static void SetQueryResult(string result)
        {
            QueryResult = result; ;
        }

        public static void SetQueryStreamResult(Stream result)
        {
            QueryStreamResult = result; ;
        }

        public static void SetQueryBytesResult(byte[] result)
        {
            QueryBytesResult = result; ;
        }

        public static void SetQueryException(WebException ex)
        {
            QueryException = ex;
        }

        public static string GetQueryResult()
        {
            return QueryResult;
        }

        public static Stream GetQueryStreamResult()
        {
            return QueryStreamResult;
        }

        public static byte[] GetQueryBytesResult()
        {
            return QueryBytesResult;
        }

        public static WebException GetQueryException()
        {
            return QueryException;
        }

        public static void DownloadData(string url, DownloadType type = DownloadType.String)
        {
            URL = url;
            DownloadType = type;
            string Result = string.Empty;
            if (CrossConnectivity.Current.IsConnected)
            { 
                Logger.Log(URL, "QUERYING PARTNER"); 
                try
                {
                    UltraDownloadString(type);
                    return;
                }
                catch (WebException we1)
                {
                }
                //only exception can make control fall through to this line  
                try
                {
                    WebClientDownloadString(type);
                    return;
                }
                catch (WebException we1)
                {
                }
                //only exception can make control fall through to this line  
                try
                {
                    HttpDownloadString(type);
                    return;
                }
                catch (WebException we1)
                {
                }


            }
            else
            {
                Logger.Log("No Internet connection");
                WebException wx = new WebException("No internet connection", new Exception("unable to reach out to external server"), WebExceptionStatus.ProtocolError, null);
                SetQueryException(wx);
                throw wx;
            }
        }

        public static async void DownloadStringAsync()
        {
            await Task.Run(() => { DownloadData(URL); });
        }

        private static void WebClientDownloadString(DownloadType downloadType)
        {
            using (WebClient _webClient = new WebClient())
            {
                try
                {
                    switch (downloadType)
                    {
                        case DownloadType.String:
                            SetQueryResult(_webClient.DownloadString(URL));
                            break;
                        case DownloadType.Stream:
                            throw new WebException("Feature not implemented");//SetQueryStreamResult(_webClient.OpenReadAsync(new System.Uri(URL)));
                            break;
                        case DownloadType.ByteArray:
                            throw new WebException("Feature not implemented");//SetQueryBytesResult(_webClient.DownloadDataAsync(new System.Uri(URL)));

                            break;
                    }

                }
                catch (WebException ex)
                {
                    SetQueryException(ex);
                }
                //finally
                //{
                //    if (_webClient != null)
                //    {
                //        _webClient.Dispose();
                //    }
                //}
            }
        }

        private static async void HttpDownloadString(DownloadType downloadType)
        {
            using (HttpClient httpc = new HttpClient())
            {
                try
                {
                    switch (downloadType)
                    {
                        case DownloadType.String:
                            SetQueryResult(await httpc.GetStringAsync(URL));
                            break;
                        case DownloadType.Stream:
                            SetQueryStreamResult(await httpc.GetStreamAsync(URL));
                            break;
                        case DownloadType.ByteArray:
                            SetQueryBytesResult(await httpc.GetByteArrayAsync(URL));
                            break;
                    } 
                }
                catch (WebException exc)
                {
                    SetQueryException(exc);
                }
            }
        }

        private static void UltraDownloadString(DownloadType downloadType)
        {
            try
            {
                switch (downloadType)
                {
                    case DownloadType.String:
                        SetQueryResult(UltraDownloadStringRaw(URL));
                        break;
                    case DownloadType.Stream:
                        throw new WebException("Feature not implemented");//SetQueryStreamResult(_webClient.OpenReadAsync(new System.Uri(URL)));
                        break;
                    case DownloadType.ByteArray:
                        throw new WebException("Feature not implemented");//SetQueryBytesResult(_webClient.DownloadDataAsync(new System.Uri(URL)));

                        break;
                }
            }
            catch (WebException exc)
            {
                SetQueryException(exc);
            }
        }
         
        private static string UltraDownloadStringRaw(string url)
        {
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

    }
}
