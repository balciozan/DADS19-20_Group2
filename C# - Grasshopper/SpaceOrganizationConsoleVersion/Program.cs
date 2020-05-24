using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Linq.Enumerable;



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
            //exteriorDoorO.AssignDimensions();
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



            //changeOverZoneO.AssignDimensions();
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

            //closetO.AssignDimensions();

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

    public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
    {
        throw new NotImplementedException();
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

        public double ObjLength;
        public double ObjWidth;
        public double ObjHeight;

        public double MrjLength;
        public double MrjWidth;
        public double MrjHeight;

        public double baseX, baseY, baseZ;
        public Vector3d BaseVector;

        public List<int> SpaceList = new List<int>();

        public double XCellMargin;
        public double YCellMargin;
        public double ZCellMargin;

        public double XCellObject;
        public double YCellObject;
        public double ZCellObject;

        public Vector3d VectorMargin;
        public Vector3d VectorObject;

        Plane PlaneBase;

        public Objects()
        {
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

            this.baseX = Obj.Plane.OriginX;
            this.baseY = Obj.Plane.OriginY;
            this.baseZ = Obj.Plane.OriginZ;

            this.PlaneBase = new Plane(PreviousObject.PlaneBase.Origin, new Vector3d(0, 0, 1));

            this.SpaceList = spaceList;
        }

        public Objects(string name, int zoneName, bool front, bool right, bool back, bool left, bool top, bool bottom, bool rotationalBool, int rotationOpt, bool mirrorBool, int mirrorOpt, string source, int fixedTowall, int cellSize, Box obj, Box objMargin, Objects previousObject, List<int> spaceList)
          : this(name, zoneName, front, right, back, left, top, bottom, rotationalBool, rotationOpt, mirrorBool, mirrorOpt, source, fixedTowall, cellSize, obj, objMargin, spaceList)
        {
            this.PreviousObject = previousObject;
        }

        public void AssignDimensions()
        {
            this.ObjWidth = this.Obj.X.T1;
            this.ObjLength = this.Obj.Y.T1;
            this.ObjHeight = this.Obj.Z.T1;

            this.MrjWidth = this.ObjMargin.X.T1;
            this.MrjLength = this.ObjMargin.Y.T1;
            this.MrjHeight = this.ObjMargin.Z.T1;

            this.VectorMargin = new Vector3d(this.ObjLength, this.ObjWidth, this.ObjHeight);
            this.VectorObject = new Vector3d(this.MrjWidth, this.MrjWidth, this.MrjHeight);
        }

        public void PrintDimensions()
        {
            Rhino.RhinoApp.WriteLine("Obj Width " + this.ObjWidth.ToString());
            Rhino.RhinoApp.WriteLine("Obj Length " + this.ObjLength.ToString());
            Rhino.RhinoApp.WriteLine("Obj Height " + this.ObjHeight.ToString());

            Rhino.RhinoApp.WriteLine("Obj Width " + this.MrjWidth.ToString());
            Rhino.RhinoApp.WriteLine("Obj Length " + this.MrjLength.ToString());
            Rhino.RhinoApp.WriteLine("Obj Height " + this.MrjHeight.ToString());
        }

        public void Rotate(double Angle)
        {
            Angle = Angle * (Math.PI / 180);

            VectorMargin.Rotate(Angle, new Vector3d(0, 0, 1));
            VectorObject.Rotate(Angle, new Vector3d(0, 0, 1));
        }

        public void Mirroring(int axis)
        {
            Point3d pointa = new Point3d(this.PlaneBase.OriginX, this.PlaneBase.OriginY, this.PlaneBase.OriginZ);
            Vector3d pointb = new Vector3d();

            if (axis == 1)                //Z Symmetry
            { pointb = new Vector3d(0, 0, 1); }
            else if (axis == 2)                //X Symmtry
            { pointb = new Vector3d(0, 1, 0); }
            else if (axis == 3)                // Y Symmetry
            { pointb = new Vector3d(1, 0, 0); }

            Plane planee = new Plane(pointa, pointb);

            VectorMargin.Transform(Transform.Mirror(planee));
            VectorObject.Transform(Transform.Mirror(planee));
        }
        Vector3d vec = new Vector3d(1, 4, 5);

        public void PlaceRefObject(int[,,] emptyFullArray, int[,,] areaArray, out Box ObjBox, out Box marginBox)
        {

            Point3d pointa = new Point3d(0, 0, 0);
            Vector3d pointb = new Vector3d(0, 0, 1);
            Plane planee = new Plane(pointa, pointb);

            this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
            this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
            this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

            this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
            this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
            this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

            Vector3d.VectorAngle(VectorMargin, new Vector3d(0, 0, 0));

            bool emptyBool = true;
            bool sameZone = true;

            bool placeSuccessful = false;
            int trycount = 0;

            while (placeSuccessful == false)
            {
                var rand = new Random();
                var randomCellIndex = rand.Next(this.SpaceList.Count / 2);

                baseX = (this.SpaceList[2 * randomCellIndex]) * CellSize;
                baseY = (this.SpaceList[2 * randomCellIndex + 1]) * CellSize;
                baseZ = 0;

                if ((baseX + this.VectorMargin.X < 360) && (baseY + this.VectorMargin.Y < 600) && (baseZ + this.VectorMargin.Z < 300) &&
                  (baseX + this.VectorMargin.X > 0) && (baseY + this.VectorMargin.Y > 0) && (baseZ + this.VectorMargin.Z > 0))
                {
                    for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
                    {
                        for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                        {
                            for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                            {
                                //Rhino.RhinoApp.WriteLine("Entrance Reference Object :  " + "  i:  " + i + "  j:  " + j + "  k:  " + k);
                                if (emptyFullArray[i, j, k] == 2 || emptyFullArray[i, j, k] == 1)
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
                        placeSuccessful = true;

                        Rhino.RhinoApp.WriteLine("Reference Object conditions are successful ");
                        for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
                        {
                            for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                            {
                                for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                                {
                                    emptyFullArray[i, j, k] = 2;
                                }
                            }
                        }
                    }
                    else
                    {
                        Rhino.RhinoApp.WriteLine("Conditions are not satisfied ");
                    }
                }

                trycount++;
                Rhino.RhinoApp.WriteLine("Try count is : " + trycount);
                if (trycount > 60)
                    Rhino.RhinoApp.WriteLine("Try count is over 60 placement is unsuccessful: ");
                placeSuccessful = true;
            }

            if (trycount < 60 && placeSuccessful)
            {
                Rhino.RhinoApp.WriteLine("baseX " + this.baseX);
                Rhino.RhinoApp.WriteLine("baseY " + this.baseY);
                Rhino.RhinoApp.WriteLine("baseZ " + this.baseZ);

                pointa = new Point3d(baseX, baseY, 0);
                pointb = new Vector3d(0, 0, 1);

                Plane planebase = new Plane(pointa, pointb);

                Interval xInterval = new Interval(0, this.ObjWidth);
                Interval yInterval = new Interval(0, this.ObjLength);
                Interval zInterval = new Interval(0, this.ObjHeight);

                ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

                xInterval = new Interval(0, this.MrjWidth);
                yInterval = new Interval(0, this.MrjLength);
                zInterval = new Interval(0, this.MrjHeight);

                marginBox = new Box(planebase, xInterval, yInterval, zInterval);

            }

            else
            {
                ObjBox = new Box();
                marginBox = new Box();
            }
        }

        public void PlaceRefObjectEntrance(int[,,] emptyFullArray, int[,,] areaArray, out Box ObjBox, out Box marginBox, out bool placementFailed)
        {
            bool inLimits = true;
            bool suitableDirection = true;

            this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
            this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
            this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

            this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
            this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
            this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

            int XLoopStartMargin = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));
            int XLoopEndMargin = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));

            int YLoopStartMargin = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);
            int YLoopEnMargin = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);

            int ZLoopStartMargin = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);
            int ZLoopEndMargin = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);

            /***/
            int XLoopStartObject = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));
            int XLoopEndObject = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));

            int YLoopStartObject = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);
            int YLoopEnObject = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);

            int ZLoopStartObject = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);
            int ZLoopEndObject = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);


            bool emptyBool = true;
            bool sameZone = true;

            bool placeSuccessful = false;
            int trycount = 0;
            bool enoughTries = false;

            while (placeSuccessful == false && enoughTries == false)
            {
                var rand = new Random();
                var randomCellIndex = rand.Next(this.SpaceList.Count / 2);

                baseX = (this.SpaceList[2 * randomCellIndex]) * CellSize;
                baseY = 0;
                baseZ = 0;

                BaseVector = new Vector3d(baseX, baseY, baseZ);

                bool inLimit = false;

                if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
              (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
                {
                    inLimit = true;
                }


                if (inLimit)
                {
                    for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                    {
                        for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                        {
                            for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                            {
                                //Rhino.RhinoApp.WriteLine("Entrance Reference Object :  " + "  i:  " + i + "  j:  " + j + "  k:  " + k);
                                if (emptyFullArray[i, j, k] == 1)
                                {
                                    emptyBool = false;
                                }

                                if (areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0)
                                {
                                    sameZone = false;
                                }
                            }
                        }
                    }

                    for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                    {
                        for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                        {
                            for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                            {
                                if (emptyFullArray[i, j, k] == 1)
                                {
                                    emptyBool = false;
                                }

                                if (areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0)
                                {
                                    sameZone = false;
                                }
                            }
                        }
                    }
                }

                if (emptyBool && sameZone)
                {



                    placeSuccessful = true;
                    Rhino.RhinoApp.WriteLine("Reference Object conditions are successful ");

                    for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                    {
                        for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                        {
                            for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                            {
                                emptyFullArray[i, j, k] = 2;
                            }
                        }
                    }

                    for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                    {
                        for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                        {
                            for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                            {
                                emptyFullArray[i, j, k] = 1;
                            }
                        }
                    }
                }
                else
                {
                    Rhino.RhinoApp.WriteLine("Conditions are not satisfied ");
                }
            }
            trycount++;
            Rhino.RhinoApp.WriteLine("Try count is : " + trycount);
            if (trycount > 60)
                Rhino.RhinoApp.WriteLine("Try count is over 60 placement is unsuccessful: ");
            enoughTries = true;


            if (trycount < 60 && placeSuccessful)
            {
                placementFailed = false;
                Rhino.RhinoApp.WriteLine("baseX " + this.baseX);
                Rhino.RhinoApp.WriteLine("baseY " + this.baseY);
                Rhino.RhinoApp.WriteLine("baseZ " + this.baseZ);

                Point3d pointa = new Point3d(baseX, 0, 0);
                Vector3d pointb = new Vector3d(0, 0, 1);

                Plane planebase = new Plane(pointa, pointb);

                Interval xInterval = new Interval(0, this.ObjWidth);
                Interval yInterval = new Interval(0, this.ObjLength);
                Interval zInterval = new Interval(0, this.ObjHeight);

                ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

                xInterval = new Interval(0, this.MrjWidth);
                yInterval = new Interval(0, this.MrjLength);
                zInterval = new Interval(0, this.MrjHeight);

                marginBox = new Box(planebase, xInterval, yInterval, zInterval);

            }
            else
            {
                ObjBox = new Box();
                marginBox = new Box();
                placementFailed = true;
            }
        }
    }

    public void PlaceObject(int[,,] emptyFullArray, int[,,] areaArray, out Box ObjBox, out Box marginBox, out bool placementFailed)
    {
        Point3d pointa = new Point3d(baseX, baseY, baseZ);
        Vector3d pointb = new Vector3d(0, 0, 1);

        Plane planebase = new Plane(pointa, pointb);

        Interval xInterval = new Interval(0, ObjWidth);
        Interval yInterval = new Interval(0, this.ObjLength);
        Interval zInterval = new Interval(0, this.ObjHeight);

        ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

        xInterval = new Interval(0, this.MrjWidth);
        yInterval = new Interval(0, this.MrjLength);
        zInterval = new Interval(0, this.MrjHeight);

        marginBox = new Box(planebase, xInterval, yInterval, zInterval);

        List<int> growDirection = new List<int>();

        if (PreviousObject.Front == true)
            growDirection.Add(1); // Front Direction
        if (PreviousObject.Right == true)
            growDirection.Add(2); // Right Direction
        if (PreviousObject.Back == true)
            growDirection.Add(3); // Back Direction
        if (PreviousObject.Left == true)
            growDirection.Add(4); // Left Direction
        if (PreviousObject.Bottom == true)
            growDirection.Add(5); // Bottom Direction
        if (PreviousObject.Top == true)
            growDirection.Add(6); // Right Direction


        if (growDirection.Count != 0)
        {
            for (int i = 0; i < growDirection.Count; i++)
            {
                Rhino.RhinoApp.WriteLine("In grow  " + growDirection[i]);
            }
        }


        bool placeSuccess = false;
        int counter = 0;

        Random randm = new Random();
        Rhino.RhinoApp.WriteLine("Element Number of the grow Direction before while loop " + growDirection.Count.ToString());

        while (growDirection.Count > 0 && placeSuccess == false && counter < 6)
        {
            Rhino.RhinoApp.WriteLine("Counter number in the checkFront Loop " + counter.ToString());

            int a = randm.Next(0, growDirection.Count); // BAK
            Rhino.RhinoApp.WriteLine("A VALUE ---------------------------------------------------" + a);

            int randomMethod = growDirection[a];

            Rhino.RhinoApp.WriteLine("Element Number of the grow Direction in while loop " + growDirection.Count.ToString());

            switch (randomMethod)
            {
                case 1:
                    if (CheckFront(emptyFullArray, areaArray))
                    {
                        this.PlaceFront(emptyFullArray, out ObjBox, out marginBox);
                        placeSuccess = true;

                    }
                    else
                    {
                        growDirection.Remove(a);
                        Rhino.RhinoApp.WriteLine("Could not Placed to Front " + growDirection.Count.ToString());
                        Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());

                    }
                    counter++;
                    break;

                case 2:
                    if (CheckRight(emptyFullArray, areaArray))
                    {
                        this.PlaceRight(emptyFullArray, out ObjBox, out marginBox);
                        placeSuccess = true;
                    }
                    else
                    {
                        growDirection.Remove(a);
                        Rhino.RhinoApp.WriteLine("Could not Placed to right " + growDirection.Count.ToString());
                        Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
                    }
                    counter++;
                    break;

                case 3:
                    if (CheckBack(emptyFullArray, areaArray))
                    {
                        this.PlaceBack(emptyFullArray, out ObjBox, out marginBox);
                        placeSuccess = true;

                    }
                    else
                    {
                        growDirection.Remove(a);
                        Rhino.RhinoApp.WriteLine("Could not Placed to left " + growDirection.Count.ToString());
                        Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());

                    }
                    counter++;
                    break;

                case 4:
                    if (CheckLeft(emptyFullArray, areaArray))
                    {
                        this.PlaceLeft(emptyFullArray, out ObjBox, out marginBox);
                        placeSuccess = true;
                    }
                    else
                    {
                        growDirection.Remove(a);
                        Rhino.RhinoApp.WriteLine("Could not Placed to LEFT " + growDirection.Count.ToString());
                        Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
                    }
                    counter++;
                    break;

                case 5:
                    if (CheckTop(emptyFullArray, areaArray))
                    {
                        this.PlaceTop(emptyFullArray, out ObjBox, out marginBox);
                        placeSuccess = true;
                    }
                    else
                    {
                        growDirection.Remove(a);
                        Rhino.RhinoApp.WriteLine("Could not Placed to Top " + growDirection.Count.ToString());
                        Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
                    }
                    counter++;
                    break;

                case 6:
                    if (CheckBottom(emptyFullArray, areaArray))
                    {
                        this.PlaceBottom(emptyFullArray, out ObjBox, out marginBox);
                        placeSuccess = true;

                    }
                    else
                    {
                        growDirection.Remove(a);
                        Rhino.RhinoApp.WriteLine("Could not Placed to Bottom " + growDirection.Count.ToString());
                        Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
                    }
                    counter++;
                    break;

                default:
                    counter++;
                    break;
            }
        }

        if (placeSuccess == false)
        {
            Rhino.RhinoApp.WriteLine("Placement unsuccessfull ");
            placementFailed = !placeSuccess;
        }
        else
            placementFailed = !placeSuccess;
    }

    public bool CheckFront(int[,,] emptyFullArray, int[,,] areaArray)
    {
        bool inLimits = true;
        bool emptyBool = true;
        bool sameZone = true;

        bool suitableDirection = true;

        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

        if (VectorMargin.X * VectorMargin.Y > 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin;
            this.baseZ = PreviousObject.baseZ;
        }

        else if (VectorMargin.X * VectorMargin.Y < 0)
        {
            this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        Point3d pointa = new Point3d(0, 0, 0);
        Vector3d pointb = new Vector3d(0, 0, 1);
        Plane planee = new Plane(pointa, pointb);

        Vector3d vecX1 = new Vector3d(1, 0, 0);

        BaseVector = new Vector3d(baseX, baseY, baseZ);


        if (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y > 0 ||
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0 ||
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X < 0 ||
            PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0
            )
        {
            suitableDirection = true;
        }

        if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
          (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
        {
            inLimits = false;
        }

        if (inLimits && suitableDirection)
        {
            for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
            {
                for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                {
                    for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                    {
                        Rhino.RhinoApp.WriteLine("i:  " + i + "  j:  " + j + "  k:  " + k);

                        if (emptyFullArray[i, j, k] == 1)
                        {
                            emptyBool = false;
                        }

                        if (areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != this.ZoneName)
                        {
                            sameZone = false;
                        }
                    }
                }
            }
            if (emptyBool && sameZone)
            {
                suitableDirection = true;
                Rhino.RhinoApp.WriteLine("Place Front:  Conditions are  satisfied ");
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Place Front:  Conditions are not satisfied ");
                suitableDirection = false;
            }
        }
        else
        {
            Rhino.RhinoApp.WriteLine("Place Front:  Conditions are not satisfied ");
            suitableDirection = false;
        }
        return suitableDirection;
    }

    public void PlaceFront(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
    {

        for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
        {
            for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
            {
                for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                {
                    emptyFullArray[i, j, k] = 1;
                }
            }
        }

        Point3d pointa = new Point3d(baseX, baseY, baseZ);
        Vector3d pointb = new Vector3d(0, 0, 1);

        Plane planebase = new Plane(pointa, pointb);

        Interval xInterval = new Interval(0, this.ObjWidth);
        Interval yInterval = new Interval(0, this.ObjLength);
        Interval zInterval = new Interval(0, this.ObjHeight);

        ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

        xInterval = new Interval(0, this.MrjWidth);
        yInterval = new Interval(0, this.MrjLength);
        zInterval = new Interval(0, this.MrjHeight);

        marginBox = new Box(planebase, xInterval, yInterval, zInterval);
    }

    public bool CheckRight(int[,,] emptyFullArray, int[,,] areaArray)
    {
        bool inLimits = true;
        bool emptyBool = true;
        bool sameZone = true;
        bool suitableDirection = true;

        /**/
        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

        if (VectorMargin.X * VectorMargin.Y > 0)
        {
            this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        else if (VectorMargin.X * VectorMargin.Y < 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin;
            this.baseZ = PreviousObject.baseZ;
        }

        Point3d pointa = new Point3d(0, 0, 0);
        Vector3d pointb = new Vector3d(0, 0, 1);
        Plane planee = new Plane(pointa, pointb);

        Vector3d vecX1 = new Vector3d(1, 0, 0);

        BaseVector = new Vector3d(baseX, baseY, baseZ);

        /**/

        if (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X > 0 ||
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0 ||
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y > 0 ||
            PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0
            )
        {
            suitableDirection = true;
        }

        if (!((baseX + this.ObjWidth < 360) && (baseY + this.ObjLength < 600) && (baseZ + this.ObjHeight < 300) &&
          (baseX + this.ObjWidth > 0) && (baseY + this.ObjLength > 0) && (baseZ + this.ObjHeight > 0)))
        {
            inLimits = false;
        }

        if (inLimits && suitableDirection)
        {
            for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
            {
                for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                {
                    for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
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
                suitableDirection = true;
                Rhino.RhinoApp.WriteLine("Place Right:  Conditions are  satisfied ");
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Place Right:  Conditions are not satisfied ");
                suitableDirection = false;
            }
        }
        else
        {
            Rhino.RhinoApp.WriteLine("Place Right:  Conditions are not satisfied ");
            suitableDirection = false;
        }
        return suitableDirection;
    }

    public void PlaceRight(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
    {
        for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
        {
            for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
            {
                for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                {
                    emptyFullArray[i, j, k] = 1;
                }
            }
        }

        Point3d pointa = new Point3d(baseX, baseY, baseZ);
        Vector3d pointb = new Vector3d(0, 0, 1);

        Plane planebase = new Plane(pointa, pointb);

        Interval xInterval = new Interval(0, this.ObjWidth);
        Interval yInterval = new Interval(0, this.ObjLength);
        Interval zInterval = new Interval(0, this.ObjHeight);

        ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

        xInterval = new Interval(0, this.MrjWidth);
        yInterval = new Interval(0, this.MrjLength);
        zInterval = new Interval(0, this.MrjHeight);

        marginBox = new Box(planebase, xInterval, yInterval, zInterval);
    }

    public bool CheckBack(int[,,] emptyFullArray, int[,,] areaArray)

    {
        bool inLimits = true;
        bool emptyBool = true;
        bool sameZone = true;

        bool suitableDirection = true;

        /**/
        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

        if (VectorMargin.X * VectorMargin.Y > 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        else if (VectorMargin.X * VectorMargin.Y < 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        Point3d pointa = new Point3d(0, 0, 0);
        Vector3d pointb = new Vector3d(0, 0, 1);
        Plane planee = new Plane(pointa, pointb);

        Vector3d vecX1 = new Vector3d(1, 0, 0);

        BaseVector = new Vector3d(baseX, baseY, baseZ);

        if (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y < 0 || //++
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0 || //--
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X > 0 || //-+
            PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0    //+-
            )
        {
            suitableDirection = true;
        }

        if (!((baseX + this.VectorMargin.X < 300) && (baseY + this.VectorMargin.Y < 600) && (baseZ + this.VectorObject.Z < 300) &&
          (baseX + this.VectorMargin.X > 0) && (baseY + this.VectorMargin.Y > 0) && (baseZ + this.VectorObject.Z > 0)))
        {
            inLimits = false;
        }
        if (inLimits && suitableDirection)
        {
            for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
            {
                for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                {
                    for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
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
                suitableDirection = true;
                Rhino.RhinoApp.WriteLine("Place Back:  Conditions are  satisfied ");
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Place Back:  Conditions are not satisfied ");
                suitableDirection = false;
            }
        }
        else
        {
            Rhino.RhinoApp.WriteLine("Place Back:  Conditions are not satisfied ");
            suitableDirection = false;
        }
        return suitableDirection;
    }

    public void PlaceBack(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
    {
        Rhino.RhinoApp.WriteLine("Place Back: Conditions are successful ");
        for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
        {
            for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
            {
                for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                {
                    emptyFullArray[i, j, k] = 1;
                }
            }
        }

        Point3d pointa = new Point3d(baseX, baseY, baseZ);
        Vector3d pointb = new Vector3d(0, 0, 1);

        Plane planebase = new Plane(pointa, pointb);

        Interval xInterval = new Interval(0, this.ObjWidth);
        Interval yInterval = new Interval(0, this.ObjLength);
        Interval zInterval = new Interval(0, this.ObjHeight);

        ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

        xInterval = new Interval(0, this.MrjWidth);
        yInterval = new Interval(0, this.MrjLength);
        zInterval = new Interval(0, this.MrjHeight);

        marginBox = new Box(planebase, xInterval, yInterval, zInterval);
    }

    public bool CheckLeft(int[,,] emptyFullArray, int[,,] areaArray)
    {


        Rhino.RhinoApp.WriteLine("Left is Checked !******************************* ");
        bool inLimits = true;
        bool emptyBool = true;
        bool sameZone = true;
        bool suitableDirection = true;

        /**/
        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

        if (VectorMargin.X * VectorMargin.Y > 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        else if (VectorMargin.X * VectorMargin.Y < 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        Point3d pointa = new Point3d(0, 0, 0);
        Vector3d pointb = new Vector3d(0, 0, 1);
        Plane planee = new Plane(pointa, pointb);

        Vector3d vecX1 = new Vector3d(1, 0, 0);

        BaseVector = new Vector3d(baseX, baseY, baseZ);

        if (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X < 0 ||
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0 ||
            PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y < 0 ||
            PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0
            )
        {
            suitableDirection = true;
        }


        if (!((baseX + this.VectorMargin.X < 360) && (baseY + this.VectorMargin.Y < 600) && (baseZ + this.VectorMargin.Z < 300) &&
          (baseX + this.VectorMargin.X > 0) && (baseY + this.VectorMargin.Y > 0) && (baseZ + this.VectorMargin.Z > 0)))
        {
            Rhino.RhinoApp.WriteLine("CAN NOT PLACE TO LEFT NOT IN LIMITS!******************************* ");
            inLimits = false;
        }

        if (inLimits && suitableDirection)
        {
            for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
            {
                for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                {
                    for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                    {
                        Rhino.RhinoApp.WriteLine("i:  " + i + "  j:  " + j + "  k:  " + k);

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
                suitableDirection = true;
                Rhino.RhinoApp.WriteLine("Place Left:  Conditions are  satisfied ");
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Place Left:  Conditions are not satisfied ");
                suitableDirection = false;
            }
        }
        else
        {
            Rhino.RhinoApp.WriteLine("Place Left:  Conditions are not satisfied ");
            suitableDirection = false;
        }
        return suitableDirection;
    }

    public void PlaceLeft(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
    {
        for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
        {
            for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
            {
                for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                {
                    emptyFullArray[i, j, k] = 1;
                }
            }
        }

        Point3d pointa = new Point3d(baseX, baseY, baseZ);
        Vector3d pointb = new Vector3d(0, 0, 1);

        Plane planebase = new Plane(pointa, pointb);

        Interval xInterval = new Interval(0, this.ObjWidth);
        Interval yInterval = new Interval(0, this.ObjLength);
        Interval zInterval = new Interval(0, this.ObjHeight);

        ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

        xInterval = new Interval(0, this.MrjWidth);
        yInterval = new Interval(0, this.MrjLength);
        zInterval = new Interval(0, this.MrjHeight);

        marginBox = new Box(planebase, xInterval, yInterval, zInterval);
    }

    public bool CheckTop(int[,,] emptyFullArray, int[,,] areaArray)
    {
        bool inLimits = true;
        bool emptyBool = true;
        bool sameZone = true;

        bool suitableDirection = true;

        /**/
        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

        if (VectorMargin.X * VectorMargin.Y > 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ + PreviousObject.ZCellMargin; ;
        }

        else if (VectorMargin.X * VectorMargin.Y < 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ + PreviousObject.ZCellMargin;
        }

        Point3d pointa = new Point3d(0, 0, 0);
        Vector3d pointb = new Vector3d(0, 0, 1);
        Plane planee = new Plane(pointa, pointb);

        Vector3d vecX1 = new Vector3d(1, 0, 0);

        BaseVector = new Vector3d(baseX, baseY, baseZ);

        if (VectorObject.Z > 0) //++
        {
            suitableDirection = true;
        }

        if (!((baseX + this.VectorMargin.X < 360) && (baseY + this.VectorMargin.Y < 600) && (baseZ + this.VectorMargin.Z < 300) &&
          (baseX + this.VectorMargin.X > 0) && (baseY + this.VectorMargin.Y > 0) && (baseZ + this.VectorMargin.Z > 0)))
        {
            inLimits = false;
        }

        if (inLimits && suitableDirection)
        {
            for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
            {
                for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                {
                    for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                    {
                        Rhino.RhinoApp.WriteLine("i:  " + i + "  j:  " + j + "  k:  " + k);

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
                suitableDirection = true;
                Rhino.RhinoApp.WriteLine("Place Top:  Conditions are  satisfied ");
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Place Top:  Conditions are not satisfied ");
                suitableDirection = false;
            }
        }
        else
        {
            Rhino.RhinoApp.WriteLine("Place Top:  Conditions are not satisfied ");
            suitableDirection = false;
        }
        return suitableDirection;
    }

    public void PlaceTop(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
    {

        for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
        {
            for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
            {
                for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                {
                    emptyFullArray[i, j, k] = 1;
                }
            }
        }

        Point3d pointa = new Point3d(baseX, baseY, baseZ);
        Vector3d pointb = new Vector3d(0, 0, 1);

        Plane planebase = new Plane(pointa, pointb);

        Interval xInterval = new Interval(0, this.ObjWidth);
        Interval yInterval = new Interval(0, this.ObjLength);
        Interval zInterval = new Interval(0, this.ObjHeight);

        ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

        xInterval = new Interval(0, this.MrjWidth);
        yInterval = new Interval(0, this.MrjLength);
        zInterval = new Interval(0, this.MrjHeight);

        marginBox = new Box(planebase, xInterval, yInterval, zInterval);
    }

    public bool CheckBottom(int[,,] emptyFullArray, int[,,] areaArray)
    {
        bool inLimits = true;
        bool emptyBool = true;
        bool sameZone = true;

        bool suitableDirection = true;

        /**/
        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

        if (VectorMargin.X * VectorMargin.Y > 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        else if (VectorMargin.X * VectorMargin.Y < 0)
        {
            this.baseX = PreviousObject.baseX;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        Point3d pointa = new Point3d(0, 0, 0);
        Vector3d pointb = new Vector3d(0, 0, 1);
        Plane planee = new Plane(pointa, pointb);

        Vector3d vecX1 = new Vector3d(1, 0, 0);

        BaseVector = new Vector3d(baseX, baseY, baseZ);


        if (VectorObject.Z < 0) //++
        {
            suitableDirection = true;
        }

        if (!((baseX + this.ObjWidth < 360) && (baseY + this.ObjLength < 600) && (baseZ + this.ObjHeight < 300) &&
          (baseX + this.ObjWidth > 0) && (baseY + this.ObjLength > 0) && (baseZ + this.ObjHeight > 0)))
        {
            inLimits = false;
        }

        if (inLimits && suitableDirection)
        {
            for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
            {
                for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
                {
                    for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                    {
                        Rhino.RhinoApp.WriteLine("i:  " + i + "  j:  " + j + "  k:  " + k);

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
                suitableDirection = true;
                Rhino.RhinoApp.WriteLine("Place Bottom:  Conditions are  satisfied ");
            }
            else
            {
                Rhino.RhinoApp.WriteLine("Place Bottom:  Conditions are not satisfied ");
                suitableDirection = false;
            }
        }
        else
        {
            Rhino.RhinoApp.WriteLine("Place Bottom:  Conditions are not satisfied ");
            suitableDirection = false;
        }
        return suitableDirection;
    }

    public void PlaceBottom(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
    {
        for (int i = (int)(baseX / CellSize); i < (baseX / CellSize + (int)XCellMargin); i++)
        {
            for (int j = (int)(baseY / CellSize); j < baseY / CellSize + (int)YCellMargin; j++)
            {
                for (int k = (int)(baseZ / 150); k < baseZ / 150 + (int)ZCellMargin; k++)
                {
                    emptyFullArray[i, j, k] = 1;
                }
            }
        }

        Point3d pointa = new Point3d(baseX, baseY, baseZ);
        Vector3d pointb = new Vector3d(0, 0, 1);

        Plane planebase = new Plane(pointa, pointb);

        Interval xInterval = new Interval(0, this.ObjWidth);
        Interval yInterval = new Interval(0, this.ObjLength);
        Interval zInterval = new Interval(0, this.ObjHeight);

        ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

        xInterval = new Interval(0, this.MrjWidth);
        yInterval = new Interval(0, this.MrjLength);
        zInterval = new Interval(0, this.MrjHeight);

        marginBox = new Box(planebase, xInterval, yInterval, zInterval);
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

            }
        }
    }
}
