using GDi_VRP_Models.Models.Routes;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace ORTools
{
    public class RouteConvert : IJsonConvert<RouteFeature>
    {
        public List<RouteFeature> ConvertFromJson(string name, string id)
        {
            string filePath = @".\\Data\\Data_01\\" + name + id + ".json";
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                RouteWrapper routes = JsonConvert.DeserializeObject<RouteWrapper>(json);
                return routes.features;
            }
        }
    }
}
