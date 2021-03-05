using System;
using System.Collections.Generic;
using System.Text;
using GDi_VRP_Models.Models;

namespace ORTools
{
    public class GetDataModel
    {
        public DataModel dataModel;

        public void GenerateDataModel(string[] names, string id)
        {
            //mapper
            ORTools.DataMapper_GDiVRP dataMapper = new ORTools.DataMapper_GDiVRP();
            //gdi model
            SchedulerVRPRequestModel model = new SchedulerVRPRequestModel();
            //json -> gdi model
            OrderConvert orderConvert = new OrderConvert();
            model.Orders = orderConvert.ConvertFromJson(names[0], id);

            RouteConvert routeConvert = new RouteConvert();
            model.Routes = routeConvert.ConvertFromJson(names[1], id);

            DepotConvert depotConvert = new DepotConvert();
            model.Depots = depotConvert.ConvertFromJson(names[2], id);

            //gdi model -> nas model
            this.dataModel = dataMapper.Map(model);
        }

    }
}
