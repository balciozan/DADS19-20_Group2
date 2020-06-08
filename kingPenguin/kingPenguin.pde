/*
   
 Digital Architectural Design Studio
 Group 2 -- Mustafa Cem GUNES - Izel Efay TAN - Nurcan SUTCU - Ozan BALCI - Pelin Gül ARMAN -- 
 Space Organization Algorithm - kingPenguin
 Version 2.1
 
 
 ...............................................................................
 
 ..................................+@WWWWWWW@=-.................................
 
 ................................#WWWWWWWWWWWWWW+...............................
 
 ..............................-WWWWWWWWWWWWWWWWW=..............................
 
 .............................-@W@WWWWWWWWWWWWW@@W*.............................
 
 .............................W:...*WWWWWWWW.....:@-............................
 
 .............................#@..-..:WWWWWW+..-..@+............................
 
 .............................#.....=W@--WW=.......=.............................
 
 ............................-+:+....=WW++WW#....:-*............................
 
 ............................::.-:.....--......+..*.............................
 
 ............................:....-:=====+:........:............................
 
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
 
 ................-+..+.....................................-+..=................
 
 ................-:.-@.....................................+#..*................
 
 ................-+.##-....................................=.*..................
 
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
 
 KingPenguin Algorithm is designed to create alternative space organizations for a mobile module 
 of Turkish Research Base in Antarctic. First aim was to create organization alternatives in 3D, 
 yet current code works on only 2D. 
 
 If we summerize the algorithm, there are 4 steps:
 1) Generating the zones;
 2) Filtering the borders of the zones;
 3) Placing objects to the zones;
 4) Saving the created alternative and re-running the function.
 
 Each step will be explained in its line.
 
 PS: Some functions are disabled for experimental purposes, please check each of them before start the code.
 
 
 ##########################################################################################
 ##                                        IMPORTANT                                     ##
 ##########################################################################################
 
 The algorithm is NOT completed. It works for certain conditions, yet here are missings parts and 
 several bugs. Next group which will work on this algorithm ,in Digital Architectural Design 
 Studio, may contact with the members of group 2 anytime. 
 
 */



/*

 ##########################################################################################
 ##                                        INPUTS                                        ##
 ##########################################################################################
 
 There are several inputs to configure the algorithm.
 
 Xbol = Width of the module
 Ybol = Depth of the module
 
 (colorName)Border = Area of the zone
 
 tolerance = Algorithm tries to fulfill the whole are with intended number of colored cells. Sometime
 it fails to create enough cells and unassigned cells occur. In this scenario, code evaluates itself as
 unsuccessful and re-runs the whole function. If you add tolerance, code will ignore specified number 
 of white cell and evaluates the current scenario as succesful.
 
 cycleTime = Determines how many time the algorithm tries to add new cell. Bigger number will create 
 more successful results, yet will take longer time. Smaller number will take shorter time yet create 
 less successful results. 300 is optimum number.
 
 */

int Xbol = 6;
int Ybol = 10;

int redBorder = 40;
int greenBorder = 6;
int cyanBorder = 14;
//int blueBorder = 6; No longer needed
//int yellowBorder = 25; No longer needed

int tolerance = 0;
int cycleTime = 300;




// =================== Creating Counters For Each Zone =================

// For each zone, there should be a counter (int) to count momentarily number of the cells
int redSize = 0; 
int greenSize = 0;
int cyanSize = 0;
//int blueSize = 0; No longer needed
//int yellowSize = 0; No longer needed


int objectTrigger = 0; // Integer which starts the object placing part of the algorithm when it is equal to 1.
int failCounter = 0; // Integer which counts the errors have been occured.
int loopCounter = 0;


// =========================== Creating Zones =========================

// If a new zone is needed, it should be created as below.
growCell red;
growCell green;
growCell cyan;
//growCell blue; No longer needed
//growCell yellow; No longer needed



// =========================== Creating Objects ========================

// If a new object is needed, it should be created as below.
objectClass table;
objectClass chair;
objectClass kitchenUnit;
objectClass bed;
objectClass monitor;
objectClass toilet;
objectClass showerCabin;



// Creation of the matrix which will be the template of the space.
int [][] matrix = new int[Xbol][Ybol]; 

void setup()
{
  frameRate(500);
  size(400, 800);
  background(255);

  // =========================== Defining The Zones =========================  

  // Each zone should defined by their 'NAME', 'SIZE COUNTER', 'ZONE VALUE', 'AREA' and 'COLOR R', 'COLOR G', 'COLOR B'
  // ZONE VALUE should be a random int between 0 - 100, each zone should have a different value.

  red = new growCell("Living Space", 1, redSize, redBorder - 1, 247, 207, 206);
  green = new growCell("Wet Area", 2, greenSize, greenBorder - 1, 215, 253, 209);
  //blue = new growCell("Wet Area", 3, blueSize, blueBorder - 1, 0, 0, 255); No longer needed
  //yellow = new growCell("Kitchen", 4, yellowSize, yellowBorder - 1, 255, 255, 0); No longer needed
  cyan = new growCell("Entrance", 5, cyanSize, cyanBorder - 1, 215, 253, 254);


  // =========================== Defining The Objects ========================

  // Each object should be defined by their 'NAME', 'VALUE', 'ZONE VALUE', 'OBJECT VALUE', 'WIDTH', 'DEPTH', 'HEIGHT',
  // 'LEFT MARGINE', 'RIGHT MARGINE', 'FRONT MARGINE', 'REAL MARGINE', 'FIX STATUS', 'SOURCE STATUS', 'COLOR R', 'COLOR G', 'COLOR B'
  // OBJECT VALUE should be a random int between 100 - 999, same type of objects belong to same hundred.

  table = new objectClass("table", 101, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 100, 100, 100);
  chair = new objectClass("chair", 111, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 90, 200, 90);
  kitchenUnit = new objectClass("kitchen unit", 121, 1, 1, 3, 0, 0, 0, 0, 0, 0, 0, 90, 0, 90);
  bed = new objectClass("bed", 131, 1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 90, 100, 0);
  monitor = new objectClass("monitor", 141, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 100, 90);
  toilet = new objectClass("toilet", 151, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 40, 10, 120);
  showerCabin = new objectClass("shower cabin", 161, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 30, 60, 90);



  // Reseting the counters  

  redSize = 0;
  greenSize = 0;
  cyanSize = 0;
  //blueSize = 0; No longer needed
  //yellowSize = 0; No longer needed
  loopCounter = 0;
  objectTrigger = 0;
  failCounter = 0;


  // =========================Creating The 2D Array ========================

  // Creating the 2D array by giving value 0 to each cell in order to remark them as empty cells.

  for (int j=0; j< Ybol; j++)
  {
    for (int i=0; i< Xbol; i++)
    {
      matrix [i][j] = 0;
    }
  }


  // ======================== Creating The First Cells =====================

  /* 
   In order to create different alternatives, zones grow randomly. First step of the growing is creation of
   first cells. (color).firstCell() function creates random first cells for each zone.
   
   But in this scenario, position of the entrance is fixed since there is one exterior door and it is static.
   Because of that, first cell of the entrance zone is created manually.
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

  // Function includes all the functions which are related with zones.  
  grow(); 

  // Function includes all the functions which are related with objects.  
  objects();
}


void keyPressed() // Resets the code.
{
  if ((keyCode == 'r') || (keyCode == 'R'))
  {
    setup();
  }
}
