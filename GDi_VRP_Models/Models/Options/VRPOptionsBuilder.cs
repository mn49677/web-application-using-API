using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Options
{
    /// <summary>
    /// Options for starting a route optimization job
    /// </summary>
    public class VRPOptionsBuilder
    {
        public string TimeUnits { get; set; }
        public string DistanceUnits { get; set; }
        public string UTurnPolicy { get; set; }
        public bool? PopulateDirections { get; set; }
        public string DirectionsLanguage { get; set; }
        public decimal? DefaultDate { get; set; }
        public string F { get; set; }
        public string TimeZoneUsageForTimeFields { get; set; }
        public string Impedance { get; set; }
        public string TravelMode { get; set; }
        public string RouteLineSimplificationTolerance { get; set; }
        public string OutSpatialReference { get; set; }

        /// <summary>
        /// Scheduled tasks will not be rescheduled, and will keep their previous dates. This does not stop from changing the scheduled resource.
        /// </summary>
        public bool PreserveScheduledTimes { get; set; }
        /// <summary>
        /// Scheduled tasks will not be rescheduled to another resource. This does not stop them from changing their dates.
        /// </summary>
        public bool PreserveScheduledResources { get; set; }
        /// <summary>
        /// Preserve relative order of tasks appearing on a route
        /// </summary>
        public bool PreserveSequence { get; set; }
        /// <summary>
        /// Time zone to convert local dates from.
        /// Most of the parameters are in UTC but some, like location working hours, are not.
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }

        public VRPOptionsBuilder()
        {
            this.TimeUnits = "Minutes";
            this.DistanceUnits = "Kilometers";
            this.UTurnPolicy = "NO_UTURNS";
            this.PopulateDirections = true;
            this.DirectionsLanguage = "en";
            this.TimeZoneUsageForTimeFields = "UTC";
            this.TravelMode = "Custom";
            this.Impedance = "Drive Time";
            this.RouteLineSimplificationTolerance = "{\"distance\":20,\"units\":\"esriMeters\"}";
            this.F = "json";
            this.OutSpatialReference = "{\"wkid\":4326}";

            PreserveScheduledTimes = false;
            PreserveScheduledResources = false;
            PreserveSequence = false;

            CostPerUnitTime = 1.0;
            MaxOrderCount = 30;
            MaxTotalTime = 8 * 60;
            ArriveDepartDelay = null;

            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        }

        public SubmitJobParams SubmitJobParams
        {
            get
            {
                return new SubmitJobParams()
                {
                    time_units = TimeUnits,
                    distance_units = DistanceUnits,
                    uturn_policy = UTurnPolicy,
                    populate_directions = PopulateDirections,
                    directions_language = DirectionsLanguage,
                    time_zone_usage_for_time_fields = TimeZoneUsageForTimeFields,
                    travel_mode = TravelMode,
                    impedance = Impedance,
                    route_line_simplification_tolerance = RouteLineSimplificationTolerance,
                    f = F,
                    out_spatial_reference = OutSpatialReference
                };
            }
        }

        public double CostPerUnitTime { get; set; }
        public int MaxOrderCount { get; set; }
        public int? MaxTotalTime { get; set; }
        public int? ArriveDepartDelay { get; set; }
    }
}
