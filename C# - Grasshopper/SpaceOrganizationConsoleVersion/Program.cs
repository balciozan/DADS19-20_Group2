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
public class Script_Instance : GH_ScriptInstance
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
    private void RunScript(object x, int Width, int Length, int Height, int LevelHeight, int cellSize, Box ConnectionPoint, Box ConnectionPointMargin, Box ChangeOverZone, Box ChangeOverZoneMargin, ref object A, ref object R, ref object G, ref object B, ref object Y, ref object K, ref object W, ref object Bxx, ref object Furnitures, ref object FurnituresMargins)
    {


        List<Box> entranceBoxes = new List<Box>();
        List<Box> commonBoxes = new List<Box>();
        List<Box> wetAreaBoxes = new List<Box>();

        /*List<Box> kitchenBoxes = new List<Box>();
        List<Box> commonAreaBoxes = new List<Box>();*/

        List<Box> WhiteBoxes = new List<Box>();

        /**  These sizes are Spreading Limits for the spaces   //total cells     **/

        double entranceSize = (2 * 100 * 100) / (cellSize * cellSize);
        double commonSize = (3 * 100 * 100) / (cellSize * cellSize);
        double wetAreaSize = (1 * 100 * 100) / (cellSize * cellSize);

        /*double kitchenSize = 7 * 100 * 100 / cellSize * cellSize;
        double commonAreaSize = 10 * 100 * 100 / cellSize * cellSize;
      */

        Point3d pA = new Point3d(0, 0, 0);
        Point3d pB = new Point3d(10, 0, 0);
        Point3d pC = new Point3d(0, 10, 0);

        Plane p = new Plane(pA, pB, pC);

        int width = Width;
        int length = Length;
        int levelHeight = LevelHeight;
        int height = Height;
        int levelNumber = Height / LevelHeight;

        int[,,] areaArray = InitializeZeroMatrice(length, width, levelNumber);
        StartZones(areaArray);

        // int[,,] emptyFullArray = InitializeZeroMatrice(length, width, levelNumber);

        for (int i = 0; i < width * length * 3; i++)
        {
            Rhino.RhinoApp.WriteLine("\n Loop Runs" + i.ToString());

            Spread(areaArray, ref entranceSize, entranceList, ref entranceCounter, entranceLimit, cellSize);
            Rhino.RhinoApp.WriteLine("Entrance size" + entranceSize.ToString());
            Rhino.RhinoApp.WriteLine("Entrance counter" + entranceCounter.ToString());
            Rhino.RhinoApp.WriteLine("\n ");


            Spread(areaArray, ref commonSize, commonList, ref commonCounter, commonLimit, cellSize);
            Rhino.RhinoApp.WriteLine("Common size" + commonSize.ToString());
            Rhino.RhinoApp.WriteLine("Common counter" + commonCounter.ToString());

            Spread(areaArray, ref wetAreaSize, wetAreaList, ref wetAreaCounter, wetAreaLimit, cellSize);

            /*Spread(areaArray, ref kitchenSize, kitchenList, ref kitchenCounter, kitchenLimit, cellSize);
            Spread(areaArray, ref commonAreaSize, commonAreaList, ref commonAreaCounter, commonAreaLimit, cellSize);*/
        }

        noiseFilter(areaArray, 0, 6, 5);
        noiseFilter(areaArray, 1, 2, 2);

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
        /*
            for (int k = 0;k < levelNumber;k++)
            {
              for (int i = 0;i < length;i++)
              {
                for (int j = 0;j < width;j++)
                {
                  emptyFullArray[i, j, k] = 0;
                }
              }
            }
        */
        entranceList.Clear();
        commonList.Clear();
        wetAreaList.Clear();
        /*kitchenList.Clear();
        commonAreaList.Clear();*/

        for (int k = 0; k < levelNumber; k++)
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (areaArray[i, j, k] == 1)
                    {
                        Interval xInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                        Interval yInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                        Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                        Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                        entranceBoxes.Add(boxgh);
                        entranceList.AddRange(new int[2] { i, j });
                    }
                    if (areaArray[i, j, k] == 2)
                    {
                        Interval xInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                        Interval yInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                        Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                        Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                        wetAreaBoxes.Add(boxgh);
                        wetAreaList.AddRange(new int[2] { i, j });
                    }
                    if (areaArray[i, j, k] == 3)
                    {
                        Interval xInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                        Interval yInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                        Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                        Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                        commonBoxes.Add(boxgh);
                        commonList.AddRange(new int[2] { i, j });
                    }
                    /*if (areaArray[i, j, k] == 4)
                    {
                      Interval xInterval = new Interval(j * cellSize, (j + 1) * (cellSize ));
                      Interval yInterval = new Interval(i * cellSize, (i + 1) * (cellSize ));
                      Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                      Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                      kitchenBoxes.Add(boxgh);
                      kitchenList.AddRange(new int[2] { i, j });
                    }
                    if (areaArray[i, j, k] == 5)
                    {
                      Interval xInterval = new Interval(j * cellSize, (j + 1) * (cellSize ));
                      Interval yInterval = new Interval(i * cellSize, (i + 1) * (cellSize ));
                      Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                      Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                      commonAreaBoxes.Add(boxgh);
                      commonAreaList.AddRange(new int[2] { i, j });
                    }*/
                    if (areaArray[i, j, k] == 0)
                    {

                        Interval xInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                        Interval yInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                        Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                        Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                        WhiteBoxes.Add(boxgh);
                        //WhiteList.AddRange(i, j);
                    }
                }
            }
        }
        /*
          var connectionPointO = new Objects
            {
              Name = "changeOverZone",
              ZoneName = 2,

              Obj = ConnectionPoint,
              ObjMargin = ConnectionPointMargin,

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
              SpaceList = entranceList,

              //PreviousObject = changeOverZoneO

              };

          var changeOverZoneO = new Objects
            {
              Name = "changeOverZone",
              ZoneName = 2,

              Obj = ChangeOverZone,
              ObjMargin = ChangeOverZoneMargin,

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

              PreviousObject = connectionPointO,
              SpaceList = entranceList,

              };

          connectionPointO.placeRefObject();
      */
        /*
            var randd = new Random();
            //var entranceList = new List<int>();
            //entranceList.Add(3);entranceList.Add(4);
            Rhino.RhinoApp.WriteLine(entranceList.Count().ToString());
            var randomIndex = randd.Next(entranceList.Count / 2);
            int a = entranceList[2 * randomIndex];
            int b = entranceList[2 * randomIndex + 1];

            Point3d pointa = new Point3d(0 * cellSize, b * cellSize, 0);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Box boxx = new Box(planebase, ConnectionPoint.Y, ConnectionPoint.X, ConnectionPoint.Z);

            List<Box> ObjectsList = new List<Box>();
            List<Box> MarginsList = new List<Box>();


        */

        /*
            //var entranceList = new List<int>();
            //entranceList.Add(3);entranceList.Add(4);
            Rhino.RhinoApp.WriteLine(wetAreaList.Count().ToString());
            var randomIndex = randd.Next(wetAreaList.Count / 2);
            int a = entranceList[2 * randomIndex];
            int b = entranceList[2 * randomIndex + 1];

            Point3d pointa = new Point3d(0 * cellSize, b * cellSize, 0);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);


            Box boxx = new Box(planebase, ChangeOverZone.Y, ChangeOverZone.X, ChangeOverZone.Z);
            */

        ShowMatrix(areaArray);

        //Bxx = boxx;
        A = areaArray;
        R = entranceBoxes;
        G = commonBoxes;
        B = wetAreaBoxes;
        /*Y = kitchenBoxes;
        K = commonAreaBoxes;*/
        W = WhiteBoxes;
        //Furnitures = ObjectsList;
        //FurnituresMargins = MarginsList;

    }

    // <Custom additional code> 

    class Objects
    {
        public string Name;
        public int ZoneName;

        public Box Obj;
        public Box ObjMargin;

        public bool Front = false;
        public bool Right = false;
        public bool Back = false;
        public bool Left = false;
        public bool Top = false;
        public bool Bottom = false;

        public bool RotationBool = true;
        public int RotationOpt; // rotation options

        public bool MirrorBool = true;
        public int MirrorOpt; // mirror options

        public string Source; // Water, Electiricity
        public int FixedToWall; // Bed, Television //1,2,3 conditions
        public int CellSize;

        public Objects PreviousObject;

        int ObjLength;
        int ObjWidth;
        int ObjHeight;

        int MrjLength;
        int MrjWidth;
        int MrjHeight;

        public double baseX, baseY, baseZ;
        public List<int> SpaceList = new List<int>();

        //deneme
        public Objects()
        {
            /*
            var Coordinates = new List<int>();
            Point3d pA = new Point3d(0, 0, 0);
            Point3d pB = new Point3d(10, 0, 0);
            Point3d pC = new Point3d(0, 10, 0);

            Plane p = new Plane(pA, pB, pC);*/
        }

        public Objects(string name, int zoneName, bool front, bool right, bool back, bool left, bool top, bool bottom, bool rotationalBool, int rotationOpt, bool mirrorBool, int mirrorOpt, string source, int fixedTowall, int cellSize, Box obj, Box objMargin, Objects previousObject, List<int> spaceList)
          : this()
        {
            this.Obj = obj;
            this.ObjMargin = objMargin;

            this.Name = name;
            this.ZoneName = zoneName;

            this.Front = front;
            this.Right = right;
            this.Back = back;
            this.Left = left;
            this.Bottom = bottom;
            this.Top = top;

            this.RotationBool = rotationalBool;
            this.RotationOpt = rotationOpt;

            this.MirrorBool = mirrorBool;
            this.MirrorOpt = mirrorOpt;

            this.Source = source;
            this.FixedToWall = fixedTowall;
            this.CellSize = cellSize;

            ObjWidth = (int)(Obj.X.Length);
            ObjLength = (int)(Obj.Y.Length);
            ObjHeight = (int)(Obj.Z.Length);

            MrjWidth = (int)(ObjMargin.X.Length);
            MrjLength = (int)(ObjMargin.Y.Length);
            MrjHeight = (int)(ObjMargin.Z.Length);

            baseX = Obj.Plane.OriginX;
            baseY = Obj.Plane.OriginY;
            baseZ = Obj.Plane.OriginZ;

            this.PreviousObject = previousObject;
            this.SpaceList = spaceList;

        }

        public Objects(string name, int zoneName, bool front, bool right, bool back, bool left, bool top, bool bottom, bool rotationalBool, int rotationOpt, bool mirrorBool, int mirrorOpt, string source, int fixedTowall, int cellSize, Box obj, Box objMargin, List<int> spaceList)
          : this()
        {
            this.Obj = obj;
            this.ObjMargin = objMargin;

            this.Name = name;
            this.ZoneName = zoneName;

            this.Front = front;
            this.Right = right;
            this.Back = back;
            this.Left = left;
            this.Bottom = bottom;
            this.Top = top;

            this.RotationBool = rotationalBool;
            this.RotationOpt = rotationOpt;

            this.MirrorBool = mirrorBool;
            this.MirrorOpt = mirrorOpt;

            this.Source = source;
            this.FixedToWall = fixedTowall;
            this.CellSize = cellSize;

            ObjWidth = (int)(Obj.X.Length);
            ObjLength = (int)(Obj.Y.Length);
            ObjHeight = (int)(Obj.Z.Length);

            MrjWidth = (int)(ObjMargin.X.Length);
            MrjLength = (int)(ObjMargin.Y.Length);
            MrjHeight = (int)(ObjMargin.Z.Length);

            baseX = Obj.Plane.OriginX;
            baseY = Obj.Plane.OriginY;
            baseZ = Obj.Plane.OriginZ;

            this.SpaceList = spaceList;

        }

        public void placeFront(int[,,] emptyFullArray, int[,,] areaArray)
        { /*
      this.baseX = PreviousObject.baseX + PreviousObject.MrjLength;
      this.baseY = PreviousObject.baseY;
      this.baseZ = PreviousObject.baseZ;
      */
            bool emptyBool = true;
            bool sameZone = true;

            double XCell = Math.Ceiling((double)(this.MrjWidth / CellSize));
            double YCell = Math.Ceiling((double)(this.MrjLength / CellSize));
            double ZCell = Math.Ceiling((double)(this.MrjHeight / CellSize));

            for (int i = (int)(PreviousObject.baseX / CellSize); i < PreviousObject.baseX / CellSize + (int)XCell; i++)
            {
                for (int j = (int)(PreviousObject.baseY / CellSize); j < PreviousObject.baseY / CellSize + (int)YCell; j++)
                {
                    for (int k = (int)(PreviousObject.baseZ / CellSize); k < PreviousObject.baseZ / CellSize + (int)ZCell; k++)
                    {
                        if (emptyFullArray[i, j, k] == 1)
                        {
                            emptyBool = false;
                        }
                        if (areaArray[i, j, k] != this.ZoneName)
                        {
                            sameZone = false;
                        }

                    }
                }
            }

            if (emptyBool && sameZone)
            {
                this.baseX = PreviousObject.baseX + CellSize * XCell;
                this.baseY = PreviousObject.baseY;
                this.baseZ = PreviousObject.baseZ;
            }

            for (int i = 0; i < XCell; i++)
            {
                for (int j = 0; j < YCell; j++)
                {
                    for (int k = 0; k < ZCell; k++)
                    {
                        emptyFullArray[i, j, k] = 1;
                    }
                }
            }

        }

        public void placeRight()
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY + this.MrjWidth;
            this.baseZ = PreviousObject.baseZ;
        }

        public void placeBack()
        {
            //this.ObjMargin.Transform(Transform.Rotation(1 * Math.PI, new Point3d(this.baseX, this.baseY, this.baseZ)));
            //this.Obj.Transform(Transform.Rotation(1 * Math.PI, new Point3d(this.baseX, this.baseY, this.baseZ)));

            this.baseX = PreviousObject.baseX - this.MrjLength;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        public void placeLeft()
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY - PreviousObject.MrjWidth;
            this.baseZ = PreviousObject.baseZ;
        }
        public void placeTop()
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ + PreviousObject.MrjHeight;
        }

        public void placeBottom()
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ - this.MrjHeight;
        }

        public void placeObject()
        {
            if (RotationBool == false)
            {

            }
        }


        public Box placeRefObject()
        {
            var rand = new Random();
            var randomCellIndex = rand.Next(this.SpaceList.Count / 2);

            baseX = SpaceList[2 * randomCellIndex];
            baseY = SpaceList[2 * randomCellIndex + 1];
            baseZ = 0;

            Point3d pointa = new Point3d(baseX * CellSize, baseY * CellSize, 0);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(baseX * CellSize, ObjWidth);
            Interval yInterval = new Interval(baseY * CellSize, ObjLength);
            Interval zInterval = new Interval(baseZ, ObjHeight);

            Box boxx = new Box(planebase, yInterval, xInterval, zInterval);
            return boxx;
        }

        static void rotate()
        {

        }


        static void mirror()
        {

        }

        static void getClosestCell()
        {

        }

        static void createWall()
        {

        }

    }




    static void ShowMatrix(int[,,] Arr)
    {
        for (int i = 0; i < Arr.GetLength(0); i++)
        {
            for (int j = 0; j < Arr.GetLength(1); j++)
            {
                Rhino.RhinoApp.Write(Arr[i, j, 0] + " ");
            }
            Rhino.RhinoApp.WriteLine("Loop Runs" + i.ToString());
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

    /* The Color lists keep the colors in a seperate List in order to choose within them easily instead of searching where they are... */
    static List<int> entranceList = new List<int>();
    static List<int> commonList = new List<int>();
    static List<int> wetAreaList = new List<int>();
    /* static List<int> kitchenList = new List<int>();
     static List<int> commonAreaList = new List<int>();*/

    /**           these counters are used for starting the límitations counts.         **/

    static int entranceCounter = 1;
    static int commonCounter = 1;
    static int wetAreaCounter = 1;
    /*static int kitchenCounter = 1;
    static int commonAreaCounter = 1;*/
    /**           these limits are used for defining max distance that zone can spread.  **/

    static int entranceLimit = 200;
    static int commonLimit = 200;
    static int wetAreaLimit = 200;
    /*static int kitchenLimit = 200;
    static int commonAreaLimit = 100;*/

    static void StartZones(int[,,] arr)
    {
        /**
         * Randomly Starts the color. Takes the Color lists as argumnts
         * (We can delete these argumnents and it can get them as local referance
         * in our case it gives us a chance to choose what to initilize)
         * **/

        Random rand = new Random();
        //----------------------
        /* GetLength(0) and GetLength(1) might be set as variable */
        //Array.GetLength(0) gives the column lenght in other words length of an array
        //Array.GetLength(1) gives the row lenght in other words width of an array
        var a = rand.Next(arr.GetLength(0) / 2, arr.GetLength(0) / 2);
        var b = rand.Next(0, 2);

        Rhino.RhinoApp.WriteLine("started");
        entranceList.AddRange(new int[2] { a, b }); //AddRange ==> Adds multiple elements to a List . We use mod%2 to call the index of the point back instead of stacking them as an another List(or Array) in our list.
        arr[a, b, 0] = 1;// entrance Starting Point
                         //----------------------

        b = rand.Next(0, arr.GetLength(1));
        a = rand.Next(0, arr.GetLength(0));

        commonList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 2; // common Starting Point
                          //----------------------
        a = rand.Next(0, arr.GetLength(0));
        b = rand.Next(0, arr.GetLength(1));

        wetAreaList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 3; // wetArea Starting Point
                          //----------------------
        a = rand.Next(0, arr.GetLength(0));
        b = rand.Next(0, arr.GetLength(1));
        /*
            kitchenList.AddRange(new int[2] { a, b });
            arr[a, b, 0] = 4; // kitchen Starting Point
            //----------------------
            a = rand.Next(0, arr.GetLength(0));
            b = rand.Next(0, arr.GetLength(1));

            commonAreaList.AddRange(new int[2] { a, b });
            arr[a, b, 0] = 5; // commonArea Starting Point
            //----------------------
          */
    }
    /* Calculates the distance between initial point in the color and the possible new point!
     *  (if the possible new point is too far from inital point eleminate it.) */
    static double distanceCalculate(List<int> spaceCoordList, int x2, int y2, int cellSize)
    {
        var distance = cellSize * Math.Sqrt((y2 - spaceCoordList[1]) * (y2 - spaceCoordList[1]) + (x2 - spaceCoordList[0]) * (x2 - spaceCoordList[0]));
        return distance;
    }

    static void Evaluate()
    {
        // according to relations give score to the created zones
        // get the centers of the zones and acoording to the distances and created sizes give
        //positive or negative points and if score is not acceptable re-run and create variatians.
    }


    static void noiseFilter(int[,,] arr, int type, int iterationNumber, int controlNumber)
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
        /*ARGUMENTS
     * The main array which has rectangular form(will be 3d in short) ,
     * spaceSize = Maximum limit that can grow.  ,
     * spaceCoordList =  the cells of the zone which has been already defined. )
     */
        var rand = new Random();
        var directionList = new List<int>();
        int Xcoord;
        int Ycoord;
        bool draw = false;
        int trycount = 0;

        while (draw == false && counter <= spaceSize)
        {
            if (counter <= spaceSize) //Can be deleted. The condition is added to the While Loop Conditions.
            {
                var randomCellIndex = rand.Next(spaceCoordList.Count / 2);

                Xcoord = spaceCoordList[2 * randomCellIndex];
                Ycoord = spaceCoordList[2 * randomCellIndex + 1];

                if (Xcoord < arr.GetLength(0) - 1 && Xcoord > 0 && Ycoord > 0 && Ycoord < arr.GetLength(1) - 1)
                {
                    /* Check the conditions INSIDE borders */
                    if (arr[Xcoord + 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord + 1, Ycoord, cellSize) < spaceLimit) // Check if possible new cell is still white or not!
                        directionList.AddRange(new int[2] { (Xcoord + 1), Ycoord }); //Store possible spread directions in a list.

                    if (arr[Xcoord - 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord - 1, Ycoord, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord - 1), Ycoord }); //Store possible spread directions in a list.

                    if (arr[Xcoord, Ycoord + 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord + 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), Ycoord + 1 }); //Store possible spread directions in a list.

                    if (arr[Xcoord, Ycoord - 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord - 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.

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
                            draw = true;
                            //Print("trycountNumber is ={0}", trycount);
                        }
                    }
                }

                /* EDGE CONDITIONS */
                //RIGHT
                else if (Xcoord == (arr.GetLength(0) - 1) && Ycoord != 0 && Ycoord != (arr.GetLength(1) - 1))
                {
                    if (arr[Xcoord - 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord - 1, Ycoord, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord - 1), (Ycoord) }); //Store possible spread directions in a list.
                    if (arr[Xcoord, Ycoord + 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord + 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), (Ycoord + 1) }); //Store possible spread directions in a list.
                    if (arr[Xcoord, Ycoord - 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord - 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.

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
                            draw = true;
                            //Print("trycountNumber is ={0}", trycount);
                        }
                    }
                }

                //Left
                else if (Xcoord == 0 && Ycoord != 0 && Ycoord != (arr.GetLength(1) - 1))
                {

                    if (arr[Xcoord + 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord + 1, Ycoord, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord + 1), (Ycoord) }); //Store possible spread directions in a list.
                    if (arr[Xcoord, Ycoord + 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord + 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), (Ycoord + 1) }); //Store possible spread directions in a list.
                    if (arr[Xcoord, Ycoord - 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord - 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.
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
                            draw = true;
                            //Print("trycountNumber is ={0}", trycount);
                        }
                    }

                }
                //TOP
                else if ((Ycoord == arr.GetLength(1) - 1) && Xcoord != 0 && Xcoord != (arr.GetLength(0) - 1))
                {

                    if (arr[Xcoord, Ycoord - 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord - 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.
                    if (arr[Xcoord + 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord + 1, Ycoord, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord + 1), (Ycoord) }); //Store possible spread directions in a list.
                    if (arr[Xcoord - 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord - 1, Ycoord, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord - 1), (Ycoord) }); //Store possible spread directions in a list.
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
                            draw = true;
                            //Print("trycountNumber is ={0}", trycount);
                        }
                    }
                }
                //BOTTOM
                else if (Ycoord == 0 && Xcoord != 0 && Xcoord != (arr.GetLength(0) - 1))
                {
                    if (arr[Xcoord, Ycoord + 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord + 1, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord), (Ycoord + 1) }); //Store possible spread directions in a list.
                    if (arr[Xcoord + 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord + 1, Ycoord, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord + 1), (Ycoord) }); //Store possible spread directions in a list.
                    if (arr[Xcoord - 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord - 1, Ycoord, cellSize) < spaceLimit)
                        directionList.AddRange(new int[2] { (Xcoord - 1), (Ycoord) }); //Store possible spread directions in a list.
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
                            draw = true;
                            //Print("trycountNumber is ={0}", trycount);
                        }
                    }
                }

                //LEFT BOTTOM
                else if (Xcoord == 0 && Ycoord == 0)
                {
                    if (arr[Xcoord, Ycoord + 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord + 1, cellSize) < spaceLimit)
                        directionList.Add(Xcoord); directionList.Add(Ycoord + 1);

                    if (arr[Xcoord + 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord + 1, Ycoord, cellSize) < spaceLimit)
                        directionList.Add(Xcoord + 1); directionList.Add(Ycoord);

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
                        //Print(" Counter Number is : {0}", counter);
                    }
                    else
                    {
                        trycount++;
                        if (trycount >= spaceCoordList.Count)
                        {
                            draw = true;
                            //Print("trycountNumber is ={0}", trycount);
                        }
                    }
                }
                //LEFT  TOP
                /*
                else if (Xcoord == 0 && Ycoord == (arr.GetLength(1) - 1))
                {
                if (arr[Xcoord + 1, Ycoord] == 0)
                directionList.Add(Xcoord + 1); directionList.Add(Ycoord);

                if (arr[Xcoord, Ycoord - 1] == 0)
                directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

                if (directionList.Count > 0)
                {
                var randomIndex = rand.Next(directionList.Count / 2);

                int a = directionList[2 * randomIndex];
                int b = directionList[2 * randomIndex + 1];

                Rhino.RhinoApp.WriteLine("Left Top "  +  a +  " "  +  b);
                arr[a, b] = arr[Xcoord, Ycoord];

                spaceCoordList.AddRange(new int[2] { a, b });

                directionList.Clear();
                directionList.TrimExcess();
                draw = true;

                counter++;
                //Print(" Counter Number is : {0}", counter);
                }
                else
                {
                trycount++;
                if (trycount >= spaceCoordList.Count)
                {
                draw = true;
                //Print("trycountNumber is ={0}", trycount);
                }
                }
                }
                */
                /*
                //RIGHT TOP
                else if (Xcoord == (arr.GetLength(0) - 1) && Ycoord == (arr.GetLength(1) - 1))
                {
                if (arr[Xcoord, Ycoord - 1] == 0)
                directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

                if (arr[Xcoord - 1, Ycoord] == 0)
                directionList.Add(Xcoord - 1); directionList.Add(Ycoord);

                if (directionList.Count > 0)
                {
                var randomIndex = rand.Next(directionList.Count / 2);

                int a = directionList[2 * randomIndex];
                int b = directionList[2 * randomIndex + 1];
                Rhino.RhinoApp.WriteLine(" Right top "+ a + " " + b);
                arr[a, b] = arr[Xcoord, Ycoord];

                //spaceCoordList.AddRange(new int[2] { a, b });

                directionList.Clear();
                directionList.TrimExcess();
                draw = true;

                counter++;
                //Print(" Counter Number is : {0}", counter);
                }
                else
                {
                trycount++;
                if (trycount >= spaceCoordList.Count)
                {
                draw = true;
                //Print("trycountNumber is ={0}", trycount);
                }
                }
                }
                */
                //RIGHT BOTTOM
                else if (Xcoord == (arr.GetLength(0) - 1) && Ycoord == 0)
                {
                    if (arr[Xcoord, Ycoord + 1, 0] == 0)
                        directionList.Add(Xcoord); directionList.Add(Ycoord + 1);

                    if (arr[Xcoord - 1, Ycoord, 0] == 0)
                        directionList.Add(Xcoord - 1); directionList.Add(Ycoord);

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
                        //Print(" Counter Number is : {0}", counter);
                    }
                    else
                    {
                        trycount++;
                        if (trycount >= spaceCoordList.Count)
                        {
                            draw = true;
                            //Print("trycountNumber is ={0}", trycount);
                        }
                    }
                }
                /* EDGE CONDITIONS */
            }
        }

    }




    /* Instead of wrting methods seperately in main program, it is better to use class based
     informations to keep things organized, we are not yet transsferentrance methods here but we created the str.*/

    /*
    public class Zone
    {
        public string Name;
        public int SpaceLimit;
        public Int16 SpaceSize;
        public int Counter;
        public List<Int16> SpaceList;

        public void Describe()
        {
            Console.WriteLine("Hey this zone is{ 0} ", Name);
        }

        public Zone()
        {
           // SpaceList = new List<int>();
        }

        public Zone(string name, int spaceLimit,int spacesize,int counter, List<int> SpaceList)
        :this()
        {
        }
        static void InitializeZeroMatrice() { }
        static void startZones() { }
        public void SpreadZones() { }

    }
    */



    // </Custom additional code> 
}