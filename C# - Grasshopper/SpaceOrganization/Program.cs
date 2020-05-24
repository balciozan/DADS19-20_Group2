using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.Linq;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public partial class Script_Instance : GH_ScriptInstance
{
    #region Utility functions
    /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
    /// <param name="text">String to print.</param>
    private void Print(string text) { /* Implementation hidden. */ }
    /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
    /// <param name="format">String format.</param>
    /// <param name="args">Formatting parameters.</param>
    private void Print(string format, params object[] args) { /* Implementation hidden. */ }
    /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
    /// <param name="obj">Object instance to parse.</param>
    private void Reflect(object obj) { /* Implementation hidden. */ }
    /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
    /// <param name="obj">Object instance to parse.</param>
    private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
    #endregion

    #region Members
    /// <summary>Gets the current Rhino document.</summary>
    private readonly RhinoDoc RhinoDocument;
    /// <summary>Gets the Grasshopper document that owns this script.</summary>
    private readonly GH_Document GrasshopperDocument;
    /// <summary>Gets the Grasshopper script component that owns this script.</summary>
    private readonly IGH_Component Component;
    /// <summary>
    /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
    /// Any subsequent call within the same solution will increment the Iteration count.
    /// </summary>
    private readonly int Iteration;
    #endregion

    /// <summary>
    /// This procedure contains the user code. Input parameters are provided as regular arguments,
    /// Output parameters as ref arguments. You don't have to assign output parameters,
    /// they will have a default value.
    /// </summary>
    private void RunScript(object x, int Width, int Length, int Height, int LevelHeight, int cellSize, Box ExteriorDoor, Box ExteriorDoorMargin, Box ChangeOverZone, Box ChangeOverZoneMargin, Box Closet, Box ClosetMargin, Box InteriorDoor, Box InteriorDoorMargin, Box Counter, Box CounterMargin, Box Table, Box TableMargin, Box SittingUnit, Box SittingUnitMargin, Box Bed, Box BedMargin, Box HealtCare, Box HealthCareMargin, Box Toilet, Box ToiletMargin, Box Sink, Box SinkMargin, Box SowerCabin, object ShowerCabinMargin, ref object A, ref object EntranceBoxes, ref object CommonBoxes, ref object WetAreaBoxes, ref object Y, ref object K, ref object W, ref object Furnitures, ref object FurnituresMargins)
    {

        List<Box> entranceBoxes = new List<Box>();
        List<Box> commonBoxes = new List<Box>();
        List<Box> wetAreaBoxes = new List<Box>();

        List<Box> ObjectsList = new List<Box>();
        List<Box> MarginsList = new List<Box>();

        List<Box> WhiteBoxes = new List<Box>();

        double entranceSize = (12 * 100 * 100) / (cellSize * cellSize);
        double commonSize = (7 * 100 * 100) / (cellSize * cellSize);
        double wetAreaSize = (7 * 100 * 100) / (cellSize * cellSize);

        Point3d pA = new Point3d(0, 0, 0);
        Point3d pB = new Point3d(10, 0, 0);
        Point3d pC = new Point3d(0, 10, 0);

        Plane p = new Plane(pA, pB, pC);

        int width = Width;
        int length = Length;
        int levelHeight = LevelHeight;
        int height = Height;
        int levelNumber = Height / LevelHeight;

        bool objectPlacementEnded = false;
        bool placementFailed = false;
        bool validZoning = false;

        int[,,] areaArray = InitializeZeroMatrice(length, width, levelNumber);
        int[,,] emptyFullArray = InitializeZeroMatrice(length, width, levelNumber);

        int objectReplacementCounter = 1;

        while ((objectPlacementEnded == false || placementFailed == true) && objectReplacementCounter < 50)
        {
            Rhino.RhinoApp.WriteLine(" ");
            Rhino.RhinoApp.WriteLine("Object Replacement Counter is:   " + objectReplacementCounter);
            Rhino.RhinoApp.WriteLine(" ");

            placementFailed = false;
            validZoning = false;
            int zoneCounter = 0;

            while (validZoning == false && zoneCounter < 100)
            {
                entranceList.Clear();
                commonList.Clear();
                wetAreaList.Clear();

                commonBoxes.Clear();
                wetAreaBoxes.Clear();
                entranceBoxes.Clear();
                WhiteBoxes.Clear();

                wetAreaCounter = 1;
                commonCounter = 1;
                entranceCounter = 1;

                areaArray = InitializeZeroMatrice(length, width, levelNumber);
                StartZones(areaArray);

                for (int i = 0; i < width * length * 3; i++)
                {
                    Spread(areaArray, ref entranceSize, entranceList, ref entranceCounter, entranceLimit, cellSize);
                    Spread(areaArray, ref commonSize, commonList, ref commonCounter, commonLimit, cellSize);
                    Spread(areaArray, ref wetAreaSize, wetAreaList, ref wetAreaCounter, wetAreaLimit, cellSize);
                }
                NoiseFilter(areaArray, 0, 3, 5);
                NoiseFilter(areaArray, 1, 2, 2);

                entranceList.Clear();
                commonList.Clear();
                wetAreaList.Clear();

                for (int k = 0; k < levelNumber; k++)
                {
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            areaArray[i, j, k] = areaArray[i, j, 0];
                        }
                    }
                }

                for (int k = 0; k < levelNumber; k++)
                {
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            emptyFullArray[i, j, k] = 0;
                        }
                    }
                }

                for (int k = 0; k < levelNumber; k++)
                {
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (areaArray[i, j, k] == 1)
                            {
                                Interval xInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                                Interval yInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                                Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                                Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                                entranceBoxes.Add(boxgh);
                                entranceList.AddRange(new int[2] { i, j });
                            }
                            if (areaArray[i, j, k] == 2)
                            {
                                Interval xInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                                Interval yInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                                Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                                Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                                commonBoxes.Add(boxgh);
                                commonList.AddRange(new int[2] { i, j });
                            }
                            if (areaArray[i, j, k] == 3)
                            {
                                Interval xInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                                Interval yInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                                Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                                Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                                wetAreaBoxes.Add(boxgh);
                                wetAreaList.AddRange(new int[2] { i, j });
                            }
                            if (areaArray[i, j, k] == 0)
                            {
                                Interval xInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                                Interval yInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                                Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                                Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                                WhiteBoxes.Add(boxgh);
                            }
                        }
                    }
                }
                if (60 * 60 * entranceList.Count / 4 >= 100 * 100 * 8 && 60 * 60 * commonList.Count / 4 >= 100 * 100 * 5 && 60 * 60 * wetAreaList.Count / 4 >= 100 * 100 * 5)
                {
                    Rhino.RhinoApp.WriteLine(" ");

                    Rhino.RhinoApp.WriteLine(" VALID ZONING is TRUE");
                    Rhino.RhinoApp.WriteLine("  ");
                    Rhino.RhinoApp.WriteLine(" Enrance Space is :" + 60 * 60 * entranceList.Count / 4 + " > " + 100 * 100 * 6);
                    Rhino.RhinoApp.WriteLine(" entrance Count is :" + entranceList.Count / 4);
                    Rhino.RhinoApp.WriteLine(" commonList Count is :" + commonList.Count / 4);
                    Rhino.RhinoApp.WriteLine(" wetAreaList Count is :" + wetAreaList.Count / 4);
                    Rhino.RhinoApp.WriteLine("  ");
                    validZoning = true;

                }
                else
                {
                    Rhino.RhinoApp.WriteLine(" ");

                    Rhino.RhinoApp.WriteLine(" VALID ZONING is FALSE ");
                    Rhino.RhinoApp.WriteLine(" Enrance Space is :" + 60 * 60 * entranceList.Count / 4 + " > " + 100 * 100 * 6);
                    Rhino.RhinoApp.WriteLine(" entrance Count is :" + entranceList.Count / 4);
                    Rhino.RhinoApp.WriteLine(" commonList Count is :" + commonList.Count / 4);
                    Rhino.RhinoApp.WriteLine(" wetAreaList Count is :" + wetAreaList.Count / 4);
                    validZoning = false;
                    zoneCounter++;
                    Rhino.RhinoApp.WriteLine("Zone Counter is :" + zoneCounter);
                }
            }
            emptyFullArray = InitializeZeroMatrice(length, width, levelNumber);


            ObjectsList.Clear();
            MarginsList.Clear();
            /* FURNITURE LAYOUT ==> ENTRANCE STARTED */

            Point3d pointa = new Point3d(0, 0, 0);
            Vector3d vectorb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, vectorb);

            Interval xInter = new Interval(0, 10);
            Interval yInter = new Interval(0, 10);
            Interval zInter = new Interval(0, 10);

            Box ObjBox = new Box(planebase, xInter, yInter, zInter);
            Box marginBox = new Box(planebase, xInter, yInter, zInter);

            /* ENTRANCE REFERENCE */
            var exteriorDoorO = new Objects
            {
                Name = "Exterior Door",
                ZoneName = 1,

                Obj = ExteriorDoor,
                ObjMargin = ExteriorDoorMargin,

                Front = true,
                Right = true,
                Back = false,
                Left = true,
                Top = false,
                Bottom = false,

                RotationBool = false,
                RotationOpt = 2,

                MirrorBool = false,
                MirrorOpt = 2,

                Source = "NULL",
                FixedToWall = 1,

                CellSize = 60,
                SpaceList = entranceList,

            };
            exteriorDoorO.AssignDimensions();
            exteriorDoorO.PlaceRefObjectEntrance(emptyFullArray, areaArray, out ObjBox, out marginBox, out placementFailed);

            ObjectsList.Add(ObjBox);
            MarginsList.Add(marginBox);

            /* ENTRANCE 2ND OBJECT */

            var changeOverZoneO = new Objects
            {
                Name = "changeOverZone",
                ZoneName = 1,

                Obj = ChangeOverZone,
                ObjMargin = ChangeOverZoneMargin,

                Front = true,
                Right = true,
                Back = true,
                Left = true,
                Top = false,
                Bottom = false,

                RotationBool = false,
                RotationOpt = 2,

                MirrorBool = false,
                MirrorOpt = 2,

                Source = "NULL",
                FixedToWall = 1,
                CellSize = 60,

                PreviousObject = exteriorDoorO,
                SpaceList = entranceList,

            };



            changeOverZoneO.AssignDimensions();
            changeOverZoneO.PlaceObject(emptyFullArray, areaArray, out ObjBox, out marginBox, out placementFailed);
            ObjectsList.Add(ObjBox);
            MarginsList.Add(marginBox);


            var closetO = new Objects
            {
                Name = "closet",
                ZoneName = 1,

                Obj = Closet,
                ObjMargin = ClosetMargin,

                Front = true,
                Right = true,
                Back = false,
                Left = false,
                Top = false,
                Bottom = false,

                RotationBool = false,
                RotationOpt = 2,

                MirrorBool = false,
                MirrorOpt = 2,

                Source = "NULL",
                FixedToWall = 1,
                CellSize = 60,

                PreviousObject = changeOverZoneO,
                SpaceList = entranceList,

            };

            closetO.AssignDimensions();

            closetO.PlaceObject(emptyFullArray, areaArray, out ObjBox, out marginBox, out placementFailed);

            ObjectsList.Add(ObjBox);
            MarginsList.Add(marginBox);

            objectReplacementCounter++;
            objectPlacementEnded = true;
        }

        ShowMatrix(areaArray);

        A = areaArray;
        EntranceBoxes = entranceBoxes;
        CommonBoxes = commonBoxes;
        WetAreaBoxes = wetAreaBoxes;
        /*Y = kitchenBoxes;
        K = commonAreaBoxes;*/
        W = WhiteBoxes;
        Furnitures = ObjectsList;
        FurnituresMargins = MarginsList;
    }

    static void ShowMatrix(int[,,] Arr)
    {
        for (int i = 0; i < Arr.GetLength(0); i++)
        {
            for (int j = 0; j < Arr.GetLength(1); j++)
            {
                Rhino.RhinoApp.Write(Arr[i, j, 0] + " ");
            }
            Rhino.RhinoApp.WriteLine("   ");
        }
        return;
    }

    static int[,,] InitializeZeroMatrice(int width, int length, int levelNumber)
    {
        var areaArray = new int[width, length, levelNumber];

        for (int k = 0; k < levelNumber; k++)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    areaArray[i, j, k] = 0;
                }
            }
        }
        return areaArray;
    }

    static List<int> entranceList = new List<int>();
    static List<int> commonList = new List<int>();
    static List<int> wetAreaList = new List<int>();
    /**           these counters are used for starting the límitations counts.         **/
    static int entranceCounter = 1;
    static int commonCounter = 1;
    static int wetAreaCounter = 1;
    /**           these limits are used for defining max distance that zone can spread.  **/
    static readonly int entranceLimit = 400;
    static readonly int commonLimit = 300;
    static readonly int wetAreaLimit = 300;

    static void StartZones(int[,,] arr)
    {
        Random rand = new Random();
        var a = rand.Next(arr.GetLength(0) / 2, arr.GetLength(0) / 2);
        var b = rand.Next(0, 2);

        Rhino.RhinoApp.WriteLine("Zones started");
        entranceList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 1;
        b = rand.Next(0, arr.GetLength(1));
        a = rand.Next(0, arr.GetLength(0));

        commonList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 2;

        a = rand.Next(0, arr.GetLength(0));
        b = rand.Next(0, arr.GetLength(1));

        wetAreaList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 3; // wetArea Starting Point
                          //----------------------
        a = rand.Next(0, arr.GetLength(0));
        b = rand.Next(0, arr.GetLength(1));

    }

    static double DistanceCalculate(List<int> spaceCoordList, int x2, int y2, int cellSize)
    {
        var distance = cellSize * Math.Sqrt((y2 - spaceCoordList[1]) * (y2 - spaceCoordList[1]) + (x2 - spaceCoordList[0]) * (x2 - spaceCoordList[0]));
        return distance;
    }

    static void NoiseFilter(int[,,] arr, int type, int iterationNumber, int controlNumber)
    {
        if (type == 0)
        {
            for (int loop = 0; loop < iterationNumber; loop++)
            {
                List<int> surrounding = new List<int>();
                for (int i = 1; i < arr.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j < arr.GetLength(1) - 1; j++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            for (int y = -1; y < 2; y++)
                            {
                                if (!(x == y && x == 0))
                                {

                                    surrounding.Add(arr[i + x, j + y, 0]);
                                }
                            }
                        }
                        for (int v = 0; v < 6; v++) // 6 is the number of the zones including  empty cells
                        {
                            int count = (from z in surrounding where z == v select z).Count();
                            if (count >= controlNumber)
                            {
                                arr[i, j, 0] = v;
                            }
                        }
                        surrounding.Clear();
                        surrounding.TrimExcess();
                    }
                }
            }
        }

        if (type == 1)
        {
            for (int loop = 0; loop < iterationNumber; loop++)
            {
                List<int> surrounding = new List<int>();
                for (int i = 1; i < arr.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j < arr.GetLength(1) - 1; j++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            for (int y = -1; y < 2; y++)
                            {
                                if (!(x == y && x == 0) && ((x + y == 1) || (x + y == -1)))
                                {
                                    surrounding.Add(arr[i + x, j + y, 0]);
                                }
                            }
                        }

                        for (int v = 0; v < 6; v++) // 6 is the number of the zones including  empty cells
                        {
                            int count = (from z in surrounding where z == v select z).Count();
                            if (count >= 2)
                            {
                                arr[i, j, 0] = v;
                            }
                        }
                        surrounding.Clear();
                        surrounding.TrimExcess();
                    }
                }
            }
        }
    }

    static void Spread(int[,,] arr, ref double spaceSize, List<int> spaceCoordList, ref int counter, int spaceLimit, int cellSize)
    {
        var rand = new Random();
        var directionList = new List<int>();
        int Xcoord;
        int Ycoord;
        bool draw = false;
        int trycount = 0;

        bool inTop;
        bool inRight;
        bool inBottom;
        bool inLeft;

        bool enoughTries = false;

        while (draw == false && counter < spaceSize && enoughTries == false)
        {
            var randomCellIndex = rand.Next(spaceCoordList.Count / 2);

            Xcoord = spaceCoordList[2 * randomCellIndex];
            Ycoord = spaceCoordList[2 * randomCellIndex + 1];

            inTop = (Ycoord + 1 < arr.GetLength(1));              // Check top border
            inRight = (Xcoord + 1 < arr.GetLength(0));              // Check Right border
            inBottom = (Ycoord - 1 >= 0);              // Check bottom border
            inLeft = (Xcoord - 1 >= 0);             //  Check left border

            if (inRight)
            {
                if (arr[Xcoord + 1, Ycoord, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord + 1, Ycoord, cellSize) < spaceLimit) // Check if possible new cell is still white or not!
                    directionList.AddRange(new int[2] { (Xcoord + 1), Ycoord }); //Store possible spread directions in a list.
            }

            if (inLeft)
            {
                if (arr[Xcoord - 1, Ycoord, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord - 1, Ycoord, cellSize) < spaceLimit)
                    directionList.AddRange(new int[2] { (Xcoord - 1), Ycoord }); //Store possible spread directions in a list.
            }

            if (inTop)
            {
                if (arr[Xcoord, Ycoord + 1, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord, Ycoord + 1, cellSize) < spaceLimit)
                    directionList.AddRange(new int[2] { (Xcoord), Ycoord + 1 }); //Store possible spread directions in a list.
            }

            if (inBottom)
            {
                if (arr[Xcoord, Ycoord - 1, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord, Ycoord - 1, cellSize) < spaceLimit)
                    directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.
            }


            if (directionList.Count > 0)
            {
                var randomIndex = rand.Next(directionList.Count / 2);
                int a = directionList[2 * randomIndex];
                int b = directionList[2 * randomIndex + 1];

                arr[a, b, 0] = arr[Xcoord, Ycoord, 0];

                spaceCoordList.AddRange(new int[2] { a, b });

                directionList.Clear();
                directionList.TrimExcess();
                draw = true;
                counter++;
            }

            else
            {
                trycount++;
                if (trycount >= spaceCoordList.Count)
                {
                    enoughTries = true;
                    //Print("trycountNumber is ={0}", trycount);
                }
            }
        }
    }

    public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
    {
        throw new NotImplementedException();
    }
    // </Custom additional code> 
}