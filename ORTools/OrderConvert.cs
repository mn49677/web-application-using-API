using DataMapperMock;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GDi_VRP_Models.Models.Orders;
using GDi_VRP_Models.Models.Routes;
using GDi_VRP_Models.Models.Depots;

namespace ORTools
{
    public class OrderConvert : IJsonConvert<OrderFeature>
    {

        public List<OrderFeature> ConvertFromJson(string name, string id)
        {
            string filePath = @".\\Data\\Data_01\\" + name + id + ".json";
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                OrderWrapper orders = JsonConvert.DeserializeObject<OrderWrapper>(json);
                return orders.features;
            }
        }
    }
}
