using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Locations;
using Xamarin.Essentials;
using SharedPartnerDataModels;
using RidersApp;

namespace Disruption
{
    class UiServices
    {
        public static async Task<string> SendSms(string messageText, List<string> recipients)
        {
            string responseCodes = "Sent";
            try
            {
                var message = new SmsMessage(messageText, recipients);
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                responseCodes = ex.Message;   // Sms is not supported on this device.
            }
            catch (Exception ex)
            {
                responseCodes = ex.Message;  // Other error has occurred.
            }
            return responseCodes;
        }

        private List<LatLng> getDirectionPolylines(List<Route> routes)
        {
            List<LatLng> directionList = new List<LatLng>();
            foreach (Route route in routes)
            {
                List<Leg> legs = route.legs;
                foreach (Leg leg in legs)
                {
                    string routeDistance = leg.distance.text;
                    string routeDuration = leg.duration.text;
                    setRouteDistanceAndDuration(routeDistance, routeDuration);
                    List<Step> steps = leg.steps;
                    foreach (Step step in steps)
                    {
                        SharedPartnerDataModels.Polyline polyline = step.polyline;
                        string points = polyline.points;
                        List<LatLng> singlePolyline = decodePoly(points);
                        foreach (LatLng direction in singlePolyline)
                        {
                            directionList.Add(direction);
                        }
                    }
                }
            }
            return directionList;
        }

        private void setRouteDistanceAndDuration(string distance, string duration)
        {
            //distanceValue.setText(distance);
            //durationValue.setText(duration);
        }

        //Reverse geocode : convert Locaton to name 
        public async Task<Address> ReverseGeoCode(MainActivity mainActivity, Context ctx, SharedPartnerDataModels.Location loc)
        {
            Geocoder geocoder = new Geocoder(ctx);
            IList<Address> addressList = await geocoder.GetFromLocationAsync(loc.lat, loc.lng, 5);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        public async Task<Address> ReverseGeoCode(Context ctx, double Latitude, double Longitude)
        {
            Geocoder geocoder = new Geocoder(ctx);
            IList<Address> addressList = await geocoder.GetFromLocationAsync(Latitude, Longitude, 5);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        public GoogleDirectionClass GetDirectionsData(SharedPartnerDataModels.Bounds2 bounds)
        {
            //Initializer and  query partner server 
            using (PartnerServiceRequestProcessor webRequest = new PartnerServiceRequestProcessor(
                new Directions(Application.Context.Resources.GetString(RidersApp.Resource.String.PlacesAPIKey), new SharedPartnerDataModels.Location(bounds.northeast), new SharedPartnerDataModels.Location(bounds.southwest))))
            {
                Exception exception = webRequest.GetPatnerServiceQueryException();
                GoogleDirectionClass directions = null;
                if (exception is null)
                {
                    directions = (GoogleDirectionClass)webRequest.ProcessPartnerServiceQueryResult();
                }
                return directions;
            }
        }


        public static async Task<object> ReverseGeoCodeAsync(string latlng)
        {
            //Initializer and  query partner server 
            return await Task.Run(() => ReverseGeoCode(latlng));
        }

        public static object ReverseGeoCode(string latlng, bool returnFullAddress=false)
        {
            //Initializer and  query partner server 
            using (PartnerServiceRequestProcessor webRequest = new PartnerServiceRequestProcessor(new ReverseGeoCode(Application.Context.Resources.GetString(RidersApp.Resource.String.PlacesAPIKey), latlng)))
            {
                Exception exception = webRequest.GetPatnerServiceQueryException();
                //check if there was an exception in the line above  
                if (exception == null)
                {
                    //since there was no exception in the line above then get the response and cast to expectad wrapper object   
                    ReverseGeoCodeRootObject address = (ReverseGeoCodeRootObject)webRequest.ProcessPartnerServiceQueryResult();
                    if (address != null)
                    {
                        if(returnFullAddress ==false) { 
                            return address.results[0].formatted_address;
                        }
                        else
                        {
                            return address;
                        }
                    }
                    else
                    {
                        Console.WriteLine(">>>>>>>>  PEN " + exception.Message + " <<<<<<<<<");
                        return null ;
                    };
                }else if(((System.Net.WebException )exception).Status == System.Net.WebExceptionStatus.SecureChannelFailure)
                {
                    //since there was no exception in the line above then get the response and cast to expectad wrapper object   
                    ReverseGeoCodeRootObject address = (ReverseGeoCodeRootObject)webRequest.ProcessPartnerServiceQueryResult();
                    if (address != null)
                    {
                        if (returnFullAddress == false)
                        {
                            return address.results[0].formatted_address;
                        }
                        else
                        {
                            return address;
                        }
                    }
                    else
                    {
                        Console.WriteLine(">>>>>>>>  PEN " + exception.Message + " <<<<<<<<<");
                        return null;
                    };
                }
                else
                {
                        Console.WriteLine(">>>>>>>>  ULTIMATE " + exception.Message + " <<<<<<<<<");
                    return null ;
                }
            }
        }

        public async Task<Address> Geocode(Context ctx, string LocationName)
        {
            Geocoder geocoder = new Geocoder(ctx);
            IList<Address> addressList = await geocoder.GetFromLocationNameAsync(LocationName, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        //=========================================================================================================================

        public static async Task<LatLng> convertNameToLatLngLocation(Context ctx, string LocationName)
        {
            var geo = new Geocoder(ctx);
            LatLng latLng = new LatLng(0, 0);
            try
            {
                var address = await geo.GetFromLocationNameAsync(LocationName, 1);
                address.ToList().ForEach((addr) =>
                {
                    latLng = new LatLng(addr.Latitude, addr.Longitude);
                });
                return latLng;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return latLng;
            }
            finally
            {
                geo = null;
            }
        }


        public static async Task<Bounds> LocationToLatLng(Context ctx, string sourceLocationName, string destinationLocationName)
        {
            try
            {
                LatLng sourceLatLng = await convertNameToLatLngLocation(ctx, sourceLocationName);
                LatLng destLatLng = await convertNameToLatLngLocation(ctx, destinationLocationName);

                return new Bounds
                {
                    northeast = new Northeast() { lat = sourceLatLng.Latitude, lng = sourceLatLng.Longitude },
                    southwest = new Southwest() { lat = destLatLng.Latitude, lng = destLatLng.Longitude }
                };
            }
            catch
            {
                return null;
            }
        }
        public static async Task<Bounds2> LocationToLatLng2(Context ctx, string sourceLocationName, string destinationLocationName)
        {
            try
            {
                LatLng sourceLatLng = await convertNameToLatLngLocation(ctx, sourceLocationName);

                LatLng destLatLng = await convertNameToLatLngLocation(ctx, destinationLocationName);

                return new Bounds2
                {
                    northeastLocationName = sourceLocationName,
                    northeast = new SharedPartnerDataModels.Northeast() { lat = sourceLatLng.Latitude, lng = sourceLatLng.Longitude },
                    southwestLocationName = destinationLocationName,
                    southwest = new SharedPartnerDataModels.Southwest() { lat = destLatLng.Latitude, lng = destLatLng.Longitude }
                };
            }
            catch
            {
                return null;
            }
        }

        //var geo = new Geocoder(ctx);//var sourceAddresss = await geo.GetFromLocationNameAsync(sourceLocationName, 1);
        //sourceAddress.ToList().ForEach((addr) => { latLngSource = new LatLng(addr.Latitude, addr.Longitude); }); 
        //var destAddress = await geo.GetFromLocationNameAsync(destinationLocationName, 1);
        //destAddress.ToList().ForEach((addr) => { latLngDestination = new LatLng(addr.Latitude, addr.Longitude); }); 

        //using google geocode api to convert location to latlng  
        //string strGeoCode = string.Format ("address={0}",strLocation);
        //string strGeoCodeFullURL = string.Format (Constants.strGeoCodingUrl,strGeoCode);
        //
        //string strResult= FnHttpRequestOnMainThread(strGeoCodeFullURL); 
        //if ( strResult != Constants.strException )
        //{ 
        //		var objGeoCodeJSONClass= JsonConvert.DeserializeObject<GeoCodeJSONClass> (strResult);  
        //		Position= new LatLng ( objGeoCodeJSONClass.results [0].geometry.location.lat , objGeoCodeJSONClass.results [0].geometry.location.lng );
        //}  
        //function to decode,encoded points

        public static List<SharedPartnerDataModels.Location> FnDecodePolylinePoints(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                return null;
            var poly = new List<SharedPartnerDataModels.Location>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylinechars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                SharedPartnerDataModels.Location p = new SharedPartnerDataModels.Location();
                p.lat = Convert.ToDouble(currentLat) / 100000.0;
                p.lng = Convert.ToDouble(currentLng) / 100000.0;
                poly.Add(p);
            } 
            return poly;
        }

        //function to decode,encoded LatLng points
        public static List<LatLng> decodePoly(string encoded)
        {
            List<LatLng> poly = new List<LatLng>();
            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;
            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;
                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;
                LatLng p = new LatLng((((double)lat / 1E5)),
                        (((double)lng / 1E5)));
                poly.Add(p);
            }
            return poly;
        }

        //function to decode,encoded SharedGoogleDataModels.Location points
        public static List<SharedPartnerDataModels.Location> locDecodePoly(string encoded)
        {
            List<SharedPartnerDataModels.Location> poly = new List<SharedPartnerDataModels.Location>();
            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;
            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;
                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;
                SharedPartnerDataModels.Location p = new SharedPartnerDataModels.Location((((double)lat / 1E5)),
                        (((double)lng / 1E5)));
                poly.Add(p);
            }
            return poly;
        }
    }
}