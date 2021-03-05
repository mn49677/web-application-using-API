using System;
using System.Collections.Generic;


namespace AlternativeAlgorithm
{
    public class ManhattanDistance
    {
        public Node[] Nodes;
        public ManhattanDistance(Node[] nodes)
        {
            this.Nodes = nodes;
        }
        /// <summary>
        /// K - constant represents maximum distance from iteration node which is examined,
        /// M - number of closest nodes to iteration nodes taken to examine and maybe use alternate algorithm
        /// out - DistanceMatrix
        /// </summary>  
        public long[,] DistanceMatrix()
        {
            // 
            int N = Nodes.Length;
            long[,] distanceMatrix = new long[N, N];

            /// Računanje Manhattan udaljenosti za svaki čvor
            for (int row = 0; row < N; ++row)
            {
                distanceMatrix[row, row] = 0;
                for (int rowElement = row; rowElement < N; ++rowElement)
                {
                    // Simetrična matrica 
                    long distance = (long)ManhattanDistanceForNode(
                        Nodes[row].Latitude, Nodes[rowElement].Latitude,
                        Nodes[row].Longitude, Nodes[rowElement].Longitude);
                    distanceMatrix[row, rowElement] = distance;
                    distanceMatrix[rowElement, row] = distance;


                }
            }
            return distanceMatrix;
        }
        /// <summary>
        /// Manhattan distance in metric system (meters)
        /// </summary>
        /// <param name="x1 x2 y1 y2">Value of x and y of point 1 and point 2 in coordinate system</param>
        /// <returns>long value which represents distance in meters</returns>
        private double ManhattanDistanceForNode(double x1, double x2, double y1, double y2)
        {
            //First, we get the latitude difference:
            //Δφ = | Δ2 - Δ1 |
            //Δφ = | 37.5613 - 37.5776 | = 0.0163
            //Now, the longitude difference:
            //Δλ = | λ2 - λ1 |
            //Δλ = | 126.978 - 126.973 | = 0.005

            //a = sin²(Δφ / 2)
            //c = 2 ⋅ atan2( √a, √(1−a) )
            //latitudeDistance = R ⋅ c // R is the Earth's radius, 6,371km
            //Now, the longitude distance, as if the latitude was 0:

            //a = sin²(Δλ / 2)
            //c = 2 ⋅ atan2( √a, √(1−a) )
            //longitudeDistance = R ⋅ c // R is the Earth's radius, 6,371km
            //Finally, just add up | latitudeDistance | + | longitudeDistance |.

            int R = 6371;
            double a = Math.Sin(Math.Abs(x1 - x2) / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double latitideDistance = R * c;

            a = Math.Sin(Math.Abs(y1 - y2));
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double longitudeDistance = R * c;

            return (Math.Abs(latitideDistance) + Math.Abs(longitudeDistance)) * 1000;
        }
    }
}
