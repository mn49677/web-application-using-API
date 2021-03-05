using System;
namespace AlternativeAlgorithm
{
    public static class Haversine
    {
        public static double Calculate(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6372.8; // In kilometers
            var dLat = toRadians(lat2 - lat1);
            var dLon = toRadians(lon2 - lon1);
            lat1 = toRadians(lat1);
            lat2 = toRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c * 1000; // u metrima
        }
        private static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static long[,] Calculate(double[,] locations)
        {
            int N = locations.GetLength(0);
            long[,] distanceMatrix = new long[N, N];

            for (int row = 0; row < N; ++row)
            {
                distanceMatrix[row, row] = 0;
                for (int rowElement = row + 1; rowElement < N; ++rowElement)
                {
                    // Simetrična matrica 
                    double distance = Calculate(locations[row, 0], locations[row, 1], locations[rowElement, 0], locations[rowElement, 1]);
                    distanceMatrix[row, rowElement] = (long)distance;
                    distanceMatrix[rowElement, row] = (long)distance;
                }
            }
            return distanceMatrix;
        }

    }
}