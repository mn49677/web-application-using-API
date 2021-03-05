using System;
namespace AlternativeAlgorithm
{
    public class Node
    {
        public int Index { get; set; }
        public long DistanceFromCurrentNode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Mark { get; set; }
    }
}
