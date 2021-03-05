using System;

namespace ORTools
{
    public class DataModel
    {
        public int VehicleNumber { get; set; } // broj vozila
        //ukupni kapacitet vozila - zbroj svih pojedinacnih kapaciteta
        public long[] VehicleCapacities { get; set; } // kapaciteti svakog vozila
        public long[] Demands { get; set; } // potražnja (količina) potrebna za određeni čvor
        public int[][] PickupsDeliveries { get; set; } // -
        public long[,] TimeMatrix { get; set; } // matrica vremena
        public long[,] TimeWindows { get; set; } // vremenski prozori

        public long[,] VehicleStartingTimes { get; set; }
        public double[ , ] Locations { get; set; }
        public string [] LocationNames { get; set; }
        public int VehicleLoadTime { get; set; } // vrijeme punjenja vozila
        public long VehicleUnloadTime { get; set; } // vrijeme praznjenja vozila

        public int DepotCapacity; // broj vozila koje se mogu puniti u isto vrijeme
        public long[] VehicleLocationUnloadTime { get; set; } // service time
        public int Depot { get; set; } // id, index skladišta
        
        public long[][] DemandsByType { get; set; } 
        
        //npr [0] banane, [0][1]  - kapacitet vozila 1 za banane, [0][2] - kapacitet vozila 2 za banane
        public long[][] VehicleCapacitiesByType { get; set; }

        public String[][] SpecialtyNamesVehicles { get; set; }
        public string SpecialtyCrewVehicle { get; set; }

        public String[][] SpecialtyNamesLocations { get; set; }
        public string SpecialtyCrewLocation { get; set; }
        //for multiple starting points - depots
        public int[] Starts { get; set; }
        //for multiple ending points 
        public int[] Ends { get; set; }

        public double[] VehicleWorkingHours { get; set; }

        public static DataModel GetMockData()
        {
            DataModel model = new DataModel();
            model.VehicleNumber = 4;
            
            model.VehicleCapacities = new long[] { 50, 50, 50, 50};
            model.Demands = new long[] { 0, 0, 1, 2, 4, 2, 4, 8, 8, 1, 2, 1, 2, 4, 4, 8, 8 };
            model.VehicleLoadTime = 5;
            model.VehicleUnloadTime = 5;
            model.VehicleLocationUnloadTime = new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            model.DepotCapacity = 1;
            model.Starts = new int[] { 0, 1, 0, 0 };
            model.Ends = new int[] { 0, 0, 0, 0 };
            model.VehicleCapacitiesByType = new long[][] { new long[] { 10, 20, 15, 20}, new long[] { 20, 20, 10, 10},
                                                                 new long[] { 10, 10, 15 , 20} };
            model.DemandsByType = new long[][] { new long[] {0,0,0 } , new long[] { 0, 1, 0 } ,  new long[] { 1, 2, 0 },
                                                    new long[] {0,0,2 },new long[] {1,1,1 }, new long[] {1,0,2 },
                                                    new long[] {3,3,0 }, new long[] {0,3,0 },
                                                    new long[] {3,2,0 },new long[] {1,1,1 },new long[]{2,3,1 },
                                                    new long[]{5,0,0 },new long[] {5,4,1 },new long[] {1,0,0 },
                                                    new long[] {1,0,3 },new long[] {2,0,3 },new long[] {0,4,0 }};
            model.TimeMatrix = new long[,] {
                {0, 6, 9, 8, 7, 3, 6, 2, 3, 2, 6, 6, 4, 4, 5, 9, 7},
                {6, 0, 8, 3, 2, 6, 8, 4, 8, 8, 13, 7, 5, 8, 12, 10, 14},
                {9, 8, 0, 11, 10, 6, 3, 9, 5, 8, 4, 15, 14, 13, 9, 18, 9},
                {8, 3, 11, 0, 1, 7, 10, 6, 10, 10, 14, 6, 7, 9, 14, 6, 16},
                {7, 2, 10, 1, 0, 6, 9, 4, 8, 9, 13, 4, 6, 8, 12, 8, 14},
                {3, 6, 6, 7, 6, 0, 2, 3, 2, 2, 7, 9, 7, 7, 6, 12, 8},
                {6, 8, 3, 10, 9, 2, 0, 6, 2, 5, 4, 12, 10, 10, 6, 15, 5},
                {2, 4, 9, 6, 4, 3, 6, 0, 4, 4, 8, 5, 4, 3, 7, 8, 10},
                {3, 8, 5, 10, 8, 2, 2, 4, 0, 3, 4, 9, 8, 7, 3, 13, 6},
                {2, 8, 8, 10, 9, 2, 5, 4, 3, 0, 4, 6, 5, 4, 3, 9, 5},
                {6, 13, 4, 14, 13, 7, 4, 8, 4, 4, 0, 10, 9, 8, 4, 13, 4},
                {6, 7, 15, 6, 4, 9, 12, 5, 9, 6, 10, 0, 1, 3, 7, 3, 10},
                {4, 5, 14, 7, 6, 7, 10, 4, 8, 5, 9, 1, 0, 2, 6, 4, 8},
                {4, 8, 13, 9, 8, 7, 10, 3, 7, 4, 8, 3, 2, 0, 4, 5, 6},
                {5, 12, 9, 14, 12, 6, 6, 7, 3, 3, 4, 7, 6, 4, 0, 9, 2},
                {9, 10, 18, 6, 8, 12, 15, 8, 13, 9, 13, 3, 4, 5, 9, 0, 9},
                {7, 14, 9, 16, 14, 8, 5, 10, 6, 5, 4, 10, 8, 6, 2, 9, 0},

            };

                model.TimeWindows = new long[,]{
                {0, 5},    // depot
                {7, 12},   // 1
                {10, 15},  // 2
                {16, 18},  // 3
                {10, 13},  // 4
                {0, 5},    // 5
                {5, 10},   // 6
                {0, 4},    // 7
                {5, 10},   // 8
                {0, 3},    // 9
                {10, 16},  // 10
                {10, 15},  // 11
                {0, 5},    // 12
                {5, 10},   // 13
                {7, 8},    // 14
                {10, 15},  // 15
                {11, 15},  // 16
              };
        model.Depot = 0;

            return model;

        }
        public static DataModel GetMockDataS()
        {
            DataModel model = new DataModel
            {
                VehicleNumber = 2,
                Starts = new int[]{0, 0},
                Ends = new int[] {0,0 },
                VehicleLoadTime = 2,
                VehicleUnloadTime = 2,
                VehicleCapacities = new long[] { 80, 80 },
                Depot = 0,
                DepotCapacity = 1,
                VehicleLocationUnloadTime = new long[] { 0, 1, 1, 1, 1},
                VehicleCapacitiesByType = new long[][] { new long[] { 20, 15}, new long[] { 20, 15 } },
               
                DemandsByType = new long[][] { new long[] { 0, 0 },new long[] { 12, 5}, new long[] { 4, 7},
                    new long[] { 10, 3}, new long[] { 1, 2}},
                Demands = new long[] { 0, 4, 2, 3, 1 },
                SpecialtyNamesVehicles = new string[2][] { new string[] { "10", "12", "14", "LDC_Zagreb" }, new string[] { "16", "20", "LDC_Zagreb"} },
                SpecialtyNamesLocations = new string[][] { new string[] {" 0", "0", "0" },new string[] { "10", "12", "LDC_Zagreb" }, new string[] {"12" , "LDC_Zagreb"},
                    new string[] {"12", "16", "LDC_Zagreb"}, new string[] { "20", "LDC_Zagreb"} },
                TimeMatrix = new long[,]
            {
                { 0, 2, 6, 3, 4},
                { 2, 0, 5, 4, 3}, //1
                { 6, 5, 0, 2, 3}, //2
                { 3, 4, 2, 0, 1}, //3
                { 4, 3, 3, 1, 0}  //4
            },
                TimeWindows = new long[,]
            {
                { 0, 5},
                { 3, 7},
                { 5, 9},
                { 11, 13},
                { 9, 12}

            },
                Locations = new double[,] {
                                           {15.969988, 45.803403}, // longitude latitude ZAgreb 
                                           {16.834784, 45.901936}, // Bjelovar
                                           {16.355739, 48.200272}, // Beč
                                           {18.214190, 46.070907}, // Pecs
                                           {15.2314, 44.1194}}
        };
            return model;
        }
    }
}
