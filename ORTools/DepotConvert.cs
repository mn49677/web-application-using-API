using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using GDi_VRP_Models.Models.Depots;

namespace ORTools
{
    public class DepotConvert : IJsonConvert<DepotFeature>
    {
        //name = "input_orders"
        public List<DepotFeature> ConvertFromJson(string name, string id)
        {

            string filePath = @".\\Data\\Data_01\\" + name + id + ".json";
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                DepotWrapper depots = JsonConvert.DeserializeObject<DepotWrapper>(json);
                return depots.features;
            }
        }
    }
}
