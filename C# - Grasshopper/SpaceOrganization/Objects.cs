using System;
using System.Collections.Generic;
using Rhino.Geometry;

public partial class Script_Instance
{
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

            this.VectorObject = new Vector3d(this.ObjWidth, this.ObjLength, this.ObjHeight);
            this.VectorMargin = new Vector3d(this.MrjWidth, this.MrjLength, this.MrjHeight);
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
            this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
            this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
            this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

            Rhino.RhinoApp.WriteLine(" MARGIN  numbers ----------------------------------" + XCellMargin.ToString() + ' ' + YCellMargin.ToString() + ' ' + ZCellMargin.ToString() + ' ');


            this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
            this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
            this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));


            Rhino.RhinoApp.WriteLine(" OBJECT  numbers ----------------------------------" + XCellObject.ToString() + ' ' + YCellObject.ToString() + ' ' + ZCellObject.ToString() + ' ');

            // We need to define the start end end points because we dont know if the inital poin is lower than end point


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
                        Rhino.RhinoApp.WriteLine("X    Loop numbers ----------------------------------" + XLoopStartMargin.ToString() + ' ' + XLoopEndMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine("Y    Loop numbers ----------------------------------" + YLoopStartMargin.ToString() + ' ' + YLoopEnMargin.ToString() + ' ');
                        Rhino.RhinoApp.WriteLine("Z    Loop numbers ----------------------------------" + ZLoopStartMargin.ToString() + ' ' + ZLoopEndMargin.ToString() + ' ');
                */
                bool inLimit = true;

                if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
                  (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
                {
                    inLimit = false;
                }

                if (inLimit)
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
                                    Rhino.RhinoApp.WriteLine(" Loop numbers ----------------------------------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
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
                        placeSuccessful = true;
                        Rhino.RhinoApp.WriteLine("Reference Object conditions are successful ");
                        //Place Margin
                        for (int i = XLoopStartMargin; i < XLoopEndMargin; i++)
                        {
                            for (int j = YLoopStartMargin; j < YLoopEnMargin; j++)
                            {
                                for (int k = ZLoopStartMargin; k < ZLoopEndMargin; k++)
                                {
                                    Rhino.RhinoApp.WriteLine(" Loop numbers ----------------------------------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
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
                                    Rhino.RhinoApp.WriteLine("Loop inetegers --------------- " + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
                                    emptyFullArray[i, j, k] = 1; // 1 represents object
                                }
                            }
                        }
                    }
                    else
                    {
                        Rhino.RhinoApp.WriteLine("Conditions are not satisfied ");
                    }
                }
            }
            trycount++;
            Rhino.RhinoApp.WriteLine("Try count is : " + trycount);
            if (trycount > 60)
            {
                Rhino.RhinoApp.WriteLine("Try count is over 60 placement is unsuccessful: ");
                enoughTries = true;
            }

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

        public void PlaceObject(int[,,] emptyFullArray, int[,,] areaArray, out Box ObjBox, out Box marginBox, out bool placementFailed)
        {
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
                    Rhino.RhinoApp.WriteLine("In grow  " + growDirection[i]);
                }
            }

            bool placeSuccess = false;
            int counter = 0;

            Random randm = new Random();
            Rhino.RhinoApp.WriteLine("Element Number of the grow Direction before while loop " + growDirection.Count.ToString());

            while (growDirection.Count > 0 && placeSuccess == false && counter < 6)
            {
                Rhino.RhinoApp.WriteLine("Counter number in the check direction loop " + counter.ToString());

                int a = randm.Next(0, growDirection.Count);

                int randomDirectionIndex = growDirection[a];

                Rhino.RhinoApp.WriteLine("Element Number of the grow Direction in while loop " + growDirection.Count.ToString());

                switch (randomDirectionIndex)
                {
                    case 1:
                        if (CheckFront(emptyFullArray, areaArray))
                        {
                            this.PlaceFront(emptyFullArray, out ObjBox, out marginBox);
                            placeSuccess = true;
                        }
                        else
                        {
                            growDirection.Remove(randomDirectionIndex);
                            Rhino.RhinoApp.WriteLine("Could not Placed to FRONT ");
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
                            growDirection.Remove(randomDirectionIndex);
                            Rhino.RhinoApp.WriteLine("Could not Placed to RIGHT ");
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
                            growDirection.Remove(randomDirectionIndex);
                            Rhino.RhinoApp.WriteLine("Could not Placed to BACK ");
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
                            growDirection.Remove(randomDirectionIndex);
                            Rhino.RhinoApp.WriteLine("Could not Placed to LEFT ");
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
                            growDirection.Remove(randomDirectionIndex);
                            Rhino.RhinoApp.WriteLine("Could not Placed to Top ");
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
                            growDirection.Remove(randomDirectionIndex);
                            Rhino.RhinoApp.WriteLine("Could not Placed to BOTTOM ");
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
            bool inLimits;
            bool emptyBool = true;
            bool sameZone = true;

            bool suitableDirection = true;
            bool initialDirectionCheck = true;

            this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
            this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
            this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

            this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
            this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
            this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

            Rhino.RhinoApp.WriteLine(" MARGIN  Front numbers ----------------------------------" + XCellMargin.ToString() + ' ' + YCellMargin.ToString() + ' ' + ZCellMargin.ToString() + ' ');
            Rhino.RhinoApp.WriteLine(" OBJECT  Front numbers ----------------------------------" + XCellObject.ToString() + ' ' + YCellObject.ToString() + ' ' + ZCellObject.ToString() + ' ');

            if (VectorMargin.X * VectorMargin.Y > 0)
            {
                this.baseX = PreviousObject.baseX;
                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
                this.baseZ = PreviousObject.baseZ;
            }


            else if (VectorMargin.X * VectorMargin.Y < 0)
            {
                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                this.baseY = PreviousObject.baseY;
                this.baseZ = PreviousObject.baseZ;
            }

            Rhino.RhinoApp.WriteLine("  baseY :      : " + baseY.ToString());

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
                initialDirectionCheck = true;
            }

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
              (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
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



            Rhino.RhinoApp.WriteLine("X  Front  Margin Loop numbers ----------------------------------" + XLoopStartMargin.ToString() + ' ' + XLoopEndMargin.ToString() + ' ');
            Rhino.RhinoApp.WriteLine("Y  Front Margin  Loop numbers ----------------------------------" + YLoopStartMargin.ToString() + ' ' + YLoopEnMargin.ToString() + ' ');
            Rhino.RhinoApp.WriteLine("Z  Front  Margin Loop numbers ----------------------------------" + ZLoopStartMargin.ToString() + ' ' + ZLoopEndMargin.ToString() + ' ');

            Rhino.RhinoApp.WriteLine("X  Front Object Loop numbers ----------------------------------" + XLoopStartObject.ToString() + ' ' + XLoopEndObject.ToString() + ' ');
            Rhino.RhinoApp.WriteLine("Y  Front Object  Loop numbers ----------------------------------" + YLoopStartObject.ToString() + ' ' + YLoopEnObject.ToString() + ' ');
            Rhino.RhinoApp.WriteLine("Z  Front Object  Loop numbers ----------------------------------" + ZLoopStartObject.ToString() + ' ' + ZLoopEndObject.ToString() + ' ');

            bool inLimit = true;

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
              (baseX + VectorMargin.X > 0) && (baseY + VectorMargin.Y > 0) && (baseZ + VectorMargin.Z > 0)))
            {
                inLimit = false;
            }
            /***/

            Rhino.RhinoApp.WriteLine("Bools " + suitableDirection.ToString() + "    " + inLimit.ToString() + ' ');


            if (inLimit && initialDirectionCheck)
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
                                Rhino.RhinoApp.WriteLine("Front Margin Array is full, Loop numbers ----------------------------------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
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
                    suitableDirection = true;
                    Rhino.RhinoApp.WriteLine("Place Front: Conditions are  satisfied ");
                }
                else
                {
                    Rhino.RhinoApp.WriteLine("Place Front: Conditions are not satisfied ");
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

        public bool CheckRight(int[,,] emptyFullArray, int[,,] areaArray)
        {
            bool emptyBool = true;
            bool sameZone = true;
            bool suitableDirection = true;
            bool inLimits = true;

            /**/
            this.XCellMargin = (this.VectorMargin.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.X / CellSize));
            this.YCellMargin = (this.VectorMargin.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Y / CellSize));
            this.ZCellMargin = (this.VectorMargin.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorMargin.Z / 150));

            this.XCellObject = (this.VectorObject.X < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.X / CellSize));
            this.YCellObject = (this.VectorObject.Y < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Y / CellSize));
            this.ZCellObject = (this.VectorObject.Z < 0 ? -1 : 1) * Math.Ceiling((double)Math.Abs(this.VectorObject.Z / 150));

            if (VectorMargin.X * VectorMargin.Y > 0)
            {
                this.baseX = PreviousObject.baseX + PreviousObject.XCellMargin * this.CellSize;
                this.baseY = PreviousObject.baseY;
                this.baseZ = PreviousObject.baseZ;
            }

            else if (VectorMargin.X * VectorMargin.Y < 0)
            {
                this.baseX = PreviousObject.baseX;
                this.baseY = PreviousObject.baseY + PreviousObject.YCellMargin * this.CellSize;
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

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
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
                                Rhino.RhinoApp.WriteLine(" Loop numbers ----------------------------------" + i.ToString() + ' ' + j.ToString() + ' ' + k.ToString() + ' ');
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

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
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

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
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

            if (!((baseX + this.VectorMargin.X < 360) && (baseY + this.VectorMargin.Y < 600) && (baseZ + this.VectorMargin.Z < 300) &&
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

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
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

            if (!((baseX + this.ObjWidth < 360) && (baseY + this.ObjLength < 600) && (baseZ + this.ObjHeight < 300) &&
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

            if (!((baseX + VectorMargin.X < 360) && (baseY + VectorMargin.Y < 600) && (baseZ + VectorMargin.Z < 300) &&
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