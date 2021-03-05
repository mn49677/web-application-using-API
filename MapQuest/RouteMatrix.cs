using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace MapQuest
{
    public class RouteMatrix
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // Constructor
        private double[,] Locations { get; set; }

        // Main functions
        public RouteMatrix(double[,] Locations)
        {
            this.Locations = Locations;
        }
        /// <summary>
        /// Calculate distance matrix with MapQuest API
        /// </summary>
        /// <returns>long[,] distance matrix</returns>
        /// <exception cref="HttpRequestException">If API response fails, method returns HttpRequestException</exception>
        public async Task<long[,]> DistanceMatrix()
        {
            int N = Locations.GetLength(0);
            long[,] distanceMatrix = new long[N, N];
            for (int s = 0; s < N - 1; ++s) // every node is one source
            {
                long[] arr = await DistanceMatrix(s);
                FillDistanceMatrixBySourceResults(distanceMatrix, arr, s);
            }

            return distanceMatrix;
        }
        public async Task<long[]> DistanceMatrix(int source)
        {
            // Create JSON for MapQuest API request
            int N = Locations.GetLength(0);
            Root root = new Root
            {
                Locations = new List<Location>()
            };
            for (int i = source; i < N; ++i)
            {
                Location loc = new Location
                {
                    LatLng = new LatLng
                    {
                        lat = Locations[i, 1],
                        lng = Locations[i, 0]
                    }
                };
                root.Locations.Add(loc);
            }
            string locations = JsonConvert.SerializeObject(root);
            // Connection setup
            string key = ConfigurationManager.AppSettings["MapQuestKey"];//"8Oowq9tE4hJGgiMMRKG2APyIddcJj6jM";
            var baseAddress = new Uri("http://www.mapquestapi.com");
            var httpClient = new HttpClient { BaseAddress = baseAddress };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders
                .TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png");
            var content = new StringContent(locations,
                                            Encoding.UTF8,
                                            "application/json");
            Console.WriteLine(content.ReadAsStringAsync().Result);
            var response = await httpClient.PostAsync("/directions/v2/routematrix?key=" + key, content);
            string responseData = await response.Content.ReadAsStringAsync();
            var data = (JObject)JsonConvert.DeserializeObject(responseData);
            Console.WriteLine(data);
            if (data["distance"] == null || data == null)
            {
                throw new Newtonsoft.Json.JsonException("API response not valid. Json sent by API:\n" + data);
            }
            else
            {
                long[] value = data["distance"].ToObject<long[]>();
                Logger.Info("MapQuest API Distance matrix request.");
                return value;
            }
        }
        public async Task<long[,]> TimeMatrix(double speed)
        {
            long[,] timeMatrix = await DistanceMatrix();
            for (int i = 0; i < timeMatrix.GetLength(0); ++i)
                for (int j = 0; j < timeMatrix.GetLength(1); ++j)
                {
                    timeMatrix[i, j] = (long)(timeMatrix[i, j] / speed);
                }

            return timeMatrix;
        }
        // Help functions
        private long[,] FillDistanceMatrixBySourceResults(long[,] distanceMatrix, long[] distancesFromSource, int source)
        {
            int k = 0;
            int N = distanceMatrix.GetLength(0);

            for (int j = source; j < N; ++j)
            {
                distanceMatrix[source, j] = distancesFromSource[k];
                distanceMatrix[j, source] = distancesFromSource[k++];
            }
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    distanceMatrix[i, j] = distanceMatrix[i, j];

            return distanceMatrix;
        }
    }
}
