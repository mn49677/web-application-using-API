using System;
namespace VRPWebApp.ViewModels.Vrp
{
    public class Location
    {
        public int StopID { get; set; }
        public int OrderID { get; set; }
        public string ArrivalTime { get; set; }
        public string LocationName { get; set; }
        public int FloorSpace { get; set; }
        public int Volume { get; set; }
        public int Weight { get; set; }

        public long ServiceTime { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

    }
}
