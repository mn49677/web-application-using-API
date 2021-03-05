using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Routes
{
    /// <summary>
    /// Specify each attribute as a key-value pair where the key is the name of a given field, and the value is the attribute value for the corresponding field.
    /// </summary>
    public class RouteAttributes
    {
        /// <summary>
        /// The name of the route.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The name of the starting depot for the route.
        /// Foreign key to <see cref="DepotAttributes.Name"/> attribute.
        /// </summary>
        public string StartDepotName { get; set; }
        /// <summary>
        /// The name of the ending depot for the route.
        /// Foreign key to <see cref="DepotAttributes.Name"/> attribute.
        /// </summary>
        public string EndDepotName { get; set; }
       
        /// <summary>
        /// The earliest allowable starting time for the route.
        /// This attribute can't contain null values and has a default time-only value of 8:00 AM 
        /// on the date given by the default_date parameter or the current date if unspecified.
        /// </summary>
        public long EarliestStartTime { get; set; }
        /// <summary>
        /// The latest allowable starting time for the route. 
        /// This attribute can't contain null values and has a default time-only value of 10:00 AM; 
        /// the default value is interpreted as 10:00 a.m. on the date given by the default_date parameter.
        /// </summary>
        public long LatestStartTime { get; set; }
        /// <summary>
        /// tores the amount of travel time needed to accelerate the vehicle to normal travel speeds, decelerate it to a stop, and move it off and on the network (for example, in and out of parking).
        /// </summary>
        public int? ArriveDepartDelay { get; set; }
        /// <summary>
        /// The maximum capacity of the vehicle. You can specify capacity in any dimension you want, such as weight, volume, or quantity. You can even specify multiple dimensions, for example, weight and volume.
        /// 
        /// References <see cref="OrderAttributes.PickupQuantities"/> and <see cref="OrderAttributes.DeliveryQuantities"/>
        /// </summary>
        /// !----------- used in data mapper : vehicle capacities------------!
        
        public string Capacities { get; set; }

        #region Costs
        /// <summary>
        /// A fixed monetary cost that is incurred only if the route is used in a solution (that is, it has orders assigned to it).
        /// </summary>
        public double? FixedCost { get; set; }
        /// <summary>
        /// The monetary cost incurred—per unit of work time—for the total route duration, including travel times as well as service times and wait times at orders, depots, and breaks.
        /// This attribute can't contain a null value and has a default value of 1.0.
        /// </summary>
        public double CostPerUnitTime { get; set; }
        /// <summary>
        /// The monetary cost incurred—per unit of distance traveled—for the route length (total travel distance).
        /// </summary>
        public double? CostPerUnitDistance { get; set; }
        /// <summary>
        /// The duration of regular work time before overtime computation begins.
        /// </summary>
        public double? OverTimeStartTime { get; set; }
        /// <summary>
        /// The monetary cost incurred per time unit of overtime work.
        /// </summary>
        public double? CostPerUnitOvertime { get; set; }
        /// <summary>
        /// The maximum allowable number of orders on the route.
        /// </summary>
        public int MaxOrderCount { get; set; }
        #endregion

        /// <summary>
        /// The maximum allowable route duration.
        /// </summary>
        public double? MaxTotalTime { get; set; }
        /// <summary>
        /// The maximum allowable travel time for the route. The travel time includes only the time spent driving on the streets and does not include service or wait times.
        /// </summary>
        public double? MaxTotalTravelTime { get; set; }
        /// <summary>
        /// The maximum allowable travel distance for the route.
        /// </summary>
        public double? MaxTotalDistance { get; set; }
        /// <summary>
        /// A space-separated string containing the names of the specialties supported by the route.
        /// This attribute is a foreign key to the <see cref="OrderAttributes.SpecialtyNames"/> attribute and so the values must match.
        /// </summary>
        public string SpecialtyNames { get; set; }
       
    }
}
