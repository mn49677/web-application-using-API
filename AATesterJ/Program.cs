using System;
using AlternativeAlgorithm;

namespace AATesterJ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            double[,] locations = new double[,] {
                                           {15.969988, 45.803403 }, // longitude latitude ZAgreb 
                                           {16.834784, 45.901936}, // Bjelovar
                                           {16.355739, 48.200272 }, // Beč
                                           {18.214190, 46.070907}, // Pecs
                                           {15.2314, 44.1194}}; // Zadar

            NodeJ[] nodes = new NodeJ[locations.GetLength(0)];
            for (int i = 0; i < locations.GetLength(0); ++i)
            {
                nodes[i] = new NodeJ();
                nodes[i].Latitude = locations[i, 1];
                nodes[i].Longitude = locations[i, 0];
            }
            AlternateAlgorithmJ aaj = new AlternateAlgorithmJ(nodes);
            var newMatrix = aaj.ChangeDistanceMatrix(10000, 3, 0.5, 0.2, locations);

            //double[,] hMtx = aaj.GetHaversineMatrix(locations);

            //PrintMatrix(hMtx);
            PrintMatrix(newMatrix);


        }
        public static void PrintMatrix(double[,] arr)
        {
            int rowLength = arr.GetLength(0);
            int colLength = arr.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0} ", arr[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }

    }
}
