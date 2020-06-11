using System;
using System.Collections.Generic;
using Rhino.Geometry;

public partial class Script_Instance
{
    // <Custom additional code> 

       
    class Objects
    {
        // we are declaring the public variables of our child objects.
        // IF YOU ARE COMFORTABLE WITH OOP PLEASE ADD PRIVIACY OF THE PROPERTIES AND FUNCTIONS. Private, readonly, get, set etc.
        // 

            // ZONE AS STRING AND INTEGER TO CHECK THE ARRAY VALUES IT MIGHT BE ENUM, OR TUPLE AS WELL.
        public string Name;
        public int ZoneName;

        // ASSGING THE CONNECTED BOX AND MARGIN BOXES.
        public Box Obj;
        public Box ObjMargin;

        // POSSIBLE ROTATIONS BOOLS
        public bool Front = false;
        public bool Right = false;
        public bool Back = false;
        public bool Left = false;
        public bool Top = false;
        public bool Bottom = false;

        // ROTATION  OPTIONS INITIALIZED AS EMPTY LIST.
        // BOOL CREATED BUT IS NOT ACTIVE RIGHT NOW.
        // WE KEEP THE ROTATION INFORMATION TO FOLLOW DIRECTIONS AND DEBUG THEM.
        public bool RotationBool = true;
        public List<double> RotationOpt = new List<double>(); // rotation options Define angles as double list; 0 is default
        public double AppliedRotation = 0;

        // MIRROR BOOL IS DEFINED AND MIRROR OPTION IS CREATED AS LIST OF INTEGERS
        // WE KEEP APPLIED MIRROR VALUE FOLLOW DIRECTIONS AND DEBUG THEM..
        public bool MirrorBool = true;
        public List<int> MirrorOpt = new List<int>(); // mirror options 0=None! 1=z, 2= X, 3 = Y;
        public int AppliedMirror = 0;

        public string Source; // Water, Electiricity
        public int FixedToWall; // Bed, Television //1,2,3 conditions
        public int CellSize;

        // WE NEED INFORMATIONS FROM THE PREVIOUS OBJECTS DIRECTION ROTATION DIMENSIONS ETC TO PLACE NEW OBJECT
        // SO WE NEED TO BE ABLE REACH THE PREV OBJ.
        public Objects PreviousObject;

        // DECLARE DIMENSION, AND BASE VARIABLES OF BOUNDRY BOXES.
        public double ObjLength;
        public double ObjWidth;
        public double ObjHeight;

        public double MrjLength;
        public double MrjWidth;
        public double MrjHeight;

        public double baseX, baseY, baseZ;
        public Vector3d BaseVector;

        public List<int> SpaceList = new List<int>();


        // CELL VALUES OF THE GIVEN OBJECTS AND MARGINS
        public double XCellMargin;
        public double YCellMargin;
        public double ZCellMargin;

        public double XCellObject;
        public double YCellObject;
        public double ZCellObject;

        // TO KEEP DIRECTION,ROTATE,MIRROR OPTIONS AND ALL THE DIMENSIONS PROPERLY WE NEED TO USE VECTORS
        // IT STARTS FROM BASE POINT GOES TO FAREST CORNER.

        public Vector3d VectorMargin;
        public Vector3d VectorObject;

        // KEEP THE INITIAL VECTORS, USEFUL FOR ROTATE AND MIRROR FUNCTIONS AND PLACEMENT TRIES.
        public Vector3d VectorMarginCache;
        public Vector3d VectorObjectCache;

        Plane PlaneBase;

        public Objects()
        {
            // EMPTY CONSTRUCTOR METHOD.
            // PLEASE CHECK CONSTRUCTOR METHOD DOCUMENTS TO MODIFY THEM.
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/constructors
            // THESE METHODS ARE USED FOR INITIALIZING THE OBJECTS OF THE CLASS IN MAIN FUNCTIONS.
            // WE CAN DEFINE MULTIPLE CONSTRUCTOR METHODS.
            // https://www.w3schools.com/cs/cs_method_overloading.asp
        }

        public Objects(string name, int zoneName, bool front, bool right, bool back, bool left, bool top, bool bottom, bool rotationalBool, List<double> rotationOpt, bool mirrorBool, List<int> mirrorOpt, string source, int fixedTowall, int cellSize, Box obj, Box objMargin, List<int> spaceList)
          : this()// THANK TO --- :this()  WE CAN COLLECT ASSIGNMENTS IN THE CONSTRUCTOR METHOD WITH MENTIONED ARGUMENTS 
        {
            // be carefull about naming convention, arguments are  starts with lowercase.
            // Objects properties starts UPPERCASE and they have .this to mention that the properties of thisobject will be getting the assigned values.

            // to understand these values functions please check the objects class beginning where these variables are declared.
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
            // this constructor DOES NOT has previousObjects so it can be used for  reference objects
        }

        public Objects(string name, int zoneName, bool front, bool right, bool back, bool left, bool top, bool bottom, bool rotationalBool, List<double> rotationOpt, bool mirrorBool, List<int> mirrorOpt, string source, int fixedTowall, int cellSize, Box obj, Box objMargin, Objects previousObject, List<int> spaceList)
          : this(name, zoneName, front, right, back, left, top, bottom, rotationalBool, rotationOpt, mirrorBool, mirrorOpt, source, fixedTowall, cellSize, obj, objMargin, spaceList)
        {
            // this constructor method has previousObject so it can be used next objects afteer reference objects.
            this.PreviousObject = previousObject;
        }

        public void AssignDimensions()
        {
            // we need to run this method after constructor to assign dimensions and vectors.
            // Get Margin and objects side lenghts - T1 represents end of the interval. T0 = 0.
            this.ObjWidth = this.Obj.X.T1;
            this.ObjLength = this.Obj.Y.T1;
            this.ObjHeight = this.Obj.Z.T1;

            this.MrjWidth = this.ObjMargin.X.T1;
            this.MrjLength = this.ObjMargin.Y.T1;
            this.MrjHeight = this.ObjMargin.Z.T1;

            // use dimensions to create a vector3d
            this.VectorObject = new Vector3d(this.ObjWidth, this.ObjLength, this.ObjHeight);
            this.VectorMargin = new Vector3d(this.MrjWidth, this.MrjLength, this.MrjHeight);

            // keep the initial vectors seperately.  
            // MAKE THEM READ-ONLY. THEY SHOULD NOT BE MODIFIED.
            this.VectorMarginCache = this.VectorMargin;
            this.VectorObjectCache = this.VectorObject;

        }

        public void PrintDimensions()
        {
            // TO SEE THE DIMS WITHOUT ANY TRANSLATIONS  OVER THE RHINO CONSOLE USE IT.
            Rhino.RhinoApp.WriteLine("Obj Width " + this.ObjWidth.ToString());
            Rhino.RhinoApp.WriteLine("Obj Length " + this.ObjLength.ToString());
            Rhino.RhinoApp.WriteLine("Obj Height " + this.ObjHeight.ToString());

            Rhino.RhinoApp.WriteLine("Obj Width " + this.MrjWidth.ToString());
            Rhino.RhinoApp.WriteLine("Obj Length " + this.MrjLength.ToString());
            Rhino.RhinoApp.WriteLine("Obj Height " + this.MrjHeight.ToString());
        }

        public void Rotate(double Angle)
        {
            // rotate the objects around Z+direction
            // input is double change it to radians.
            Angle = Angle * (Math.PI / 180);
            
            VectorMargin.Rotate(Angle, new Vector3d(0, 0, 1));
            VectorObject.Rotate(Angle, new Vector3d(0, 0, 1));
        }

        public void Mirroring(int axis)
        {
            // get the symmetric of th objects according the symmetry lines.
            if (axis != 0)
            {
                Point3d pointa = new Point3d(this.baseX, this.baseY, this.baseZ);
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
        }

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

                if ((baseX + this.VectorMargin.X < 360) && (baseY + this.VectorMargin.Y < 660) && (baseZ + this.VectorMargin.Z < 300) &&
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
            // find the cell numbers that objects fills in the Area. 
            // Math.Ceiling ==> 1.1 ==> 2.0 
            // because of preveting from errors like -0.9 ==> 0.0 (we want -1.0) we are using absolute values and multiply it with -1 if vector value is actually negative
            this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
            this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
            this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

            //Rhino.RhinoApp.WriteLine(" MARGIN  numbers ----------------------------------" + XCellMargin.ToString() + ' ' + YCellMargin.ToString() + ' ' + ZCellMargin.ToString() + ' ');

            this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
            this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
            this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

            //Rhino.RhinoApp.WriteLine(" OBJECT  numbers ----------------------------------" + XCellObject.ToString() + ' ' + YCellObject.ToString() + ' ' + ZCellObject.ToString() + ' ');

            // We need to define the start end end points because we dont know if the inital poin is lower than end point

            bool emptyBool = true;
            bool sameZone = true;

            // while loop control variables.
            bool placeSuccessful = false; // stop if placement is successfull
            int trycount = 0; 
            bool enoughTries = false; // stop after enough tries.

            while (placeSuccessful == false && enoughTries == false)
            {
                var rand = new Random();
                var randomCellIndex = rand.Next(this.SpaceList.Count / 2);

                // to place exteriorDoor we a random point over X. y=z=0
                baseX = (this.SpaceList[2 * randomCellIndex]) * CellSize;
                baseY = 0;
                baseZ = 0;

                BaseVector = new Vector3d(baseX, baseY, baseZ);

                /**
                  we are gonna use these start and end values in for loops but the problem here is we don't know if the new point after a vector is added is 
                  lower than the base point or not. So we need o compare each axis'start end end conditions and find the min and max values.
                  like that for loops can work without any problem.
                 */
                 //FUTURE WORK : UPATE 150 constant with a level height!  it works but if we divide to differnt levels it will crash! 
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

                // can open the comments for debugging.
                /*
                        Rhino.RhinoApp.WriteLine("X    Loop numbers ----------------------------------" + XLoopStartMargin.ToString() + ' ' + XLoopEndMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine("Y    Loop numbers ----------------------------------" + YLoopStartMargin.ToString() + ' ' + YLoopEnMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine("Z    Loop numbers ----------------------------------" + ZLoopStartMargin.ToString() + ' ' + ZLoopEndMargin.ToString() + ' ');
                */
                bool inLimit = true;

                // Before entering the for loops check if the objects boundries are in given area zone!
                // we are doing it to avoid for crashing AKA -- Out of Bound Exception --

                if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 660) && (baseZ + VectorMargin.Z < 300) &&
                  (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
                {
                    inLimit = false;
                }

                if (inLimit) // if the conditions are okay! Dive into loops
                {
                    for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                    {
                        for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                        {
                            for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                            {
                                //checks if the cell is empty (We dont want to see any object in a cell! )
                                if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                {
                                    //Rhino.RhinoApp.WriteLine(" Loop numbers ----------------------------------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                    emptyBool = false;
                                }

                                // 2nd Loop checks if the zone is the same or empty zone // Actually margin can ovewflow to other zones
                                /*
                                if (!(areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0))
                                {
                                sameZone = false;
                                }
                                */
                            }
                        }
                    }

                    // 2nd Loop checks if the zone is the same or empty zone
                    for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                    {
                        for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                        {
                            for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                            {
                                //checks if the cell is empty (We dont want to see margin or object in the cell )
                                if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                {
                                    emptyBool = false;
                                }

                                // 2nd Loop checks if the zone is the same or empty zone
                                if (!(areaArray[i, j, k] == this.ZoneName || areaArray[i, j, k] == 0)) // if the zone is not empty or same zone ==> False
                                {
                                    sameZone = false;
                                }
                            }
                        }
                    }

                    if (emptyBool && sameZone) // if the conditions above are true // Place object and margin!
                    {
                        placeSuccessful = true;
                        Rhino.RhinoApp.WriteLine("Reference Object conditions are successful ");
                        //Place Margin
                        for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                        {
                            for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                            {
                                for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                                {
                                    //Rhino.RhinoApp.WriteLine(" Loop numbers ----------------------------------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                    emptyFullArray[i, j, k] = 2; // 2 represents margin
                                }
                            }
                        }
                        //Place Object, object is smaller than margin
                        for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                        {
                            for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                            {
                                for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                                {
                                    //Rhino.RhinoApp.WriteLine("Loop inetegers --------------- " + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                    emptyFullArray[i, j, k] = 1; // 1 represents object
                                }
                            }
                        }
                    }
                    else
                    {
                        Rhino.RhinoApp.WriteLine("EntranceZone Placement Conditions are not satisfied ");
                        trycount++;
                        goto skipEntrance; //sometimes program were stuck here so instead of expecting it to continue it calculations force program to go out of the loop!
                                           // it automatically goes where the goto element is.

                        //PLEASE CHECK JUMP STATEMENTS FOR  goto, break, continues etc.
                        //https://www.geeksforgeeks.org/c-sharp-jump-statements-break-continue-goto-return-and-throw/

                    }
                }
            }
            skipEntrance:
            trycount++;

            Rhino.RhinoApp.WriteLine("Try count is : " + trycount);

            if (trycount > 60) // check the try number if it is over given number go out od the loop! to do that return a bool value.
            {
                Rhino.RhinoApp.WriteLine("Try count is over 60 placement is unsuccessful: ");
                enoughTries = true;
            }

            if (trycount < 60 && placeSuccessful)
            {
                // if all the conditions  are right place object and margin.
                placementFailed = false;
                Rhino.RhinoApp.WriteLine("baseX " + this.baseX);
                Rhino.RhinoApp.WriteLine("baseY " + this.baseY);
                Rhino.RhinoApp.WriteLine("baseZ " + this.baseZ);


                // create point and verctor for creaating plane
                Point3d pointa = new Point3d(baseX, 0, 0);
                Vector3d pointb = new Vector3d(0, 0, 1);

                // create plane to define base point of BOX
                Plane planebase = new Plane(pointa, pointb);

                // define box sides as intervals.
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
                // even if conditions are false we need to return out parameter!
                // so we  are creating empty box but we are also returning PlacementFailed bool
                // so main function will not continue over this iteration anymore!
                ObjBox = new Box();
                marginBox = new Box();
                placementFailed = true;
            }
        }

        public void PlaceObject(int[,,] emptyFullArray, int[,,] areaArray, out Box ObjBox, out Box marginBox, out bool placementFailed)
        {
            // Create point and vector to create plane

            Point3d pointa = new Point3d(baseX, baseY, baseZ);
            Vector3d pointb = new Vector3d(0, 0, 1);

            // Crate a palne to create a Box
            Plane planebase = new Plane(pointa, pointb);

            // Define intervals by using Vector dimensions
            Interval xInterval = new Interval(0, this.VectorObject.X);
            Interval yInterval = new Interval(0, this.VectorObject.Y);
            Interval zInterval = new Interval(0, this.VectorObject.Z);

            ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

             
            xInterval = new Interval(0, this.VectorMargin.X);
            yInterval = new Interval(0, this.VectorMargin.Y);
            zInterval = new Interval(0, this.VectorMargin.Z);

            marginBox = new Box(planebase, xInterval, yInterval, zInterval);

            List<int> growDirection = new List<int>();

            // Stack all the possible directions in a list!

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

            //Show the possible direction number !
            if (growDirection.Count != 0)
            {
                for (int i = 0; i < growDirection.Count; i++)
                {
                    //Rhino.RhinoApp.WriteLine("In grow  " + growDirection[i]);
                }
            }

            // declare varibles to  check loop conditions. 
            bool placeSuccess = false;
            int counter = 0;

            Random randm = new Random();
            // Rhino.RhinoApp.WriteLine("Element Number of the grow Direction before while loop " + growDirection.Count.ToString());

            while (growDirection.Count > 0 && placeSuccess == false && counter < 6) // if max 6 directions are tried go out of the loop. go out if one of them successsfull.
            {
                //Rhino.RhinoApp.WriteLine("Counter number in the check direction loop " + counter.ToString());

                int a = randm.Next(0, growDirection.Count); // get the index of one of the directions

                int randomDirectionValue = growDirection[a]; // get the value of direction in that index

                // Rhino.RhinoApp.WriteLine("Element Number of the grow Direction in while loop " + growDirection.Count.ToString());

                switch (randomDirectionValue)
                {
                    case 1: // Try to place this.object to front of previousobject!
                        if (CheckFront(emptyFullArray, areaArray)) // Check front controls each possible options to place the object and returns a bool true if the condition is successful
                        {
                            // if conditions are valid place object and margin.
                            this.PlaceFront(emptyFullArray, out ObjBox, out marginBox);
                            placeSuccess = true; // assign it true so loo will be broken
                        }
                        else
                        {
                            growDirection.Remove(randomDirectionValue);
                            Rhino.RhinoApp.WriteLine("Could not be Placed to FRONT ");
                            //Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
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
                            growDirection.Remove(randomDirectionValue);
                            Rhino.RhinoApp.WriteLine("Could not Placed to RIGHT ");
                            //Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
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
                            growDirection.Remove(randomDirectionValue);
                            Rhino.RhinoApp.WriteLine("Could not Placed to BACK ");
                            //Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
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
                            growDirection.Remove(randomDirectionValue);
                            Rhino.RhinoApp.WriteLine("Could not Placed to LEFT ");
                            //Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
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
                            growDirection.Remove(randomDirectionValue);
                            Rhino.RhinoApp.WriteLine("Could not Placed to Top ");
                            //Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
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
                            growDirection.Remove(randomDirectionValue);
                            Rhino.RhinoApp.WriteLine("Could not Placed to BOTTOM ");
                            //Rhino.RhinoApp.WriteLine("Element Number of the grow Direction after removal " + growDirection.Count.ToString());
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
                placementFailed = true;// after all of the tries placement failed. It will result in breaking the main loop.!
            }
            else
                placementFailed = false;
        }

        public bool CheckFront(int[,,] emptyFullArray, int[,,] areaArray)
        {

            /*
             Check Functions have same process in itselfs. 
             It checkes each possible mirror, rotation and closest base points in given directions projecting the previousMargin border.
             
             */
            bool suitableDirection = true;

            // start baseSlider variables. These are the variables sliding over previous objects side and creating multiple base points fot this.Object 
            int baseSliderMin = 0; 
            int baseSliderMax = 0;

            // Shuffle the rotation and mirror functions in order to randomize process.

            Fisher_Yates_CardDeck_Shuffle(MirrorOpt);
            Fisher_Yates_CardDeck_Shuffle(RotationOpt);

            for (int f = 0; f < this.MirrorOpt.Count; f++)
            {
                for (int g = 0; g < this.RotationOpt.Count; g++)
                {
                    if (PreviousObject.AppliedMirror == 0)// Mirror default
                    {
                        // Min value is the min integeer value when we are checking growing direction side. Normally we can set one point but we want to 
                        // slide over from min valuee to max value over edge in the direction.
                        // we can write if statements separetly but ? operator gives us a chance to check condition and return expected values in one line.
                        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator

                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY)));
                    }

                    if (PreviousObject.AppliedMirror == 1) // Mirror Z
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY)));
                    }

                    if (PreviousObject.AppliedMirror == 2) // Mirror X
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY)));
                    }

                    if (PreviousObject.AppliedMirror == 3) // Mirror Y
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY)));
                    }

                    for (int h = baseSliderMin; h <= baseSliderMax; h = h + this.CellSize)
                        // slide over the base points given above.
                    {
                        this.Mirroring(MirrorOpt[f]);      // apply  mirror function
                        this.Rotate((int)RotationOpt[g]); // apply rotate function

                        AppliedRotation = RotationOpt[g]; // save applied rotation
                        AppliedMirror = MirrorOpt[f];    // save applied mirror 

                        bool inLimits = true;
                        bool emptyBool = true;
                        bool sameZone = true;

                        suitableDirection = true;
                        bool initialDirectionCheck = false;

                        // find the cell numbers that objects fills in the Area. 
                        // Math.Ceiling ==> 1.1 ==> 2.0 
                        // because of preveting from errors like -0.9 ==> 0.0 (we want -1.0) we are using absolute values and multiply it with -1 if vector value is actually negative

                        // Get the cell values * check entranceReference method for more.
                        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
                        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
                        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

                        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
                        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
                        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

                        /*
                        Rhino.RhinoApp.WriteLine(" MARGIN  Front numbers -----------------------" + XCellMargin.ToString() + ' ' + YCellMargin.ToString() + ' ' + ZCellMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine(" OBJECT  Front numbers -----------------------" + XCellObject.ToString() + ' ' + YCellObject.ToString() + ' ' + ZCellObject.ToString() + ' ');
                        */

                        // Cactch the base point conditions according to its rotation and mirror conditions! 
                        // h is the sliding values.

                        if (PreviousObject.AppliedMirror == 0) // assign base points, Use sliding variables 
                        {
                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0)
                            {
                                this.baseX = h; // slide over X axis , Y axis should be constant
                                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                                this.baseY = h; // slide over Y axis , X axis should be constant
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 2)
                        {

                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = h; // slide over X axis , Y axis should be constant
                                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.PreviousObject.VectorMargin.X * PreviousObject.PreviousObject.VectorMargin.Y > 0)
                            {
                                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                                this.baseY = h; // slide over X axis , Y axis should be constant
                                this.baseZ = PreviousObject.baseZ;
                            }

                            if (AppliedRotation == 90)
                            {
                                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize; // slide over X axis , Y axis should be constant
                                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 3)
                        {
                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = h; // slide over X axis , Y axis should be constant
                                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (VectorMargin.X * VectorMargin.Y > 0)
                            {
                                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                                this.baseY = h; // slide over Y axis , X axis should be constant
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        Point3d pointa = new Point3d(0, 0, 0);
                        Vector3d pointb = new Vector3d(0, 0, 1);
                        Plane planee = new Plane(pointa, pointb);

                        Vector3d vecX1 = new Vector3d(1, 0, 0);
                        BaseVector = new Vector3d(baseX, baseY, baseZ); // by using the given base points create base vector.

                        /**
                         * We don't want to new object is overflows to other directions so we need to limit the directions. 
                         * these methods checks if everything works fine after placing new object vector to base point..
                         * 
                         */
                        if (PreviousObject.AppliedMirror == 0) // mirror default
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y > 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X < 0) || // if previous object looks left //  X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0)       // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 1) //mirror  Z
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y > 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X < 0) || // if previous object looks left //  X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0)       // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 2) // mirror x
                        {
                            if ((PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X > 0) || 
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0) || 
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y > 0) || 
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0)    
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 3) //mirror  Y
                        {
                            if ((PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X > 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y > 0) || // if previous object looks left  // X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0)      // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }

                        }
                        else
                        {
                            /*
                            if (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y > 0 || // if previous object looks front // Y value should be positive
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0 || // if previous object looks back  // Y value should be negative
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X < 0 || // if previous object looks left //  X value should be positive
                              PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0       // if previous object looks right // X value should be positive
                              )
                            {
                              initialDirectionCheck = true;
                            }
                              */
                        }

                        if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 660) && (baseZ + VectorMargin.Z < 300) &&
                          (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
                        {
                            inLimits = false;
                        }
                        // Rhino.RhinoApp.WriteLine("Bools " + suitableDirection.ToString() + "    " + inLimits.ToString() + ' ');

                        /**
                  we are gonna use these start and end values in for loops but the problem here is we don't know if the new point after a vector is added is 
                  lower than the base point or not. So we need o compare each axis'start end end conditions and find the min and max values.
                  like that for loops can work without any problem.
                 */
                        //FUTURE WORK : UPATE 150 constant with a level height!  it works but if we divide to differnt levels it will crash! 

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

                        /*
                                    Rhino.RhinoApp.WriteLine("X  Front  Margin Loop numbers -------------" + XLoopStartMargin.ToString() + ' ' + XLoopEndMargin.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Y  Front Margin  Loop numbers -------------" + YLoopStartMargin.ToString() + ' ' + YLoopEnMargin.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Z  Front  Margin Loop numbers -------------" + ZLoopStartMargin.ToString() + ' ' + ZLoopEndMargin.ToString() + ' ');

                                    Rhino.RhinoApp.WriteLine("X  Front Object Loop numbers ----------" + XLoopStartObject.ToString() + ' ' + XLoopEndObject.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Y  Front Object  Loop numbers ---------" + YLoopStartObject.ToString() + ' ' + YLoopEnObject.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Z  Front Object  Loop numbers ---------" + ZLoopStartObject.ToString() + ' ' + ZLoopEndObject.ToString() + ' ');
                        */
                        if (inLimits && initialDirectionCheck)
                        {
                            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                            {
                                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                                {
                                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see an object in a cell! )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            //Rhino.RhinoApp.WriteLine("Front Margin Array is full, Loop numbers ------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }

                                        // 2nd Loop checks if the zone is the same or empty zone // Actually margin can ovewflow to other zones
                                        /*
                                        if (!(areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0))
                                        {
                                        sameZone = false;
                                        }
                                        */
                                    }
                                }
                            }
                            // 2nd Loop checks if the zone is the same or empty zone
                            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                            {
                                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                                {
                                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see margin or object in the cell )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            //Rhino.RhinoApp.WriteLine("Front object Array is full: Loop numbers ---------------------------------- i:" + i.ToString() + 'j' + j.ToString() + 'k' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }

                                        // 2nd Loop checks if the zone is the same or empty zone
                                        if (!(areaArray[i, j, k] == this.ZoneName || areaArray[i, j, k] == 0))
                                        {
                                            sameZone = false;
                                        }
                                    }
                                }
                            }

                            if (emptyBool && sameZone)
                            {
                                //if everything is fine change bool to  true and skip the loop...
                                Rhino.RhinoApp.WriteLine(" ");
                                Rhino.RhinoApp.WriteLine("Rotation Angle is:  " + RotationOpt[g].ToString());
                                Rhino.RhinoApp.WriteLine("MirrorOption is:  " + MirrorOpt[f].ToString());
                                Rhino.RhinoApp.WriteLine("Sliding base position is:  " + h.ToString());
                                Rhino.RhinoApp.WriteLine("Base VEctor is :  " + this.BaseVector.ToString());
                                Rhino.RhinoApp.WriteLine(" ");

                                suitableDirection = true;
                                Rhino.RhinoApp.WriteLine("Place Front: Conditions are  satisfied********************************************************************************************************** ");
                                goto SkipLoop;
                                // BREAK THE LOOP IF A SOLUTION IS FOUND
                            }
                            else
                            {

                                // even if conditions are false we need to return out parameter!
                                // so we  are creating empty box but we are also returning PlacementFailed bool
                                // so main function will not continue over this iteration anymore!

                                Rhino.RhinoApp.WriteLine("Place Front: Conditions are not satisfied ");
                                suitableDirection = false;
                                // RESET DRIECTION AND MIRROR
                                VectorMargin = this.VectorMarginCache;
                                VectorObject = this.VectorObjectCache;
                            }
                        }
                        else
                        {
                            Rhino.RhinoApp.WriteLine("Place Front:  Conditions are not satisfied ");
                            suitableDirection = false;
                            // RESET DRIECTION AND MIRROR
                            VectorMargin = this.VectorMarginCache;
                            VectorObject = this.VectorObjectCache;
                        }
                    }
                }
            }
        SkipLoop:
            return suitableDirection;// return true!
        }

        public void PlaceFront(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
        {

            /**
              we are gonna use these start and end values in for loops but the problem here is we don't know if the new point after a vector is added is 
              lower than the base point or not. So we need o compare each axis'start end end conditions and find the min and max values.
              like that for loops can work without any problem.
             */

            int XLoopStartMargin = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));
            int XLoopEndMargin = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));

            int YLoopStartMargin = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);
            int YLoopEnMargin = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);

            int ZLoopStartMargin = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);
            int ZLoopEndMargin = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);


            int XLoopStartObject = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));
            int XLoopEndObject = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));

            int YLoopStartObject = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);
            int YLoopEnObject = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);

            int ZLoopStartObject = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);
            int ZLoopEndObject = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);

            //place margin
            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
            {
                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                {
                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                    {
                        emptyFullArray[i, j, k] = 2; // 2 represents margin
                    }
                }
            }
            //Place Object, object is smaller than margin
            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
            {
                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                {
                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                    {
                        emptyFullArray[i, j, k] = 1; // 1 represents object
                    }
                }
            }


            //Rhino.RhinoApp.WriteLine("Base Vector in placeFront is :  " + this.BaseVector.ToString());

            // Create output Boxes. !  For margin and Objects
            Point3d pointa = new Point3d(baseX, baseY, baseZ);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(0, this.VectorObject.X);
            Interval yInterval = new Interval(0, this.VectorObject.Y);
            Interval zInterval = new Interval(0, this.VectorObject.Z);

            ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

            xInterval = new Interval(0, this.VectorMargin.X);
            yInterval = new Interval(0, this.VectorMargin.Y);
            zInterval = new Interval(0, this.VectorMargin.Z);

            marginBox = new Box(planebase, xInterval, yInterval, zInterval);

        }

        public bool CheckRight(int[,,] emptyFullArray, int[,,] areaArray)
        {

            /*****************************/
            bool suitableDirection = true;

            int baseSliderMin = 0;
            int baseSliderMax = 0;

            Fisher_Yates_CardDeck_Shuffle(MirrorOpt);
            Fisher_Yates_CardDeck_Shuffle(RotationOpt);

            for (int f = 0; f < this.MirrorOpt.Count; f++)
            {
                for (int g = 0; g < this.RotationOpt.Count; g++)
                {
                    if (PreviousObject.AppliedMirror == 0)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y))
                          : (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y))
                          : (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX)));
                    }

                    if (PreviousObject.AppliedMirror == 1)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    if (PreviousObject.AppliedMirror == 2)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    if (PreviousObject.AppliedMirror == 3)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Max(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    for (int h = baseSliderMin; h <= baseSliderMax; h = h + this.CellSize)

                    {
                        this.Mirroring(MirrorOpt[f]);
                        this.Rotate((int)RotationOpt[g]);

                        AppliedRotation = RotationOpt[g];
                        AppliedMirror = MirrorOpt[f];

                        bool inLimits = true;
                        bool emptyBool = true;
                        bool sameZone = true;

                        suitableDirection = true;
                        bool initialDirectionCheck = false;

                        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
                        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
                        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

                        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
                        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
                        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

                        /*
                        Rhino.RhinoApp.WriteLine(" MARGIN  Front numbers -----------------------" + XCellMargin.ToString() + ' ' + YCellMargin.ToString() + ' ' + ZCellMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine(" OBJECT  Front numbers -----------------------" + XCellObject.ToString() + ' ' + YCellObject.ToString() + ' ' + ZCellObject.ToString() + ' ');
                        */

                        if (PreviousObject.AppliedMirror == 0)
                        {
                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0)
                            {
                                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                                this.baseY = h;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = h;
                                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 2)
                        {

                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0)
                            {
                                this.baseX = h;
                                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                                this.baseZ = PreviousObject.baseZ;

                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                                this.baseY = h;
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 3)
                        {

                            if (VectorMargin.X * VectorMargin.Y > 0)
                            {
                                this.baseX = h;
                                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {

                                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                                this.baseY = h; // slide over Y axis , X axis should be constant
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }
                        Point3d pointa = new Point3d(0, 0, 0);
                        Vector3d pointb = new Vector3d(0, 0, 1);
                        Plane planee = new Plane(pointa, pointb);

                        BaseVector = new Vector3d(baseX, baseY, baseZ);

                        if (PreviousObject.AppliedMirror == 0)
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X > 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y > 0) || // if previous object looks left //  X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0)       // if previous object looks right // X value should be positive
                              )

                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 1) // Z
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X > 0) ||
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0) ||
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y > 0) ||
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0)
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 2) // x
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y > 0) ||
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0) ||
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X < 0) ||
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0)
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 3) // Y
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y > 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X < 0) || // if previous object looks left  // X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0)      // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }

                        }
                        else
                        {
                        }

                        if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 660) && (baseZ + VectorMargin.Z < 300) &&
                          (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
                        {
                            inLimits = false;
                        }
                        // Rhino.RhinoApp.WriteLine("Bools " + suitableDirection.ToString() + "    " + inLimits.ToString() + ' ');

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

                        if (inLimits && initialDirectionCheck)
                        {
                            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                            {
                                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                                {
                                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see an object in a cell! )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            Rhino.RhinoApp.WriteLine("Front Margin Array is full, Loop numbers ------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }

                                        // 2nd Loop checks if the zone is the same or empty zone // Actually margin can ovewflow to other zones
                                        /*
                                        if (!(areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0))
                                        {
                                        sameZone = false;
                                        }
                                        */
                                    }
                                }
                            }
                            // 2nd Loop checks if the zone is the same or empty zone
                            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                            {
                                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                                {
                                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see margin or object in the cell )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            Rhino.RhinoApp.WriteLine("Front object Array is full: Loop numbers ---------------------------------- i:" + i.ToString() + 'j' + j.ToString() + 'k' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }

                                        // 2nd Loop checks if the zone is the same or empty zone
                                        if (!(areaArray[i, j, k] == this.ZoneName || areaArray[i, j, k] == 0))
                                        {
                                            sameZone = false;
                                        }
                                    }
                                }
                            }

                            if (emptyBool && sameZone)
                            {
                                Rhino.RhinoApp.WriteLine(" ");
                                Rhino.RhinoApp.WriteLine("Rotation Angle is:  " + RotationOpt[g].ToString());
                                Rhino.RhinoApp.WriteLine("MirrorOption is:  " + MirrorOpt[f].ToString());
                                Rhino.RhinoApp.WriteLine("Sliding base position is:  " + h.ToString());
                                Rhino.RhinoApp.WriteLine("Base VEctor is :  " + this.BaseVector.ToString());
                                Rhino.RhinoApp.WriteLine(" ");

                                suitableDirection = true;
                                Rhino.RhinoApp.WriteLine("Place Right: Conditions are  satisfied********************************************************************************************************** ");
                                goto SkipLoop;
                                // BREAK THE LOOP IF A SOLUTION IS FOUND
                            }
                            else
                            {
                                Rhino.RhinoApp.WriteLine("Place Right: Conditions are not satisfied ");
                                suitableDirection = false;
                                // RESET DRIECTION AND MIRROR
                                VectorMargin = this.VectorMarginCache;
                                VectorObject = this.VectorObjectCache;
                            }
                        }
                        else
                        {
                            Rhino.RhinoApp.WriteLine("Place Right:  Conditions are not satisfied ");
                            suitableDirection = false;
                            // RESET DRIECTION AND MIRROR
                            VectorMargin = this.VectorMarginCache;
                            VectorObject = this.VectorObjectCache;
                        }
                    }
                }
            }
        SkipLoop:
            return suitableDirection;
        }

        public void PlaceRight(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
        {
            int XLoopStartMargin = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));
            int XLoopEndMargin = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));

            int YLoopStartMargin = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);
            int YLoopEnMargin = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);

            int ZLoopStartMargin = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);
            int ZLoopEndMargin = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);


            int XLoopStartObject = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));
            int XLoopEndObject = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));

            int YLoopStartObject = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);
            int YLoopEnObject = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);

            int ZLoopStartObject = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);
            int ZLoopEndObject = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);

            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
            {
                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                {
                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                    {
                        Rhino.RhinoApp.WriteLine("Front object Array is full: Loop numbers ---------------------------------- i:" + i.ToString() + 'j' + j.ToString() + 'k' + k.ToString() + ' ');
                        emptyFullArray[i, j, k] = 2; // 2 represents margin
                    }
                }
            }
            //Place Object, object is smaller than margin
            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
            {
                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                {
                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                    {
                        emptyFullArray[i, j, k] = 1; // 1 represents object
                    }
                }
            }

            Point3d pointa = new Point3d(baseX, baseY, baseZ);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(0, this.VectorObject.X);
            Interval yInterval = new Interval(0, this.VectorObject.Y);
            Interval zInterval = new Interval(0, this.VectorObject.Z);

            ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

            xInterval = new Interval(0, this.VectorMargin.X);
            yInterval = new Interval(0, this.VectorMargin.Y);
            zInterval = new Interval(0, this.VectorMargin.Z);

            marginBox = new Box(planebase, xInterval, yInterval, zInterval);

        }

        public bool CheckBack(int[,,] emptyFullArray, int[,,] areaArray)
        {
            /**/
            bool suitableDirection = true;

            int baseSliderMin = 0;
            int baseSliderMax = 0;

            Fisher_Yates_CardDeck_Shuffle(MirrorOpt);
            Fisher_Yates_CardDeck_Shuffle(RotationOpt);

            for (int f = 0; f < this.MirrorOpt.Count; f++)
            {
                for (int g = 0; g < this.RotationOpt.Count; g++)
                {
                    if (PreviousObject.AppliedMirror == 0)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    if (PreviousObject.AppliedMirror == 1)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    if (PreviousObject.AppliedMirror == 2)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    if (PreviousObject.AppliedMirror == 3)
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    for (int h = baseSliderMin; h <= baseSliderMax; h = h + this.CellSize)

                    {
                        this.Mirroring(MirrorOpt[f]);
                        this.Rotate((int)RotationOpt[g]);

                        AppliedRotation = RotationOpt[g];
                        AppliedMirror = MirrorOpt[f];

                        bool inLimits = true;
                        bool emptyBool = true;
                        bool sameZone = true;

                        suitableDirection = true;
                        bool initialDirectionCheck = false;

                        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
                        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
                        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

                        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
                        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
                        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

                        /*
                        Rhino.RhinoApp.WriteLine(" MARGIN  Front numbers -----------------------" + XCellMargin.ToString() + ' ' + YCellMargin.ToString() + ' ' + ZCellMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine(" OBJECT  Front numbers -----------------------" + XCellObject.ToString() + ' ' + YCellObject.ToString() + ' ' + ZCellObject.ToString() + ' ');
                        */

                        if (PreviousObject.AppliedMirror == 0)
                        {
                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0) // back checked true
                            {
                                this.baseX = h; // slide over X axis , Y axis should be constant
                                this.baseY = PreviousObject.baseY;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)// back checked true
                            {
                                this.baseX = PreviousObject.baseX;
                                this.baseY = h; // slide over Y axis , X axis should be constant
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 2) // back checked true
                        {
                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = h; // slide over X axis , Y axis should be constant
                                this.baseY = PreviousObject.baseY;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0)
                            {
                                this.baseX = PreviousObject.baseX;
                                this.baseY = h; // slide over Y axis , X axis should be constant
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 3) // back checked true
                        {
                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = h; // slide over X axis , Y axis should be constant
                                this.baseY = PreviousObject.baseY;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (VectorMargin.X * VectorMargin.Y > 0)
                            {
                                this.baseX = PreviousObject.baseX;
                                this.baseY = h; // slide over Y axis , X axis should be constant
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        Point3d pointa = new Point3d(0, 0, 0);
                        Vector3d pointb = new Vector3d(0, 0, 1);
                        Plane planee = new Plane(pointa, pointb);

                        Vector3d vecX1 = new Vector3d(1, 0, 0);
                        BaseVector = new Vector3d(baseX, baseY, baseZ);

                        if (PreviousObject.AppliedMirror == 0) //back true
                        {
                            if (
                              PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y < 0 || //++
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0 || //--
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X > 0 || //-+
                              PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0)   //+-

                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 1) // Z 
                        {

                            if (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y < 0 || //++
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0 || //--
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X > 0 || //-+
                              PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0       // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 2) // x back true
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X < 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y < 0) || // if previous object looks left //  X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0)     // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 3) // Y
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X < 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y < 0) || // if previous object looks left  // X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0)      // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }

                        }
                        else
                        {
                            /*
                            if (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y > 0 || // if previous object looks front // Y value should be positive
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0 || // if previous object looks back  // Y value should be negative
                              PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X < 0 || // if previous object looks left //  X value should be positive
                              PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0       // if previous object looks right // X value should be positive
                              )
                            {
                              initialDirectionCheck = true;
                            }
                              */
                        }

                        if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 660) && (baseZ + VectorMargin.Z < 300) &&
                          (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
                        {
                            inLimits = false;
                        }
                        // Rhino.RhinoApp.WriteLine("Bools " + suitableDirection.ToString() + "    " + inLimits.ToString() + ' ');

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

                        /*  
                                    Rhino.RhinoApp.WriteLine("X  Front  Margin Loop numbers -------------" + XLoopStartMargin.ToString() + ' ' + XLoopEndMargin.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Y  Front Margin  Loop numbers -------------" + YLoopStartMargin.ToString() + ' ' + YLoopEnMargin.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Z  Front  Margin Loop numbers -------------" + ZLoopStartMargin.ToString() + ' ' + ZLoopEndMargin.ToString() + ' ');

                                    Rhino.RhinoApp.WriteLine("X  Front Object Loop numbers ----------" + XLoopStartObject.ToString() + ' ' + XLoopEndObject.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Y  Front Object  Loop numbers ---------" + YLoopStartObject.ToString() + ' ' + YLoopEnObject.ToString() + ' ');
                                    Rhino.RhinoApp.WriteLine("Z  Front Object  Loop numbers ---------" + ZLoopStartObject.ToString() + ' ' + ZLoopEndObject.ToString() + ' ');
                        */
                        if (inLimits && initialDirectionCheck)
                        {
                            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                            {
                                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                                {
                                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see an object in a cell! )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            //Rhino.RhinoApp.WriteLine("Front Margin Array is full, Loop numbers ------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }

                                        // 2nd Loop checks if the zone is the same or empty zone // Actually margin can ovewflow to other zones
                                        /*
                                        if (!(areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0))
                                        {
                                        sameZone = false;
                                        }
                                        */
                                    }
                                }
                            }
                            // 2nd Loop checks if the zone is the same or empty zone
                            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                            {
                                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                                {
                                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see margin or object in the cell )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            //Rhino.RhinoApp.WriteLine("Front object Array is full: Loop numbers ---------------------------------- i:" + i.ToString() + 'j' + j.ToString() + 'k' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }
                                        // 2nd Loop checks if the zone is the same or empty zone
                                        if (!(areaArray[i, j, k] == this.ZoneName || areaArray[i, j, k] == 0))
                                        {
                                            sameZone = false;
                                        }
                                    }
                                }
                            }

                            if (emptyBool && sameZone)
                            {
                                /*
                                Rhino.RhinoApp.WriteLine(" ");
                                Rhino.RhinoApp.WriteLine("Rotation Angle is:  " + RotationOpt[g].ToString());
                                Rhino.RhinoApp.WriteLine("MirrorOption is:  " + MirrorOpt[f].ToString());
                                Rhino.RhinoApp.WriteLine("Sliding base position is:  " + h.ToString());
                                Rhino.RhinoApp.WriteLine("Base VEctor is :  " + this.BaseVector.ToString());
                                Rhino.RhinoApp.WriteLine(" ");
                                */

                                // In order the follow placement Informations uncomment the lines upwards.
                                // Usefull for debugging

                                suitableDirection = true;
                                Rhino.RhinoApp.WriteLine("Place Front: Conditions are  satisfied");
                                goto SkipLoop;
                                // BREAK THE LOOP IF A SOLUTION IS FOUND
                            }
                            else
                            {
                                Rhino.RhinoApp.WriteLine("Place Front: Conditions are not satisfied ");
                                suitableDirection = false;
                                // RESET DRIECTION AND MIRROR
                                VectorMargin = this.VectorMarginCache;
                                VectorObject = this.VectorObjectCache;
                            }
                        }
                        else
                        {
                            Rhino.RhinoApp.WriteLine("Place Front:  Conditions are not satisfied ");
                            suitableDirection = false;
                            // RESET DRIECTION AND MIRROR
                            VectorMargin = this.VectorMarginCache;
                            VectorObject = this.VectorObjectCache;
                        }
                    }
                }
            }
        SkipLoop:
            return suitableDirection;

        }

        public void PlaceBack(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
        {
            int XLoopStartMargin = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));
            int XLoopEndMargin = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));

            int YLoopStartMargin = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);
            int YLoopEnMargin = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);

            int ZLoopStartMargin = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);
            int ZLoopEndMargin = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);


            int XLoopStartObject = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));
            int XLoopEndObject = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));

            int YLoopStartObject = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);
            int YLoopEnObject = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);

            int ZLoopStartObject = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);
            int ZLoopEndObject = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);

            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
            {
                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                {
                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                    {
                        emptyFullArray[i, j, k] = 2; // 2 represents margin
                    }
                }
            }
            //Place Object, object is smaller than margin
            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
            {
                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                {
                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                    {
                        emptyFullArray[i, j, k] = 1; // 1 represents object
                    }
                }
            }

            Point3d pointa = new Point3d(baseX, baseY, baseZ);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(0, this.VectorObject.X);
            Interval yInterval = new Interval(0, this.VectorObject.Y);
            Interval zInterval = new Interval(0, this.VectorObject.Z);

            ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

            xInterval = new Interval(0, this.VectorMargin.X);
            yInterval = new Interval(0, this.VectorMargin.Y);
            zInterval = new Interval(0, this.VectorMargin.Z);

            marginBox = new Box(planebase, xInterval, yInterval, zInterval);
        }

        public bool CheckLeft(int[,,] emptyFullArray, int[,,] areaArray)
        {

            /*****************************/
            bool suitableDirection = true;

            int baseSliderMin = 0;
            int baseSliderMax = 0;

            Fisher_Yates_CardDeck_Shuffle(MirrorOpt);
            Fisher_Yates_CardDeck_Shuffle(RotationOpt);

            for (int f = 0; f < this.MirrorOpt.Count; f++)
            {
                for (int g = 0; g < this.RotationOpt.Count; g++)
                {
                    if (PreviousObject.AppliedMirror == 0) // left okay
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y))
                          : (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y))
                          : (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX)));
                    }

                    if (PreviousObject.AppliedMirror == 1) // oaky
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y))
                          : (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X)));
                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0 ? (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y))
                          : (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX)));
                    }

                    if (PreviousObject.AppliedMirror == 2) //okay
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    if (PreviousObject.AppliedMirror == 3)//okay
                    {
                        baseSliderMin = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Min(PreviousObject.baseX, PreviousObject.baseX + PreviousObject.VectorMargin.X))
                          : (Math.Min(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));

                        baseSliderMax = (int)(PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0 ? (Math.Max(PreviousObject.baseX + PreviousObject.VectorMargin.X, PreviousObject.baseX))
                          : (Math.Max(PreviousObject.baseY, PreviousObject.baseY + PreviousObject.VectorMargin.Y)));
                    }

                    for (int h = baseSliderMin; h <= baseSliderMax; h = h + this.CellSize)

                    {
                        this.Mirroring(MirrorOpt[f]);
                        this.Rotate((int)RotationOpt[g]);

                        AppliedRotation = RotationOpt[g];
                        AppliedMirror = MirrorOpt[f];

                        bool inLimits = true;
                        bool emptyBool = true;
                        bool sameZone = true;

                        suitableDirection = true;
                        bool initialDirectionCheck = false;

                        this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
                        this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
                        this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

                        this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
                        this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
                        this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

                        /*
                        Rhino.RhinoApp.WriteLine(" MARGIN  Front numbers -----------------------" + XCellMargin.ToString() + ' ' + YCellMargin.ToString() + ' ' + ZCellMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine(" OBJECT  Front numbers -----------------------" + XCellObject.ToString() + ' ' + YCellObject.ToString() + ' ' + ZCellObject.ToString() + ' ');
                        */

                        if (PreviousObject.AppliedMirror == 0) // left kontrol edildi dogru
                        {
                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0)
                            {
                                this.baseX = PreviousObject.baseX;
                                this.baseY = h;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = h;
                                this.baseY = PreviousObject.baseY;
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 2) // left checked true
                        {

                            if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y > 0)
                            {
                                this.baseX = h;
                                this.baseY = PreviousObject.baseY;
                                this.baseZ = PreviousObject.baseZ;

                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = PreviousObject.baseX;
                                this.baseY = h;
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        if (PreviousObject.AppliedMirror == 3)// left checked true
                        {

                            if (VectorMargin.X * VectorMargin.Y > 0)
                            {
                                this.baseX = h;
                                this.baseY = PreviousObject.baseY;
                                this.baseZ = PreviousObject.baseZ;
                            }
                            else if (PreviousObject.VectorMargin.X * PreviousObject.VectorMargin.Y < 0)
                            {
                                this.baseX = PreviousObject.baseX;
                                this.baseY = h;
                                this.baseZ = PreviousObject.baseZ;
                            }
                        }

                        Point3d pointa = new Point3d(0, 0, 0);
                        Vector3d pointb = new Vector3d(0, 0, 1);
                        Plane planee = new Plane(pointa, pointb);

                        BaseVector = new Vector3d(baseX, baseY, baseZ);

                        if (PreviousObject.AppliedMirror == 0) // Left is checked
                        {
                            if (

                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X < 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y > 0) || // if previous object looks left //  X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0)       // if previous object looks right // X value should be positive

                              )

                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 1) // Z Left is checked
                        {
                            if (

                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.X < 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X > 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.Y > 0) || // if previous object looks left //  X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y < 0)       // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 2) // x  Left is checked
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y < 0) ||
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0) ||
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X > 0) ||
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0)
                              )
                            {
                                initialDirectionCheck = true;
                            }
                        }

                        else if (PreviousObject.AppliedMirror == 3) // Y Left is checked
                        {
                            if (
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y > 0 && VectorObject.Y < 0) || // if previous object looks front // Y value should be positive
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.Y > 0) || // if previous object looks back  // Y value should be negative
                              (PreviousObject.VectorMargin.X < 0 && PreviousObject.VectorMargin.Y > 0 && VectorMargin.X > 0) || // if previous object looks left  // X value should be positive
                              (PreviousObject.VectorMargin.X > 0 && PreviousObject.VectorMargin.Y < 0 && VectorMargin.X < 0)      // if previous object looks right // X value should be positive
                              )
                            {
                                initialDirectionCheck = true;
                            }

                        }
                        else
                        {
                        }

                        if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 660) && (baseZ + VectorMargin.Z < 300) &&
                          (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
                        {
                            inLimits = false;
                        }
                        // Rhino.RhinoApp.WriteLine("Bools " + suitableDirection.ToString() + "    " + inLimits.ToString() + ' ');

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

                        if (inLimits && initialDirectionCheck)
                        {
                            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                            {
                                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                                {
                                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see an object in a cell! )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            //Rhino.RhinoApp.WriteLine("Front Margin Array is full, Loop numbers ------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }

                                        // 2nd Loop checks if the zone is the same or empty zone // Actually margin can ovewflow to other zones
                                        /*
                                        if (!(areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0))
                                        {
                                        sameZone = false;
                                        }
                                        */
                                    }
                                }
                            }
                            // 2nd Loop checks if the zone is the same or empty zone
                            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                            {
                                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                                {
                                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                                    {
                                        //checks if the cell is empty (We dont want to see margin or object in the cell )
                                        if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                                        {
                                            //Rhino.RhinoApp.WriteLine("Front object Array is full: Loop numbers ---------------------------------- i:" + i.ToString() + 'j' + j.ToString() + 'k' + k.ToString() + ' ');
                                            emptyBool = false;
                                        }

                                        // 2nd Loop checks if the zone is the same or empty zone
                                        if (!(areaArray[i, j, k] == this.ZoneName || areaArray[i, j, k] == 0))
                                        {
                                            sameZone = false;
                                        }
                                    }
                                }
                            }

                            if (emptyBool && sameZone)
                            {
                                Rhino.RhinoApp.WriteLine(" ");
                                Rhino.RhinoApp.WriteLine("Rotation Angle is:  " + RotationOpt[g].ToString());
                                Rhino.RhinoApp.WriteLine("MirrorOption is:  " + MirrorOpt[f].ToString());
                                Rhino.RhinoApp.WriteLine("Sliding base position is:  " + h.ToString());
                                Rhino.RhinoApp.WriteLine("Base VEctor is :  " + this.BaseVector.ToString());
                                Rhino.RhinoApp.WriteLine(" ");

                                suitableDirection = true;
                                Rhino.RhinoApp.WriteLine("Place Right: Conditions are  satisfied********************************************************************************************************** ");
                                goto SkipLoop;
                                // BREAK THE LOOP IF A SOLUTION IS FOUND
                            }
                            else
                            {
                                Rhino.RhinoApp.WriteLine("Place Right: Conditions are not satisfied ");
                                suitableDirection = false;
                                // RESET DRIECTION AND MIRROR
                                VectorMargin = this.VectorMarginCache;
                                VectorObject = this.VectorObjectCache;
                            }
                        }
                        else
                        {
                            Rhino.RhinoApp.WriteLine("Place Right:  Conditions are not satisfied ");
                            suitableDirection = false;
                            // RESET DRIECTION AND MIRROR
                            VectorMargin = this.VectorMarginCache;
                            VectorObject = this.VectorObjectCache;
                        }
                    }
                }
            }
        SkipLoop:
            return suitableDirection;
        }

        public void PlaceLeft(int[,,] emptyFullArray, out Box ObjBox, out Box marginBox)
        {
            int XLoopStartMargin = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));
            int XLoopEndMargin = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));

            int YLoopStartMargin = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);
            int YLoopEnMargin = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);

            int ZLoopStartMargin = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);
            int ZLoopEndMargin = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);


            int XLoopStartObject = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));
            int XLoopEndObject = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));

            int YLoopStartObject = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);
            int YLoopEnObject = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);

            int ZLoopStartObject = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);
            int ZLoopEndObject = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);

            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
            {
                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                {
                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                    {
                        emptyFullArray[i, j, k] = 2; // 2 represents margin
                    }
                }
            }
            //Place Object, object is smaller than margin
            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
            {
                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                {
                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                    {
                        emptyFullArray[i, j, k] = 1; // 1 represents object
                    }
                }
            }

            Point3d pointa = new Point3d(baseX, baseY, baseZ);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(0, this.VectorObject.X);
            Interval yInterval = new Interval(0, this.VectorObject.Y);
            Interval zInterval = new Interval(0, this.VectorObject.Z);

            ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

            xInterval = new Interval(0, this.VectorMargin.X);
            yInterval = new Interval(0, this.VectorMargin.Y);
            zInterval = new Interval(0, this.VectorMargin.Z);

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
                this.baseZ = PreviousObject.baseZ + PreviousObject.ZCellMargin * this.CellSize;
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

            if (!((baseX + this.VectorMargin.X < 360) && (baseY + this.VectorMargin.Y < 660) && (baseZ + this.VectorMargin.Z < 300) &&
              (baseX + this.VectorMargin.X > 0) && (baseY + this.VectorMargin.Y > 0) && (baseZ + this.VectorMargin.Z > 0)))
            {
                inLimits = false;
            }


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

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 660) && (baseZ + VectorMargin.Z < 300) &&
              (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
            {
                inLimits = false;
            }

            if (inLimits && suitableDirection)
            {

                for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                {
                    for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                    {
                        for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                        {
                            //checks if the cell is empty (We dont want to see an object in a cell! )
                            if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                            {
                                emptyBool = false;
                            }

                            // 2nd Loop checks if the zone is the same or empty zone // Actually margin can ovewflow to other zones
                            /*
                            if (!(areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0))
                            {
                            sameZone = false;
                            }
                            */
                        }
                    }
                }
                // 2nd Loop checks if the zone is the same or empty zone
                for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                {
                    for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                    {
                        for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                        {
                            //checks if the cell is empty (We dont want to see margin or object in the cell )
                            if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                            {
                                emptyBool = false;
                            }

                            // 2nd Loop checks if the zone is the same or empty zone
                            if (!(areaArray[i, j, k] == this.ZoneName || areaArray[i, j, k] == 0))
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

            int XLoopStartMargin = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));
            int XLoopEndMargin = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));

            int YLoopStartMargin = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);
            int YLoopEnMargin = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);

            int ZLoopStartMargin = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);
            int ZLoopEndMargin = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);


            int XLoopStartObject = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));
            int XLoopEndObject = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));

            int YLoopStartObject = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);
            int YLoopEnObject = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);

            int ZLoopStartObject = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);
            int ZLoopEndObject = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);

            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
            {
                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                {
                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                    {
                        emptyFullArray[i, j, k] = 2; // 2 represents margin
                    }
                }
            }
            //Place Object, object is smaller than margin
            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
            {
                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                {
                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                    {
                        emptyFullArray[i, j, k] = 1; // 1 represents object
                    }
                }
            }

            Point3d pointa = new Point3d(baseX, baseY, baseZ);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(0, this.VectorObject.X);
            Interval yInterval = new Interval(0, this.VectorObject.Y);
            Interval zInterval = new Interval(0, this.VectorObject.Z);

            ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

            xInterval = new Interval(0, this.VectorMargin.X);
            yInterval = new Interval(0, this.VectorMargin.Y);
            zInterval = new Interval(0, this.VectorMargin.Z);

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

            if (!((baseX + this.ObjWidth < 360) && (baseY + this.ObjLength < 660) && (baseZ + this.ObjHeight < 300) &&
              (baseX + this.ObjWidth > 0) && (baseY + this.ObjLength > 0) && (baseZ + this.ObjHeight > 0)))
            {
                inLimits = false;
            }


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

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 660) && (baseZ + VectorMargin.Z < 300) &&
              (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
            {
                inLimits = false;
            }

            if (inLimits && suitableDirection)
            {

                for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                {
                    for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                    {
                        for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                        {
                            //checks if the cell is empty (We dont want to see an object in a cell! )
                            if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                            {
                                emptyBool = false;
                            }

                            // 2nd Loop checks if the zone is the same or empty zone // Actually margin can ovewflow to other zones
                            /*
                            if (!(areaArray[i, j, k] != this.ZoneName || areaArray[i, j, k] != 0))
                            {
                            sameZone = false;
                            }
                            */
                        }
                    }
                }
                // 2nd Loop checks if the zone is the same or empty zone
                for (int i = XLoopStartObject; i < XLoopEndObject; i++)
                {
                    for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                    {
                        for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                        {
                            //checks if the cell is empty (We dont want to see margin or object in the cell )
                            if (((emptyFullArray[i, j, k] == 1) || (emptyFullArray[i, j, k] == 2)))
                            {
                                emptyBool = false;
                            }

                            // 2nd Loop checks if the zone is the same or empty zone
                            if (!(areaArray[i, j, k] == this.ZoneName || areaArray[i, j, k] == 0))
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
            int XLoopStartMargin = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));
            int XLoopEndMargin = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellMargin) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellMargin));

            int YLoopStartMargin = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);
            int YLoopEnMargin = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellMargin ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellMargin);

            int ZLoopStartMargin = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);
            int ZLoopEndMargin = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellMargin ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellMargin);


            int XLoopStartObject = (int)((int)(baseX / CellSize) < (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));
            int XLoopEndObject = (int)((int)(baseX / CellSize) > (baseX / CellSize + (int)XCellObject) ? (int)(baseX / CellSize) : (baseX / CellSize + (int)XCellObject));

            int YLoopStartObject = (int)((int)(baseY / CellSize) < baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);
            int YLoopEnObject = (int)((int)(baseY / CellSize) > baseY / CellSize + (int)YCellObject ? (int)(baseY / CellSize) : baseY / CellSize + (int)YCellObject);

            int ZLoopStartObject = (int)((int)(baseZ / 150) < baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);
            int ZLoopEndObject = (int)((int)(baseZ / 150) > baseZ / 150 + (int)ZCellObject ? (int)(baseZ / 150) : baseZ / 150 + (int)ZCellObject);

            for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
            {
                for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                {
                    for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                    {
                        emptyFullArray[i, j, k] = 2; // 2 represents margin
                    }
                }
            }
            //Place Object, object is smaller than margin
            for (int i = XLoopStartObject; i < XLoopEndObject; i++)
            {
                for (int j = YLoopStartObject; j < YLoopEnObject; j++)
                {
                    for (int k = ZLoopStartObject; k < ZLoopEndObject; k++)
                    {
                        emptyFullArray[i, j, k] = 1; // 1 represents object
                    }
                }
            }

            Point3d pointa = new Point3d(baseX, baseY, baseZ);
            Vector3d pointb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(0, this.VectorObject.X);
            Interval yInterval = new Interval(0, this.VectorObject.Y);
            Interval zInterval = new Interval(0, this.VectorObject.Z);

            ObjBox = new Box(planebase, xInterval, yInterval, zInterval);

            xInterval = new Interval(0, this.VectorMargin.X);
            yInterval = new Interval(0, this.VectorMargin.Y);
            zInterval = new Interval(0, this.VectorMargin.Z);

            marginBox = new Box(planebase, xInterval, yInterval, zInterval);
        }
    }


}