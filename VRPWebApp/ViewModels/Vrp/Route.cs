using System;
using System.Collections.Generic;

namespace VRPWebApp.ViewModels.Vrp
{
    public class Route
    {
        public int Id { get; set; }
        public string RouteName { get; set; }
        public float Hours { get; set; }
        public int Drive { get; set; }
        public int Distance { get; set; }
        public double TravelTime { get; set; }
        public double ServiceTime { get; set; }
        public int Stops { get; set; }
        public int FloorSpace { get; set; }
        public int Volume { get; set; }
        public int Weight { get; set; }
        public ICollection<Location> Locations { get; set; }

    }
}
