using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOrganizationConsoleVersion
{
    class Program
    {
        static void Main(string[] args)
        {


            //deneme
        }
    }


    class Objects
    {

        public string Name;

        public bool Front=false;
        public bool Right=false;
        public bool Back = false;
        public bool Left = false;
        public bool Top = false;
        public bool Bottom = false;

        //public int x,y,z ; //location

        public bool RotationBool;
        public int rotation; // rotation options

        public string Source; // Water, Electiricity
        public int FixedToWall; // Bed, Television //1,2,3 conditions 
        public Objects previousObject;

        //deneme
        public Objects()
        {
            var Coordinates = new List<int>();
        }

        public Objects(bool Front)
         :this()
        {

        }

    }



}
