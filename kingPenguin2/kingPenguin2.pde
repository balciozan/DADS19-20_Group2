  
// Digital Architectural Design Studio - Group 2
// Space Organization Algorithm - kingPenguin
// Version 2.0



int Xbol = 6; // Interior boundries of the module
int Ybol = 10; // Each cell has 60*60 cm dimensions


int redBorder = 40; // Intended number of the cells
int greenBorder = 6;
int cyanBorder = 14;
//int blueBorder = 6; No longer needed
//int yellowBorder = 25; No longer needed

int tolerance = 0; // Negligible amount of missing - uncolored - cells
int cycleTime = 300;
int loopCounter = 0;


int redSize = 0; // Momentarily number of the cells
int greenSize = 0;
int cyanSize = 0;
//int blueSize = 0;
//int yellowSize = 0;

int objectTrigger = 0;


growCell red;
growCell green;
growCell cyan;
//growCell blue;
//growCell yellow; 

objectClass table;
objectClass chair;
objectClass kitchenUnit;
objectClass bed;
objectClass monitor;
objectClass toilet;
objectClass showerCabin;

int [][] matrix = new int[Xbol][Ybol]; 

void setup()
{
  
  frameRate(500);
  size(400, 800);
  background(255);
  
  red = new growCell("Living Space", 1, redSize, redBorder - 1, 247, 207, 206);
  green = new growCell("Wet Area", 2, greenSize, greenBorder - 1, 215, 253, 209);
  //blue = new growCell("Wet Area", 3, blueSize, blueBorder - 1, 0, 0, 255); No longer needed
  //yellow = new growCell("Kitchen", 4, yellowSize, yellowBorder - 1, 255, 255, 0); No longer needed
  cyan = new growCell("Entrance", 5, cyanSize, cyanBorder - 1, 215, 253, 254);


  //objectClass(String oName, int oValue, int oZoneValue, int oWidth, int oDepth, int oHeight, int oMarLeft, int oMarRight, int oMarFront, int oMarRear, int oFixed, int oSource, int oR, int oG, int oB)
  table = new objectClass("table", 101, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 100, 100, 100);
  chair = new objectClass("chair", 111, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 90, 200, 90);
  kitchenUnit = new objectClass("kitchen unit", 121, 1, 3, 1, 0, 0, 0, 0, 0, 0, 0, 90, 0, 90);
  bed = new objectClass("bed", 131, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 90, 100, 250);
  monitor = new objectClass("monitor", 141, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 100, 90);
 toilet = new objectClass("toilet", 221, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 40, 10, 120);
  showerCabin = new objectClass("shower cabin", 201, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 30, 60, 90);
  
  redSize = 0;
  greenSize = 0;
  cyanSize = 0;
  //blueSize = 0;
  //yellowSize = 0;
  loopCounter = 0;
  objectTrigger = 0;



  for (int j=0; j< Ybol; j++)
  {
    for (int i=0; i< Xbol; i++)
    {
      matrix [i][j] = 0;
    }
  }

  /*
Below, one cell for each color - which represents different spaces - are created 
   randomly in order to generate alternative organizations. Each color - space - different
   int value to make further calculations.
   */

  matrix[3][0] = 5; // Only the first cyan - entrance - cell is not random since the entrance is fixed.
  if (matrix[3][0]== 5)
  {
    fill(215, 253, 254);
    noStroke();
    rect((width/Xbol)*3, (height/Ybol)*0, (width/Xbol), (height/Ybol));
  }

  red.firstCell();
  //blue.firstCell(); No longer needed
  green.firstCell();
  //yellow.firstCell(); No longer needed
}

void draw()
{
  grow();
  objects();
}


void keyPressed() // Resets the code.
{
  if ((keyCode == 'r') || (keyCode == 'R'))
  {
    setup();
  }
}
