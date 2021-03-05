using System;
using Google.OrTools.ConstraintSolver;

namespace ORTools
{
    public class VRPResult
    {
        public RoutingModel Routing { get; set; }
        public RoutingIndexManager Manager { get; set; }
        public Assignment Solution { get; set; }
    }

    // KAJ MAI
}
