using System;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ArcGIS.Request;
using ArcGIS.Response;
using System.Threading.Tasks;

namespace ArcGIS
{
    public class RouteMatrix
    {
        public static int DISTANCE_MATRIX = 0;
        public static int TIME_MATRIX = 1;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private double[,] Locations { get; set; }

        public RouteMatrix(double[,] Locations)
        {
            this.Locations = Locations;
        }

        public async Task<long[]> CostMatrix(int costType, int source)
        {

            // Setup the request
            Root origin = new Root
            {
                features = new List<Feature>()
            };
            Root destinations = new Root
            {
                features = new List<Feature>()
            };
            int N = Locations.GetLength(0);
            for (int i = 0; i < N; ++i)
            {
                if (i != source) {
                    destinations.spatialReference = new Request.SpatialReference
                    {
                        wkid = 102100,
                    };
                    destinations.features.Add(new Feature
                    {
                        attributes = new Attributes
                        {
                            Name = i.ToString(),

                        },
                        geometry = new Geometry
                        {
                            x = Locations[i, 1],
                            y = Locations[i, 0]
                        }
                    });
                }
                else if (i == source)
                    origin.spatialReference = new Request.SpatialReference
                    {
                        wkid = 102100,
                    };
                    origin.features.Add(new Feature
                    {
                        attributes = new Attributes
                        {
                            Name = source.ToString(),

                        },
                        geometry = new Geometry
                        {
                            x = Locations[i, 1],
                            y = Locations[i, 0]
                        }
                    });
            }
            string key = ConfigurationManager.AppSettings["ArcGisKey"];
            var parameters = new Dictionary<string, string>
            {   { "origins", JsonConvert.SerializeObject(origin)}, 
                { "destinations",  JsonConvert.SerializeObject(destinations) }, 
                { "f", "pjson"}, 
                { "token",  key}, 
                { "distance_units", "Meters"} 
            };
            string uri = "https://logistics.arcgis.com";
            var encodedContent = new FormUrlEncodedContent(parameters);

            // Connection setup, JOB ID
            var baseAddress = new Uri(uri);
            var httpClient = new HttpClient { BaseAddress = baseAddress };
            httpClient.DefaultRequestHeaders.Clear();
            //Console.WriteLine(encodedContent.ReadAsStringAsync().Result);
            var response = await httpClient.PostAsync("/arcgis/rest/services/World/OriginDestinationCostMatrix/GPServer/GenerateOriginDestinationCostMatrix/submitJob/", encodedContent);
            string responseData = await response.Content.ReadAsStringAsync();
            var data = (JObject)JsonConvert.DeserializeObject(responseData);
            Console.WriteLine("JOB ID:\n"+data);
            // j
            string jobId = data["jobId"].ToObject<string>();
            //string jobId = "jecfce6d84c4b4e0dbc09d510d6326077";
            // GET DESTIONATIONS DISTANCES
            baseAddress = new Uri(uri);
            do
            {
                httpClient = new HttpClient { BaseAddress = baseAddress };
                httpClient.DefaultRequestHeaders.Clear();
                uri = "/arcgis/rest/services/World/OriginDestinationCostMatrix/GPServer/GenerateOriginDestinationCostMatrix/jobs/" + jobId + "?returnMessages=true&f=pjson&token=" + key;
                response = await httpClient.PostAsync(uri, null);
                responseData = await response.Content.ReadAsStringAsync();
                data = (JObject)JsonConvert.DeserializeObject(responseData);
            }
            while (data["jobStatus"].ToString() == "esriJobExecuting");

            httpClient = new HttpClient { BaseAddress = baseAddress };
            httpClient.DefaultRequestHeaders.Clear();
            uri = "/arcgis/rest/services/World/OriginDestinationCostMatrix/GPServer/GenerateOriginDestinationCostMatrix/jobs/" + jobId + "/results/Output_Origin_Destination_Lines?f=pjson&token=" + key;
            response = await httpClient.PostAsync(uri, null);
            responseData = await response.Content.ReadAsStringAsync();
            Console.WriteLine("RESULT:\n" + responseData);

            ArcGIS.Response.RootJson responseRoot = JsonConvert.DeserializeObject<ArcGIS.Response.RootJson>(responseData);
            long[] distances = new long[Locations.GetLength(0)];
            try
            {
                if (responseRoot.Equals( null )) throw new NullReferenceException();
                if (responseRoot.value == null) throw new NullReferenceException();
                if (responseRoot.value.features == null) throw new NullReferenceException();
                foreach (Featuree f in responseRoot.value.features)
                {
                    if (costType == RouteMatrix.DISTANCE_MATRIX) distances[Int32.Parse(f.attributes.DestinationName)] = (long)f.attributes.Total_Distance;
                    else if (costType == RouteMatrix.TIME_MATRIX) distances[Int32.Parse(f.attributes.DestinationName)] = (long)f.attributes.Total_Time;
                    return distances;
                }
            } 
            catch(NullReferenceException ex)
            {
                Logger.Error("NullReferenceException in distance matrix ArcGIS calculation. Probably invalid key. Exception: " + ex);
                Logger.Info("Distances from node " + source + " are calculated with Haversine.");
                
            }
            return null;
        }
        /// <summary>
        /// Cost Matrix ArcGIS REST Service
        /// </summary>
        /// <param name="costType">DISTANCE_MATRIX or TIME_MATRIX</param>
        /// <returns>long[,] distances between locations as matrix</returns>
        public async Task<long[,]> CostMatrix(int costType)
        {
            int N = Locations.GetLength(0);
            long[,] costMatrix = new long[N, N];
            for (int i = 0; i < N; ++i)
            {
                long[] distancesForSource = await CostMatrix(costType, i);

                AddDistancesToMatrix(costMatrix, distancesForSource, i);
            }
            return costMatrix;
        }

        // Help methods
        private void AddDistancesToMatrix(long[,] costMatrix, long[] distancesForSource, int source)
        {
            int N = costMatrix.GetLength(0);
            costMatrix[source, source] = 0;
            for (int i = 0; i < N; ++i)
            {
                costMatrix[source, i] = distancesForSource[i];
                costMatrix[i, source] = distancesForSource[i];
            }
        }
    }
}

