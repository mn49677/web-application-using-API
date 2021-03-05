using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading;
using System.Configuration;

namespace OpenRouteService
{
    public class DataModelORS
    {
        public DataModelORS(double[,] Locations)
        {
            this.Locations = Locations;
        }
        public double[,] Locations { get; set; }
        public async Task<long[,]> DistanceMatrix()
        {
            int N = Locations.GetLength(0);
            long[,] distanceMatrix = new long[N, N];
            for (int s = 0; s < N; ++s) // every node is one source
            {
                //Thread.Sleep(1);
                long[,] arr = await DistanceMatrix(Locations, s);
                FillDistanceMatrixBySourceResults(distanceMatrix, arr, s);
            }
            return distanceMatrix;
        }
        private async Task<long[,]> DistanceMatrix(double[,] Locations, int source)
        {
            return await DistanceMatrix(JsonConvert.SerializeObject(Locations), source, Locations.GetLength(0));
        }
        private async Task<long[,]> DistanceMatrix(string locations, int source, int locationNumber)
        {
            var baseAddress = new Uri("https://api.openrouteservice.org");

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png");
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["OpenRouteServiceKey"]); //"5b3ce3597851110001cf624815034960d1524c1095953c44be5b0ab5");
                // quotes might have to be escaped
                // string serialized = JsonConvert.SerializeObject(Locations);
                string destinations = "";
                for (int i = source; i < locationNumber; ++i)
                {
                    destinations += i + ",";
                }
                destinations = destinations.Remove(destinations.Length - 1);

                using (var content = new StringContent("{\"locations\":" + locations + ",\n"
                                                        + "\"sources\":" + "[" + source + "]" + ",\n"
                                                        + "\"destinations\":" + "[" + destinations + "]" + ",\n"
                                                        + "\"metrics\":[\"distance\"],\n"
                                                        + "\"units\":" + "\"m\"}",
                                                        Encoding.UTF8,
                                                        "application/json"))
                {

                    while (true) {
                        var response = await httpClient.PostAsync("/v2/matrix/driving-car", content);
                        string responseData = await response.Content.ReadAsStringAsync();
                        var data = (JObject)JsonConvert.DeserializeObject(responseData);
                        if(data["distances"] == null)
                        {
                            // do nothing
                            Thread.Sleep(60000);
                            Console.WriteLine(data);
                        }
                        else
                        {
                            long[,] value = data["distances"].ToObject<long[,]>();
                            return value;
                        }
                    }
                }
            }
        }
        private long[,] FillDistanceMatrixBySourceResults(long[,] distanceMatrix, long[,] distancesFromSource, int source)
        {
            int k = 0;
            int N = distanceMatrix.GetLength(0);

            for (int j = source; j < N; ++j)
            {
                distanceMatrix[source, j] = distancesFromSource[0, k];
                distanceMatrix[j, source] = distancesFromSource[0, k++];
            }
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    distanceMatrix[i, j] = distanceMatrix[i, j];

            return distanceMatrix;
        }
        public async Task<long[,]> TimeMatrix(double speed)
        {
            long[,] timeMatrix = await DistanceMatrix();
            for(int i = 0; i < timeMatrix.GetLength(0); ++ i)
                for(int j = 0; j < timeMatrix.GetLength(1); ++j)
                {
                    timeMatrix[i, j] = (long) ((timeMatrix[i,j] / 1000) / speed);
                }
            return timeMatrix;
        }
    }
}