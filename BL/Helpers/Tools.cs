using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Helpers;
internal class Tools
{

    /// <summary>
    /// the function gets an address and if the address is legal returns its longitude and latitude        
    /// </summary>      
    public static async Task<(double Latitude, double Longitude)> GetCoordinatesFromAddress(string address)
    {
        string baseUrl = "https://nominatim.openstreetmap.org/search";
        string requestUrl = $"{baseUrl}?q={Uri.EscapeDataString(address)}&format=json";

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetStringAsync(requestUrl);
            var json = JArray.Parse(response);

            if (json.Count > 0)
            {
                double latitude = (double)json[0]["lat"];
                double longitude = (double)json[0]["lon"];
                return (latitude, longitude);
            }
            else
            {
                throw new Exception("No results were found for the address.");
            }
        }
    }

    /// <summary>
    /// Calculates the distance between 2 addresses represented by latitude and longitude points
    /// </summary>
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);
        double EarthRadiusKm = 6371.0; // Earth's radius in kilometers

        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c; //The result is in kilometers
    }
};

