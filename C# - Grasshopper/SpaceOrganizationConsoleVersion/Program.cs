using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;

using Grasshopper;

namespace SpaceOrganizationConsoleVersion
{


    class Program
    {
        public int cellSize = 60;

        static void Main(string[] args)
        {
            var commonArea = new Zone
            {
                SpaceLimit = 20,
                SpaceSize = 300,
                Counter = 1,
                Id = 3
            };

            var changeOverZoneO = new Objects
            {
                Name = "changeOverZone",
                ZoneName = commonArea,

                //Obj =Box,
                //ObjMargin = Box,

                Front = false,
                Right = false,
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

                //PreviousObject = Obj1;

            };
        }
    }

    public class Zone
    {
        public int SpaceLimit;
        public int SpaceSize;
        public int Counter;
        public int Id;
        public List<int> SpaceList;
        public List<Circle> Circles = new List<Circle>();

        public Zone() // Constructor method
        {
            SpaceList = new List<int>();
            Circles = new List<Circle>();
        }

        public Zone(int spaceLimit, int spacesize, int counter, int id)
          : this()
        {
            this.SpaceLimit = spaceLimit;
            this.Counter = counter;
            this.SpaceSize = spacesize;
            this.Id = id;
        }

        static void ShowMatrix(int[,,] Arr)
        {
            ////Print("");
            for (int i = 0; i < Arr.GetLength(0); i++)
            {
                for (int j = 0; j < Arr.GetLength(1); j++)
                {
                    ////Print(Arr[i, j].ToString() + " ");
                }
                ////Print("");
            }
            ////Print("   ");
            return;
        }

        static double distanceCalculate(List<int> spaceCoordList, int x2, int y2)
        {
            var distance = Math.Sqrt((y2 - spaceCoordList[1]) * (y2 - spaceCoordList[1]) + (x2 - spaceCoordList[0]) * (x2 - spaceCoordList[0]));
            return distance;
        }

        public void StartColors(int[,,] arr)
        {

            /**
             * Randomly Starts the color. Takes the Color lists as argumnts
             * (We can delete these argumnents and it can get them as local referance
             * in our case it gives us a chance to choose what to initilize)
             * **/

            Random rand = new Random();
            //----------------------
            /* GetLength(0) and GetLength(1) might be set as variable */
            var a = rand.Next(0, arr.GetLength(0)); //Array.GetLength(0) gives the column lenght in other words length of an array
            var b = rand.Next(0, arr.GetLength(1)); //Array.GetLength(1) gives the row lenght in other words width of an array

            this.SpaceList.AddRange(new int[2] { a, b }); //AddRange ==> Adds multiple elements to a List . We use mod%2 to call the index of the point back instead of stacking them as an another List(or Array) in our list.
            arr[a, b, 0] = this.Id;//  Starting Point
        }

        public void SpreadZone(int[,,] arr, int spaceSize, List<int> spaceCoordList, ref int counter, int spaceLimit)
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
                    ////Print("Counter Number is : {0}\n", counter);
                    ////Print("Element number of the list {0}", spaceCoordList.Count / 2);
                    var randomCellIndex = rand.Next(spaceCoordList.Count / 2);

                    Xcoord = spaceCoordList[2 * randomCellIndex];
                    Ycoord = spaceCoordList[2 * randomCellIndex + 1];

                    //Print("Coordinates if the new Cell is = {0}, {1}\n", Xcoord, Ycoord);

                    if (Xcoord < arr.GetLength(0) - 1 && Xcoord > 0 && Ycoord > 0 && Ycoord < arr.GetLength(1) - 1)
                    {
                        /* Check the conditions INSIDE borders */
                        if (arr[Xcoord + 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord + 1, Ycoord) < spaceLimit) // Check if possible new cell is still white or not!
                            directionList.AddRange(new int[2] { (Xcoord + 1), Ycoord }); //Store possible spread directions in a list.

                        if (arr[Xcoord - 1, Ycoord, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord - 1, Ycoord) < spaceLimit)
                            directionList.AddRange(new int[2] { (Xcoord - 1), Ycoord }); //Store possible spread directions in a list.

                        if (arr[Xcoord, Ycoord + 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord + 1) < spaceLimit)
                            directionList.AddRange(new int[2] { (Xcoord), Ycoord + 1 }); //Store possible spread directions in a list.

                        if (arr[Xcoord, Ycoord - 1, 0] == 0 && distanceCalculate(spaceCoordList, Xcoord, Ycoord - 1) < spaceLimit)
                            directionList.AddRange(new int[2] { (Xcoord), (Ycoord - 1) }); //Store possible spread directions in a list.

                        if (directionList.Count > 0)
                        {
                            var randomIndex = rand.Next(directionList.Count / 2);

                            int a = directionList[2 * randomIndex];
                            int b = directionList[2 * randomIndex + 1];
                            //print("Value of Xcoord and Ycoord: {0}, {1}", Xcoord, Ycoord);
                            //Print("Value of new Coordinates: {0}, {1}", a, b);
                            //Print("Value of initial value of Array[a, b]: {0}", arr[a, b]);

                            arr[a, b, 0] = arr[Xcoord, Ycoord, 0];
                            //Print("Value of new Array[a, b]: {0}", arr[a, b]);

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
                    //BOTTOM
                    else if (Xcoord == (arr.GetLength(0) - 1) && Ycoord != 0 && Ycoord != (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord - 1, Ycoord, 0] == 0)
                        {
                            arr[Xcoord - 1, Ycoord, 0] = arr[Xcoord, Ycoord, 0];

                            int a = Xcoord - 1;
                            int b = Ycoord;

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                            //Print(" Counter Number is : {0}", counter);
                        }
                    }
                    //TOP
                    else if (Xcoord == 0 && Ycoord != 0 && Ycoord != (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord + 1, Ycoord, 0] == 0)
                        {
                            arr[Xcoord + 1, Ycoord, 0] = arr[Xcoord, Ycoord, 0];

                            int a = Xcoord + 1;
                            int b = Ycoord;

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                            //Print(" Counter Number is : {0}", counter);
                        }
                    }
                    //RIGHT
                    else if ((Ycoord == arr.GetLength(1) - 1) && Xcoord != 0 && Xcoord != (arr.GetLength(0) - 1))
                    {
                        if (arr[Xcoord, Ycoord - 1, 0] == 0)
                        {
                            arr[Xcoord, Ycoord - 1, 0] = arr[Xcoord, Ycoord, 0];

                            int a = Xcoord;
                            int b = Ycoord - 1;

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                            //Print(" Counter Number is : {0}", counter);
                        }
                    }
                    //LEFT
                    else if (Ycoord == 0 && Xcoord != 0 && Xcoord != (arr.GetLength(0) - 1))
                    {
                        if (arr[Xcoord, Ycoord + 1, 0] == 0)
                        {
                            arr[Xcoord, Ycoord + 1, 0] = arr[Xcoord, Ycoord, 0];

                            int a = Xcoord;
                            int b = Ycoord + 1;

                            arr[Xcoord, Ycoord + 1, 0] = arr[Xcoord, Ycoord, 0];

                            spaceCoordList.AddRange(new int[2] { a, b });

                            draw = true;
                            counter++;
                            //Print(" Counter Number is : {0}", counter);
                        }
                    }

                    //LEFT TOP
                    else if (Xcoord == 0 && Ycoord == 0)
                    {
                        if (arr[Xcoord, Ycoord + 1, 0] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord + 1);

                        if (arr[Xcoord + 1, Ycoord, 0] == 0)
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
                    }
                    //RIGHT TOP
                    else if (Xcoord == 0 && Ycoord == (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord + 1, Ycoord, 0] == 0)
                            directionList.Add(Xcoord + 1); directionList.Add(Ycoord);

                        if (arr[Xcoord, Ycoord - 1, 0] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

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
                    }
                    //RIGHT BOTTOM
                    else if (Xcoord == (arr.GetLength(0) - 1) && Ycoord == (arr.GetLength(1) - 1))
                    {
                        if (arr[Xcoord, Ycoord - 1, 0] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

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
                    }
                    //LEFT BOTTOM
                    else if (Xcoord == (arr.GetLength(1) - 1) && Ycoord == 0)
                    {
                        if (arr[Xcoord, Ycoord + 1, 0] == 0)
                            directionList.Add(Xcoord); directionList.Add(Ycoord - 1);

                        if (arr[Xcoord - 1, Ycoord, 0] == 0)
                            directionList.Add(Xcoord - 1 - 1); directionList.Add(Ycoord);

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
                    }
                    /* EDGE CONDITIONS */
                }
            }
        }
    }


    class Objects
    {
        public string Name;
        public Zone ZoneName;

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

        public Objects(string name, Zone zoneName, bool front, bool right, bool back, bool left, bool top, bool bottom, bool rotationalBool, int rotationOpt, bool mirrorBool, int mirrorOpt, string source, int fixedTowall, int cellSize,Box obj,Box objMargin, Objects previousObject)
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
            baseY= Obj.Plane.OriginY;
            baseZ = Obj.Plane.OriginZ;
            

            this.PreviousObject = previousObject;
        }

        public Objects(string name, Zone zoneName, bool front, bool right, bool back, bool left, bool top, bool bottom, bool rotationalBool, int rotationOpt, bool mirrorBool, int mirrorOpt, string source, int fixedTowall,int cellSize, Box obj, Box objMargin)
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
            this.CellSize=cellSize;


            ObjWidth = (int)(Obj.X.Length);
            ObjLength = (int)(Obj.Y.Length);
            ObjHeight = (int)(Obj.Z.Length);

            MrjWidth = (int)(ObjMargin.X.Length);
            MrjLength = (int)(ObjMargin.Y.Length);
            MrjHeight = (int)(ObjMargin.Z.Length);

            baseX = Obj.Plane.OriginX;
            baseY = Obj.Plane.OriginY;
            baseZ = Obj.Plane.OriginZ;

        }

        public void placeFront()
        {
            this.baseX = PreviousObject.baseX + PreviousObject.MrjLength  ;
            this.baseY = PreviousObject.baseY;
            this.baseZ = PreviousObject.baseZ;
        }

        public void placeRight()
        {
            this.baseX = PreviousObject.baseX ;
            this.baseY = PreviousObject.baseY + MrjWidth;
            this.baseZ = PreviousObject.baseZ;
        }

        public void placeBack()
        {
            //this.ObjMargin.Transform(Transform.Rotation(1 * Math.PI, new Point3d(this.baseX, this.baseY, this.baseZ)));
            //this.Obj.Transform(Transform.Rotation(1 * Math.PI, new Point3d(this.baseX, this.baseY, this.baseZ)));

            this.baseX = PreviousObject.baseX - PreviousObject.MrjLength;
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
            this.baseY = PreviousObject.baseY ;
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
            var randomCellIndex = rand.Next(ZoneName.SpaceList.Count / 2);

            baseX = ZoneName.SpaceList[2 * randomCellIndex];
            baseY = ZoneName.SpaceList[2 * randomCellIndex + 1];
            baseZ = 0;

            Point3d pointa = new Point3d(baseX * CellSize, baseY * CellSize, 0);
            Vector3d pointb = new Vector3d(0, 0, 1); 

            Plane planebase = new Plane(pointa, pointb);

            Interval xInterval = new Interval(baseX * CellSize, ObjWidth);
            Interval yInterval = new Interval(baseY * CellSize, ObjLength);
            Interval zInterval = new Interval(baseZ,  ObjHeight);

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

    class Cell
    {
        bool Full = false;


        public Cell()
        {
            
        }

        static void checkNeigbours()
        {

        }
    }
}
