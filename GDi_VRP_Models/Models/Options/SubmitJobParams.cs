using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDi_VRP_Models.Models.Options
{
    public class SubmitJobParams
    {
        public string time_units { get; set; }

        public string distance_units { get; set; }

        public string uturn_policy { get; set; }

        public bool? populate_directions { get; set; }

        public string directions_language { get; set; }

        public decimal? default_date { get; set; }
        public string f { get; set; }
        //public string token { get; set; }
        public string time_zone_usage_for_time_fields { get; set; }
        public string impedance { get; set; }
        public string travel_mode { get; set; }
        public string route_line_simplification_tolerance { get; set; }

        public string out_spatial_reference { get; set; }
    }
}
