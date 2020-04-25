// Digital Architectural Design Studio - Group 2
// Space Organization Algorithm - kingPenguin
// Version 17.1



int Xbol = 7; // Interior boundries of the module
int Ybol = 14; // Each cell has 50*50 cm dimensions

int redBorder = 68; // Intended number of the cells
int greenBorder = 10;
int cyanBorder = 20;
//int blueBorder = 6;
//int yellowBorder = 25;

int tolerance = 4; // Negligible amount of missing - uncolored - cells
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

objectClass masa;


int [][] matrix = new int[Xbol][Ybol]; 

void setup()
{
  frameRate(5000);
  size(400, 800);
  background(255);

  red = new growCell("Living Space", 1, redSize, redBorder, 255, 0, 0);
  green = new growCell("Wet Area", 2, greenSize, greenBorder, 0, 255, 0);
  //blue = new growCell("Wet Area", 3, blueSize, blueBorder, 0, 0, 255);
  //yellow = new growCell("Kitchen", 4, yellowSize, yellowBorder, 255, 255, 0);
  cyan = new growCell("Entrance", 5, cyanSize, cyanBorder, 0, 255, 255);




  masa = new objectClass("Living Space", 101, 1, 2, 1, 0, 0, 0, 0, 0, 0, 0, 100, 100, 100);

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

  matrix[3][1] = 5; // Only the first cyan - entrance - cell is not random since the entrance is fixed.
  if (matrix[3][1]== 5)
  {
    fill(0, 255, 255, 50);
    noStroke();
    rect((width/Xbol)*3, (height/Ybol), (width/Xbol), (height/Ybol));
  }

  red.firstCell();
  //blue.firstCell();
  green.firstCell();
  //yellow.firstCell();
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
