using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRPWebApp.Enums;

namespace VRPWebApp.Models
{
    public class ConfigSpecification
    {
      //  public int IdConfiguration { get; set; }
        public AlgorithmType AlgorithmMode { get; set; }
        public string TimeLimit { get; set; }
        public string SolutionLimit { get; set; }
        public string DataIndex { get; set; }
        public DateTime DateTimeRequest { get; set; }
    }
}
