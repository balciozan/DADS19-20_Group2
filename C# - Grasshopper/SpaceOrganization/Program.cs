using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.Linq; // This is the Library to modify List and data in general fast and practical ways. LINQ is advanced topic of C#


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
    private void RunScript(object x, int Width, int Length, int Height, int LevelHeight, int cellSize, Box ExteriorDoor, Box ExteriorDoorMargin, Box ChangeOverZone, Box ChangeOverZoneMargin, Box Closet, Box ClosetMargin, Box InteriorDoor, Box InteriorDoorMargin, Box Counter, Box CounterMargin, Box Table, Box TableMargin, Box SittingUnit, Box SittingUnitMargin, Box Bed, Box BedMargin, Box HealtCare, Box HealthCareMargin, Box Toilet, Box ToiletMargin, Box Sink, Box SinkMargin, Box SowerCabin, object ShowerCabinMargin, ref object A, ref object EntranceBoxes, ref object CommonBoxes, ref object WetAreaBoxes, ref object Y, ref object K, ref object W, ref object MV, ref object OV, ref object Base_Points, ref object Furnitures, ref object FurnituresMargins)
    {
        /*
         * 
         * 
         * 
         * ...............................................................................

            ..................................+@WWWWWWW@=-.................................

            ................................#WWWWWWWWWWWWWW+...............................

            ..............................-WWWWWWWWWWWWWWWWW=..............................

            .............................-@W@WWWWWWWWWWWWW@@W*.............................

            .............................*W:...*WWWWWWWW*...:@-............................

            .............................#@..-..:WWWWWW+..-..@+............................

            .............................*#.....=W@--WW=.....=*............................

            ............................-+:+....=WW++WW#....:-*............................

            ............................::.-*:.....--.....+*..*............................

            ............................*:....-:*=====*+:.....*:...........................

            ...........................:@.....................:@...........................

            ..........................+W:......................*@-.........................

            ........................-@W-........................+W=........................

            .......................=W*............................#W*......................

            .....................:W=...............................-@@.....................

            ....................:W:..................................*@-...................

            ...................-@:....................................+#...................

            ...................#+......................................=+..................

            ..................:=........................................#-.................

            ..................=-........................................++.................

            .................-=.........................................-=.................

            .................++..........................................*-................

            .................=-..........................................++................

            .................=...........................................-*................

            .................*............................................*................

            ................-+..+.....................................-+..=................

            ................-:.-@.....................................+#..*................

            ................-+.##-....................................*=*.*................

            .................@W=:+....................................=-@@#................

            .................-:.-=...................................:=..+-................

            .....................#:..................................#-....................

            .....................-@-................................*=.....................

            ......................+@-..............................*#......................

            .......................+W*............................#@-......................

            ........................-@W=-.......................*W=........................

            ..........................-#WW#+=#:+@+....=#:+@+-#@W@-.........................

            ............................@WWWWWWWWW#==@WWWWWWWWW=...........................

            ............................#WW@WW@WWW==##WW@WWW@WW*...........................

            .............................::.-+..+.....-+..+..+-............................
         * This Project is created by a KingPenuin Team. Cem Gunes, Efay Tan, Nurcan Sutcu  Pelin Arman, Ozan Balci
         * 
         * This InteriorLivingSpace Generator is developped as part of Digital Architectural Design Studio (DADS) 2019-2020 at ITU
         * Tutored by: Prof. Dr. Mine Ozkar, Prof. Dr. Yuksel Avci
         * Assistant Tutors: Koray, Inanc Sencan, Burak Delikanli, Muge Halici
         * 
         * In case of you have any question please contact us
         * 
         * This generator has 2 main parts. 1-) Zone Generator 2-) Furniture Layout Algorithm
         * Width, Height, Level Count, Cell Size are given by the user. (In this spesific solution some methods uses cell size and dimensions as static values, If you change the dimensions
         * please be sure that you updated them or use dynamic cellsize variable instead of constants.)
         * 
         * First part keeps working until it produces expected solutions. Checks min required sizes. Filter(Cellular Automata Filters) the borders in order to reduce jagged/rigged cells. 
         * We started to define Zones as Objects which can contain large amount of data in it however we faced software problem within grasshopper. 
         * So we continued in conventional methods. In case of swithing to class you may easily transfer the given methods here.
         * 
         * FurnitureLayout Alg. start whenever Zone creation is complated.  Strating from Reference Objects(which are only one in each zone and has no previous object), 
         * all the objects are tried to be fitted in given zones. All off the methods and properties related Layout Solution placed in Objects Class.
         * So we can keep layers of ínformations for each object.
         * 
         * 
         * 
         * - Each furniture has main boundry representations. Object and Margin. 
         * - Objects and Margins should be connected to our package in order to get size and direction informations of objects.
         * -
         * - Margins can be intersecting. It is accaptable so we can still save some space and create enough circulation area.
         * - Margins can overflow to other zones. 
         * - Objects can not intersect with objects or Margins
         * - Objects should be in the zone which is defined.
         * - Furnitures has multiple properties such as: RotationAngles, MirrorOptions, Zone, Name etc. 
         * - While placement continues all off the options are tried for aech possible base point in give direction
         * - !!All the objects are placed without any extra gap except Cell Snapping. Our Processing Code studied this case. In case of you want to keep building our method in that way
         * - Please also check KingPenguin Teams Processing folder.
         * - Some spesific cases like Television Direction, Window placements, Wall creation couldn't be codified because of the time limitations.
         * 
         * - There are prepared evaluation Flowcharts you may check them for write a method which finds the better solutions. 
         * 
         * - To solve the problem in faster way square area is choosen. New works may be focused on make our methods work in triangular cells or within another types of borders.
         * 
         * - To run program faster please comment the console string outputs. It takes time more than calculations!
         * 
         * */




        // To represent our zones as Boxes we need to be stacking them in a list
        // At the end of the main part we will be giving them to outputs

        List<Box> entranceBoxes = new List<Box>();
        List<Box> commonBoxes = new List<Box>();
        List<Box> wetAreaBoxes = new List<Box>();


        List<Box> WhiteBoxes = new List<Box>(); // empty.

        // Stack Object and Margin Boundries in Lists

        List<Box> ObjectsList = new List<Box>();
        List<Box> MarginsList = new List<Box>();

        // Expected Areas of the zones
        double entranceSize = (12 * 100 * 100) / (cellSize * cellSize);
        double commonSize = (7 * 100 * 100) / (cellSize * cellSize);
        double wetAreaSize = (7 * 100 * 100) / (cellSize * cellSize);


        // Points to create p Plane , XY plane.
        Point3d pA = new Point3d(0, 0, 0);
        Point3d pB = new Point3d(10, 0, 0);
        Point3d pC = new Point3d(0, 10, 0);

        // XY plane , used  for placing boxes
        Plane p = new Plane(pA, pB, pC);

        // Stack vectors and Points in a list.
        List<Vector3d> MarginVectors = new List<Vector3d>();
        List<Vector3d> ObjectVectors = new List<Vector3d>();
        List<Point3d> BasePoints = new List<Point3d>();

        // get the input values and assing them to new varibles. This is not necessary. variables can be used directly.
        int width = Width;
        int length = Length;
        int levelHeight = LevelHeight;
        int height = Height;
        // calc level number
        int levelNumber = Height / LevelHeight;


        // Bools to finish loop
        bool objectPlacementEnded = false;
        bool placementFailed = false;
        bool validZoning = false;

        // Initialize matrices for checking empty/full conditons or zone values.// in case of we have a class it would be more practical.
        // Should be considered in future works.
        int[,,] areaArray = InitializeZeroMatrice(length, width, levelNumber);
        int[,,] emptyFullArray = InitializeZeroMatrice(length, width, levelNumber);

        int objectReplacementCounter = 1;

        //The loop: whole algorithm works here. runs until it gives acceptable solutionor it reached enough tries.
        while ((objectPlacementEnded == false || placementFailed == true) && objectReplacementCounter < 50)
        {
            // Each Lists should be cleared in order to get rid of the elements from previous runs.
            MarginVectors.Clear();
            ObjectVectors.Clear();
            BasePoints.Clear();

            //Check Placement Counter for debug or avoiding infinite loops. // If you are sure everything works fine Comment these line program will work faster.
            Rhino.RhinoApp.WriteLine(" ");
            Rhino.RhinoApp.WriteLine("Object Replacement Counter is:   " + objectReplacementCounter);
            Rhino.RhinoApp.WriteLine(" ");

            placementFailed = false;
            validZoning = false;
            int zoneCounter = 0;
            // ZONING STARTS
            while (validZoning == false && zoneCounter < 100)
            {
                // CLEAR EACH LIST TO START EACH LOOP WITH EMPTY LIST.
                entranceList.Clear();
                commonList.Clear();
                wetAreaList.Clear();

                commonBoxes.Clear();
                wetAreaBoxes.Clear();
                entranceBoxes.Clear();
                WhiteBoxes.Clear();

                // SET EACH ONE CELL COUNTER TO 1 IN EACH LOOP
                wetAreaCounter = 1;
                commonCounter = 1;
                entranceCounter = 1;

                //INIITIALIZE A ZERO MATRICE WITH TTHE GIVEN DIMENSIONS 

                areaArray = InitializeZeroMatrice(length, width, levelNumber);
                StartZones(areaArray);// RANDOMLY ASSING ZONE VALUES

                //SPREAD EACH ZONE WITHIN DEFINED LIMITS. 
                // KEEP THE VALUES ELEMENTS IN LIST TO HAVE THEM AS OUTPUTS
                // REF keyword represent the argument which taken from already existing ones and retured as output. So ref values are already defined out of the func
                // Please check `scope` topic for further informations .
                for (int i = 0; i < width * length * 3; i++)
                {
                    Spread(areaArray, ref entranceSize, entranceList, ref entranceCounter, entranceLimit, cellSize);
                    Spread(areaArray, ref commonSize, commonList, ref commonCounter, commonLimit, cellSize);
                    Spread(areaArray, ref wetAreaSize, wetAreaList, ref wetAreaCounter, wetAreaLimit, cellSize);
                }

                // FILTER THE JAGGED BORDERS
                NoiseFilter(areaArray, 0, 3, 5); // CHEKS 8 SIDES (FILTER TYPE, REPEAT COUNT, THE NUMBER OF THE CELLS IN SAME SIZE)
                NoiseFilter(areaArray, 1, 2, 2); // CHEKS 4 SIDES

                entranceList.Clear();
                commonList.Clear();
                wetAreaList.Clear();

                //ASSIGN THE ZONE INFORMATIONS IN FIRST LEVEL TO OTHER LEVELS.
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

                // START 0 MATRICE. LATER  WITH PLACEMENTS CHECK IF THE CELL IS EMPTY , MARGINED OR FULL

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

                // PLACE BOXES ACCORDING TO ITS ZONE VALUES AND REARRANGE THE ZONELISTS

                for (int k = 0; k < levelNumber; k++)
                {
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (areaArray[i, j, k] == 1)  // if the zone is 1 =Entrance Zone
                            {
                                Interval xInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                                Interval yInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                                Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                                Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                                entranceBoxes.Add(boxgh);
                                entranceList.AddRange(new int[2] { i, j });
                            }
                            if (areaArray[i, j, k] == 2)  // if the zone is 2 = ... Zone
                            {
                                Interval xInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                                Interval yInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                                Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                                Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                                commonBoxes.Add(boxgh);
                                commonList.AddRange(new int[2] { i, j });
                            }
                            if (areaArray[i, j, k] == 3)  // if the zone is 3 = ... Zone
                            {
                                Interval xInterval = new Interval(i * cellSize, (i + 1) * (cellSize));
                                Interval yInterval = new Interval(j * cellSize, (j + 1) * (cellSize));
                                Interval zInterval = new Interval(k * LevelHeight, k * LevelHeight + levelHeight);

                                Box boxgh = new Box(p, xInterval, yInterval, zInterval);
                                wetAreaBoxes.Add(boxgh);
                                wetAreaList.AddRange(new int[2] { i, j });
                            }
                            if (areaArray[i, j, k] == 0)  // if the zone is empty
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
                // Divide ZoneLists count to 4 , because we are stacking both coordinates in one list[x1,y1,x2,y,x3,y3...] and we have 2 levels in our case!
                // IF LEVEL NUMBER IS CHANGED PLASE UPDATE THIS EQUATION IN THIS WAY. enra  entranceList.Count / 4  == entranceList.Count/ LevelNumber * 2
                if (60 * 60 * entranceList.Count / 4 >= 100 * 100 * 8 && 60 * 60 * commonList.Count / 4 >= 100 * 100 * 6 && 60 * 60 * wetAreaList.Count / 4 >= 100 * 100 * 6)
                {
                    Rhino.RhinoApp.WriteLine(" ");

                    Rhino.RhinoApp.WriteLine(" VALID ZONING is TRUE");
                    Rhino.RhinoApp.WriteLine("  ");
                    /*
                    Rhino.RhinoApp.WriteLine(" Enrance Space is :" + 60 * 60 * entranceList.Count / 4 + " > " + 100 * 100 * 6);
                    Rhino.RhinoApp.WriteLine(" entrance Count is :" + entranceList.Count / 4);
                    Rhino.RhinoApp.WriteLine(" commonList Count is :" + commonList.Count / 4);
                    Rhino.RhinoApp.WriteLine(" wetAreaList Count is :" + wetAreaList.Count / 4);
               */
                    Rhino.RhinoApp.WriteLine("  ");
                    validZoning = true;

                }
                else
                {
                    Rhino.RhinoApp.WriteLine(" ");
                    /*
                              Rhino.RhinoApp.WriteLine(" VALID ZONING is FALSE ");
                              Rhino.RhinoApp.WriteLine(" Enrance Space is :" + 60 * 60 * entranceList.Count / 4 + " > " + 100 * 100 * 6);
                              Rhino.RhinoApp.WriteLine(" entrance Count is :" + entranceList.Count / 4);
                              Rhino.RhinoApp.WriteLine(" commonList Count is :" + commonList.Count / 4);
                              Rhino.RhinoApp.WriteLine(" wetAreaList Count is :" + wetAreaList.Count / 4);*/
                    validZoning = false;
                    zoneCounter++;
                    Rhino.RhinoApp.WriteLine("Zone Counter is :" + zoneCounter);
                }
            }
            // ZONING ENDS

            emptyFullArray = InitializeZeroMatrice(length, width, levelNumber);

            //RESET THE OBJECT AND MARGIN LIST AT THE BEGINNING OF EACH ITERATIONS.
            ObjectsList.Clear();
            MarginsList.Clear();

            /* FURNITURE LAYOUT ==> ENTRANCE STARTED */

            Point3d pointa = new Point3d(0, 0, 0);
            Vector3d vectorb = new Vector3d(0, 0, 1);

            Plane planebase = new Plane(pointa, vectorb);

            //IN GRASSHOPPER WE SHOULD DEFINE INTERVALS TO REPRESENT BOXES.

            Interval xInter = new Interval(0, 10);
            Interval yInter = new Interval(0, 10);
            Interval zInter = new Interval(0, 10);

            Box ObjBox = new Box(planebase, xInter, yInter, zInter);
            Box marginBox = new Box(planebase, xInter, yInter, zInter);



            /* ENTRANCE REFERENCE */
            var exteriorDoorO = new Objects
            {
                /*enter the name of the object in case of it is needed in user interface.*/
                // get the zone value. It will be compared with placement cell.
                Name = "Exterior Door",
                ZoneName = 1,


                // get the margin and objects boxes plugged in as input.
                // we will get direction and dimenion infrmations from them
                Obj = ExteriorDoor,
                ObjMargin = ExteriorDoorMargin,

                // rotation options to be tried 
                Front = true,
                Right = true,
                Back = false,
                Left = true,
                Top = false,
                Bottom = false,

                //rotation options to be tried, elements in the lists are looping until it one of them works or stop if none...
                RotationBool = true,
                RotationOpt = new List<double>() { 0, 90, 180, 270 },

                // mirrors to be tried. 0 = default. 1 = Z, 2= X, 3=y.
                MirrorBool = false,
                MirrorOpt = new List<int>() { 0, 2, 3 },

                // FUTURE WORK: some sources can be identifed such as waste water, clean water,electricity. So their placement and connections can be codified.
                Source = "NULL",
                FixedToWall = 1,

                //Cellsize is defined to place objects snapping on grids. This is static here but it can be equalified to cellsize variable.
                CellSize = 60,
                // checks the random placement points if it is needed so we need this list.
                SpaceList = entranceList,

            };

            // Constructor method had a problem with assigning the  dimensions son we have seperate funtion to do that.
            // Place function tries to fit the given in given zone. If it is referenceOBject. No prevOBject. if not  place the object around prev object.

            // 1- Check Each possible sides.
            // 2- Check Each possible Mirror.
            // 3- Check Each possible base point node

            // FUTURE WORK: NOW THE PLACEMENT WORKS ONLY OVER THE POINTS OVER THE OBJECTS SIDE ON GIVEN DIRECTION CELL. 
            // BASE POINT SLIDES OVER THE REFERENCE SIDE
            // IT CAN ALSO SLIDE AS LONG AS SOME PART IS TOUCHING TO REFERENCE OBJECT SO BASE POINT DOES NOT HAVE TO DIRECTLY CONNECTED TO REF OBJECT.

            // FUTURE WORK 2: WE PLACE OBJECTS TO NEAREST CELLS . THERE MIGHT BE EXTRA GAP TOLERANCE. CHECK KINGPENGUIN PROCESSING CODE.

            // FUTURE WORK 3: WE CHECK COLLUSION WITH HAND WRITTEN FUNCTIONS. ACTUALLY  THERE ARE SOME METHODS IN RHINO/GRASSHOPPER. FOR SIMPLIFICATION THEY CAN BE USED



            //OUT IDENTIFIER IS USED FOR RETURNING MULTIPLE VALUES
            // IF YOU HAVE NO PRIOR INFORMATION ABOUT KEYWORDS SUCH AS --REF-- OR --OUT-- PLEASE CHECK DOCUMENTS.

            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out-parameter-modifier
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ref

            exteriorDoorO.AssignDimensions();
            exteriorDoorO.PlaceRefObjectEntrance(emptyFullArray, areaArray, out ObjBox, out marginBox, out placementFailed);


            //STACK THE VECTOR AND BASE POINTS OUTPUTS IN SEPERATE LISTS. IT IS NOTHING TO BE REPRESENTED BUT USEFULL FOR DEBUGGING AND UNDERSTANDING THE CODE.
            ObjectVectors.Add(exteriorDoorO.VectorObject);
            MarginVectors.Add(exteriorDoorO.VectorMargin);

            
            BasePoints.Add(new Point3d(exteriorDoorO.BaseVector));

            // STACK OBJECT AND MARHIN BOXES.
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
                RotationOpt = new List<double>() { 0, 90, 180, 270 },

                MirrorBool = false,
                MirrorOpt = new List<int>() { 0, 2, 3 },

                Source = "NULL",
                FixedToWall = 1,
                CellSize = 60,

                PreviousObject = exteriorDoorO,
                SpaceList = entranceList,

            };


            changeOverZoneO.AssignDimensions();
            changeOverZoneO.PlaceObject(emptyFullArray, areaArray, out ObjBox, out marginBox, out placementFailed);
            // OUT PARAM HAS TO RETURN SOME VALUES SO WORKING OF .PLACEOBJECT DOES NOT MEAN WE GOT ACCEPTABLE SOLUTION
            // WE NEED TO CHECK IF WE GOT WE EXPECTED SO FUCTION ALSO RETURNS A BOOL SHOWS IF THE PLACEMENTS IS VALID
            // IN IF CONDITION WE ARE DECIDING TO CONTINUE OR BREAK THE UNSUCCESSFULL LOOP!

            // continue;  BREAKS THE LOOP ARE SKIP TO NEXT ITERATION. SO WE DON'T LOOSE TIME OR COMPUTATION POWER.
            // WE DO THIS CONTROL AFTER EACH OBJECT IS PLACED.
            
            if (placementFailed == true)
            {
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("PLACEMENT FAILED RESTARTED");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");
                objectReplacementCounter++;
                continue;
            }
            else

            {
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("PLACEMENT SATISFIED");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");
            }
            ObjectsList.Add(ObjBox);
            MarginsList.Add(marginBox);

            ObjectVectors.Add(changeOverZoneO.VectorObject);
            MarginVectors.Add(changeOverZoneO.VectorMargin);

            BasePoints.Add(new Point3d(changeOverZoneO.BaseVector));

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
                RotationOpt = new List<double>() { 0, 90, 180, 270 },

                MirrorBool = false,
                MirrorOpt = new List<int>() { 0, 2, 3 },

                Source = "NULL",
                FixedToWall = 1,
                CellSize = 60,

                PreviousObject = changeOverZoneO,
                SpaceList = entranceList,
            };

            closetO.AssignDimensions();
            closetO.PlaceObject(emptyFullArray, areaArray, out ObjBox, out marginBox, out placementFailed);

            if (placementFailed == true)
            {
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("PLACEMENT FAILED RESTARTED");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");
                objectReplacementCounter++;
                continue;
            }
            else

            {
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("PLACEMENT OKAY ");
                Rhino.RhinoApp.WriteLine("");
                Rhino.RhinoApp.WriteLine("");

                ;
            }
            ObjectVectors.Add(closetO.VectorObject);
            MarginVectors.Add(closetO.VectorMargin);

            BasePoints.Add(new Point3d(closetO.BaseVector));

            ObjectsList.Add(ObjBox);
            MarginsList.Add(marginBox);

            // THIS IS THE END POINTS IF PLACEMENT IS ENDED END THE LOOP WITH BOOL! 
            objectReplacementCounter++;
            objectPlacementEnded = true;
        }

        // SINCE EVERTHING WORKED FINE UNTILL HERE WE ARE READY TO SHOW THE OUTPUTS.
        // SHOW MATRIX PRINT AN ARRAY AS INTEGERS TO RHINO CONSOLE

        ShowMatrix(areaArray);

        // WE ARE ASSIGNING EACH LIST AND VARIABLE TO SEE THEM AS OUTPUT IN GRASSHOPPER
        A = areaArray;
        EntranceBoxes = entranceBoxes;
        CommonBoxes = commonBoxes;
        WetAreaBoxes = wetAreaBoxes;
        /*Y = kitchenBoxes;
        K = commonAreaBoxes;*/
        W = WhiteBoxes;

        MV = MarginVectors;
        OV = ObjectVectors;
        Base_Points = BasePoints;

        Furnitures = ObjectsList;
        FurnituresMargins = MarginsList;
    }

        static void ShowMatrix(int[,,] Arr)
    {
        /* SHOW 2D ARRAY
         IT CAN BE USED FOR EMPTY/FULL/MARGIN 
         OR ZONE LAYOUT */
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
        // CREATE AN ARRAY IN GIVEN DIMENSIONS
        var areaArray = new int[width, length, levelNumber];
        /* PASS THROUGH EAC ELEMENT OF THE ARRAY AND  ASSING THEM 0*/
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

    /*         Initialize zone list to stack cell informations    */
    static List<int> entranceList = new List<int>();
    static List<int> commonList = new List<int>();
    static List<int> wetAreaList = new List<int>();

    /**           these counters are used for starting the límitations counts.         **/
    static int entranceCounter = 1;
    static int commonCounter = 1;
    static int wetAreaCounter = 1;

    /**           these limits are used for defining max distance that zone can spread. Unit is cm.  **/
    static readonly int entranceLimit = 400;
    static readonly int commonLimit = 300;
    static readonly int wetAreaLimit = 300;

    static void StartZones(int[,,] arr)
    {
        /*GetLenght0  == width of an array, getLength == lenght of an array. It is dynamic because we can change area anytime we want. */
        Rhino.RhinoApp.WriteLine("Zone creation started");
        Random rand = new Random();
        // get a zone in the middle of X=0 or X=1 or X=2 line. 
        // it is resticred because we want to place entrance Zone and entrance door close to X=0.
        var a = rand.Next(arr.GetLength(0) / 2, arr.GetLength(0) / 2);
        var b = rand.Next(0, 2);
        entranceList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 1;

        //Add.Range adds multiple elements to a list. Instead of creating multiple sublists we are using flatted data structure. We are stacking them in one long list. Faster!
        // x1,y1,x2,y2,x3,y3.... all in one list 

        // Randomly assign tho any cell. 
        b = rand.Next(0, arr.GetLength(1));
        a = rand.Next(0, arr.GetLength(0));
        commonList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 2;

        // Randomly assign tho any cell. 
        a = rand.Next(0, arr.GetLength(0));
        b = rand.Next(0, arr.GetLength(1));
        wetAreaList.AddRange(new int[2] { a, b });
        arr[a, b, 0] = 3; 


    }

    static double DistanceCalculate(List<int> spaceCoordList, int x2, int y2, int cellSize)
    {
        // Euclidian distance calculator.
        var distance = cellSize * Math.Sqrt((y2 - spaceCoordList[1]) * (y2 - spaceCoordList[1]) + (x2 - spaceCoordList[0]) * (x2 - spaceCoordList[0]));
        return distance;
    }

    static void NoiseFilter(int[,,] arr, int type, int iterationNumber, int controlNumber)
    {
        /*
         *https://www.researchgate.net/figure/Neighboring-type-in-cellular-automata-A-von-Neumann-configuration-B-Moor_fig3_326665499
         * We are using Von neumann and Moore controls to filter the borders. (cellular automata.)
         * Check the link for further informations.
         *
         */
        if (type == 0)    //check 8 neighbour cells.
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
                            int count = (from z in surrounding where z == v select z).Count(); //USED LINQ methods for getting the number of the each surrounding cell number.
                            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/introduction-to-linq-queries
                            if (count >= controlNumber)
                            {
                                arr[i, j, 0] = v; // if one of the Zone has elements => number elements than control number argument transform current cell to it.
                            }
                        }
                        surrounding.Clear();
                        surrounding.TrimExcess();
                    }
                }
            }
        }

        if (type == 1)  //check 4 neighbour cells.
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
                                // if 2 neigbours has same zone value transform itself to this! 
                                // controlNumber CAN BE ASSIGNED INSTEAD OF 2 here . we can give it as arguments as well.
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

    public static void Fisher_Yates_CardDeck_Shuffle(List<int> aList)
    {
        // Method to shuffle List elements.
        System.Random _random = new System.Random();

        int myGO;

        int n = aList.Count;
        for (int i = 0; i < n; i++)
        {
            // NextDouble returns a random number between 0 and 1.
            // ... It is equivalent to Math.random() in Java.
            int r = i + (int)(_random.NextDouble() * (n - i));
            myGO = aList[r];
            aList[r] = aList[i];
            aList[i] = myGO;
        }
        // return aList;
    }


    public static void Fisher_Yates_CardDeck_Shuffle(List<double> aList)
    {
        // Method to shuffle List elements.
        System.Random _random = new System.Random();

        double myGO;

        int n = aList.Count;
        for (int i = 0; i < n; i++)
        {
            // NextDouble returns a random number between 0 and 1.
            // ... It is equivalent to Math.random() in Java.
            int r = i + (int)(_random.NextDouble() * (n - i));
            myGO = aList[r];
            aList[r] = aList[i];
            aList[i] = myGO;
        }
        // return aList;
    }
    static void Spread(int[,,] arr, ref double spaceSize, List<int> spaceCoordList, ref int counter, int spaceLimit, int cellSize)
    {
        // SPREAD ZONE CELLS BY USING THE INITIAL CELLS IN THE ZONE LISTS.
        var rand = new Random();
        var directionList = new List<int>();
        int Xcoord;
        int Ycoord;
        bool draw = false;
        int trycount = 0;

        /*  
         *  
         *  
         *  THIS inDirection BOOLS ARE VITAL!! DON'T DELETE THEM.
         *  THEY CHECKS IF THE GIVEN CONTROLS ARE IN THE ARRAY LIMITS OTHERWISE PROGRAM CRASHES!
         *  FOR NOT GETTING OUTOFBOUND EXCEPTION BE CAREFUL ABOUT CHECKING ARRAY BOUNDRIES BEFORE USING THERE ARRAYS!
         *  
         *
         */

        bool inTop;
        bool inRight;
        bool inBottom;
        bool inLeft;

        bool enoughTries = false;

        while (draw == false && counter < spaceSize && enoughTries == false)
        {
            /*
               TO RUN THE WHILE LOOP WE NEED 

               1- DRAW CONTITIONS IS FALSE. iF IT IS SUCCESSFULL END THE LOOP            
               2- SPACESIZE CONTITIONS IS FALSE. IN THIS CASE IT IS CELL NUMBER 
               3- IF THERE IS NO POSSIBLE SOLUTION AFTE ENOUGH TRIES WE STOP THE LOOP. SO WE SHOULD BE IN VALID NUMBER OF TRIES.
             */


            // THIS RANDOM VALUE LEADS US  A X VALUES IN COORDINATE LIST. IF WE DIDNT DIVIDE IT BY 2 IT MIGHT RETURN US Y VALUES TOO BUT IT WILL HARDER TO CONTROL THEM. 
            // NOW WE ARE SURE THAT IF WE MULTIPLE THE VALUE BY TWO BY 2 WE AREA GONAN GET X COORDINATE AT GIVEN INDEX.

            // SO CHOOSE ANY RANDOM CELL IN THE GIVEN ZONE.
            var randomCellIndex = rand.Next(spaceCoordList.Count / 2);

            // AS WE ARE USING FLATTENED DATA WE NEED TO MULTIPLY INDEX BY 2    TO GET X VALUES IN GIVEN INDEX
            // AS WE ARE USING FLATTENED DATA WE NEED TO MULTIPLY INDEX BY 2 +1 TO GET Y VALUES IN GIVEN INDEX
            Xcoord = spaceCoordList[2 * randomCellIndex];
            Ycoord = spaceCoordList[2 * randomCellIndex + 1];

            // THIS BOOLS ARE TO BE SURE THAT WE ARE IN THE ARERA LIMITS.

            inTop = (Ycoord + 1 < arr.GetLength(1));              // Check top border
            inRight = (Xcoord + 1 < arr.GetLength(0));              // Check Right border
            inBottom = (Ycoord - 1 >= 0);              // Check bottom border
            inLeft = (Xcoord - 1 >= 0);             //  Check left border

            if (inRight)
            {
                /*
                 * 1- CHECK IF THE POSSIBLE DIRECTION IS EMPTY. 
                 * 2- CHECK IF THE POSSIBLE DIRECTION IS IN CIRCULAR DISTANCE LIMIT FROM THE STARTING POINT. 
                 */
                if (arr[Xcoord + 1, Ycoord, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord + 1, Ycoord, cellSize) < spaceLimit) // Check if possible new cell is still white or not!
                    directionList.AddRange(new int[2] { (Xcoord + 1), Ycoord }); //Store possible spread directions in a list.
            }

            if (inLeft)
            {
                /*
                 * 1- CHECK IF THE POSSIBLE DIRECTION IS EMPTY. 
                 * 2- CHECK IF THE POSSIBLE DIRECTION IS IN CIRCULAR DISTANCE LIMIT FROM THE STARTING POINT. 
                 */
                if (arr[Xcoord - 1, Ycoord, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord - 1, Ycoord, cellSize) < spaceLimit)
                    directionList.AddRange(new int[2] { (Xcoord - 1), Ycoord }); //Store possible spread directions in a list.
            }

            if (inTop)
            {
                /*
                 * 1- CHECK IF THE POSSIBLE DIRECTION IS EMPTY. 
                 * 2- CHECK IF THE POSSIBLE DIRECTION IS IN CIRCULAR DISTANCE LIMIT FROM THE STARTING POINT. 
                 */
                if (arr[Xcoord, Ycoord + 1, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord, Ycoord + 1, cellSize) < spaceLimit)
                    directionList.AddRange(new int[2] { (Xcoord), Ycoord + 1 }); //Store possible spread directions in a list.
            }

            if (inBottom)
            {
                /*
                 * 1- CHECK IF THE POSSIBLE DIRECTION IS EMPTY. 
                 * 2- CHECK IF THE POSSIBLE DIRECTION IS IN CIRCULAR DISTANCE LIMIT FROM THE STARTING POINT. 
                 */
                if (arr[Xcoord, Ycoord - 1, 0] == 0 && DistanceCalculate(spaceCoordList, Xcoord, Ycoord - 1, cellSize) < spaceLimit)
                    directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.
            }

            // NOW WE HAVE A DIRECTION LIST WITH POSSIBLE DIRECTION COORDINATES.
            // IN NEXT STEP IF WE HAVE MORE THAN 0 DIRECTIONS WE ARE GONNA CHOOSE THEM RANDOMLY.

            if (directionList.Count > 0)
            {
                var randomIndex = rand.Next(directionList.Count / 2);
                int a = directionList[2 * randomIndex];
                int b = directionList[2 * randomIndex + 1];

                // ASSSIGN THE VALUES TO AREA ARRAY
                arr[a, b, 0] = arr[Xcoord, Ycoord, 0];

                // ADD THE COORDINATES TO ZONE LIST.
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