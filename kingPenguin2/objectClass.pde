class objectClass
{
  IntList listX;
  IntList listY;
  IntList listPos;
  String objectName;
  int objectValue;
  int objectZoneValue;
  int objectR;
  int objectG;
  int objectB;
  int objectWidth;
  int objectHeight;
  int objectDepth;
  int objectMarLeft;
  int objectMarRight;
  int objectMarFront;
  int objectMarRear;
  int objectSource;
  int objectFixed;

  //PImage img;
int refWidth;
int refDepth;


  objectClass(String oName, int oValue, int oZoneValue, int oWidth, int oDepth, int oHeight, int oMarLeft, int oMarRight, int oMarFront, int oMarRear, int oFixed, int oSource, int oR, int oG, int oB) {

    objectName = oName;
    objectValue = oValue;
    objectZoneValue = oZoneValue;
    objectWidth = oWidth;
    objectDepth = oDepth;
    objectHeight = oHeight;
    objectMarLeft = oMarLeft;
    objectMarRight = oMarRight;
    objectMarFront = oMarFront;
    objectMarRear = oMarRear;
    objectFixed = oFixed;
    objectSource = oSource;
    objectR = oR;
    objectG = oG;
    objectB = oB;
  }

  void referenceObject() { // Function for placing reference objects. Each zone has 1 reference object.
    listX = new IntList();
    listY = new IntList();
    listX.clear();
    listY.clear();

    for (int j=0; j< Ybol; j++)
    {
      for (int i=0; i< Xbol; i++)
      {
        if (matrix[i][j] == objectZoneValue) {
          listX.append(i);
          listY.append(j);
        }
      }
    }
    int success1 = 0;
    int whilecnt = 0;
    while (success1 == 0) {
      int rand = 0;
      int realX = 0;
      int realY = 0;

      whilecnt++;

      rand = int(random(0, listX.size())); 


      realX = listX.get(rand);
      realY = listY.get(rand);

      if (realX + objectWidth -1 <= listX.max() && realY + objectDepth -1 <= listY.max()) {
        for (int i = realX; i < realX + objectWidth; i++)
        {
          for (int j= realY; j< realY + objectDepth; j++)
          {
            int newi = i;
            int newj = j;
            matrix[newi][newj] = objectValue;
            fill(objectR, objectG, objectB);
            noStroke();
            rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
            //img = loadImage(objectName+".png");
            //image(img, (width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
          }
        }
        success1++;
      } else if (whilecnt == 1000) {
        success1++;
      }
    }
    
  }

  void placeObject(int refObject, int method, int dist, int longside, int rotate)
  {
    //rotate 0, 1, 2 = no rotate, rotate,random
    //longside 0,1,2= random, add from long edge, add from short edge
     // refObject is the reference for the object to be placed.
    // method can have 3 values for now. 0 = no relation / 1 = near to / 2 = next to.
    // if the method is equal to 1, dist will determine the range.
    
     //**************ROTATE**********************************
     for (int i=0; i<1; i++)
     {
    if (  rotate ==1 ) 
      //1 means 90 degree rotating
    { 
      int hiddenlayer=0;
      hiddenlayer=objectDepth;
      objectDepth= objectWidth;
      objectWidth = hiddenlayer;
       println("90 degree rotated");//kontrol
    } 
    else if (  rotate ==0 ) {
      //0 means no rotating at all
      println("no rotate");
    }
   else if (  rotate ==2 ) 
    {
      //2 means rotating option will be choosing randomly
      int select =int(random(0, 1));
      println("randomly rotated");//kontrol
      if (select ==1 )
      { 
        //1 means rotating 90 degrees
        int hiddenlayer=0;
        hiddenlayer=objectDepth;
        objectDepth= objectWidth;
        objectWidth = hiddenlayer;  
        println("90 degree rotated");//kontrol
      }
      else 
        println("no rotate");
    }
     }
    
    listX = new IntList();
    listY = new IntList();
    listX.clear();
    listY.clear();
    dist = 0;

    for (int j=0; j< Ybol; j++)
    {
      for (int i=0; i< Xbol; i++)
      {
        if (matrix[i][j] == refObject) {
          listX.append(i);
          listY.append(j);
        }
      }
    }
   //  println("1fromref "+ listX);//kontrol
           println("refObject "+refObject);//kontrol
           
           if (refObject== 121)
    {
      refWidth=1;
      refDepth=3;
    }

   
   


    int success1 = 0;
    int whilecnt = 0;
    while (success1 == 0) {
      int rand = 0;
      int realX = 0;
      int realY = 0;

      whilecnt++;
      println("2+"+listX);//kontrol
      println("refObject"+refObject);//kontrol
      rand = int(random(0, listX.size())); 

      realX = listX.get(rand);
      realY = listY.get(rand);

      int success2 = 0;
      int whilecnt2 = 0;
      while (success2 == 0 ) {
        int randOpt = 0;
        int objectOpt = 0;
        listPos = new IntList();
        listPos.clear();

        whilecnt2++;
                println("REFDEPTH********"+refDepth);//kontrol
     //controlling if the short edge placed in y axis
        // longside =1 means place next object from long side
        if (longside==1 && refDepth > refWidth )
        {
        longside=2;
        
        }
        //controlling if the short edge placed in y axis
        // longside =2 means place next object from short side
        if (longside==2 && refDepth > refWidth )
        {
        longside=1;
        }
       // referans objenin ölçüsünü almalı
        
        if (longside == 0 )
         
        //default option where side of the placement does not matter
        {
           println("longside" +longside);
          if (realX - objectWidth < 0 && realY - objectDepth < 0) {  // Left upper corner -- add only to right and down
          listPos.append(2);
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY - objectDepth < 0 && realX - objectWidth >= 0 && realX + objectWidth < Xbol) {  // Upper boundry -- add only to right, down and left
          listPos.append(2);
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth >= Xbol && realY - objectDepth < 0) {  // Right upper corner -- add only to down and left
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth >= Xbol && realY + objectDepth < Ybol && realY - objectDepth >= 0) {  // Right boundry -- add only to down, left and up
          listPos.append(1);
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth >= Xbol && realY + objectDepth >= Ybol) {  // Right down corner -- add only to left and up
          listPos.append(1);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY + objectDepth >= Ybol && realX - objectWidth >= 0 && realX + objectWidth < Xbol) {  // Down boundry -- add only to left, up and right
          listPos.append(1);
          listPos.append(2);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth < 0 && realY + objectDepth >= Ybol) {  // Left down corner -- add only to up and right
          listPos.append(1);
          listPos.append(2);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth < 0 && realY + objectDepth < Ybol && realY - objectDepth >= 0) {  // Left boundry -- add only to up, right and down
          listPos.append(1);
          listPos.append(2);
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else {
          listPos.append(1);
          listPos.append(2);
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        }
        }
       else if (longside == 2  )
       
        //place short edge
        {
             println("longside" + longside);
             
          if (realX - objectWidth < 0 && realY - objectDepth < 0) {  // Left upper corner -- add only to right and down
            listPos.append(2);

            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if  (realY - objectDepth < 0 && realX - objectWidth >= 0 && realX + objectWidth < Xbol) {  // Upper boundry -- add only to right, down and left
            listPos.append(2);

            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if  (realX + objectWidth >= Xbol && realY - objectDepth < 0) {  // Right upper corner -- add only to down and left

            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if (realX + objectWidth >= Xbol && realY + objectDepth < Ybol && realY - objectDepth >= 0) {  // Right boundry -- add only to down, left and up


            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if (realX + objectWidth >= Xbol && realY + objectDepth >= Ybol) {  // Right down corner -- add only to left and up

            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if  (realY + objectDepth >= Ybol && realX - objectWidth >= 0 && realX + objectWidth < Xbol) {  // Down boundry -- add only to left, up and right

            listPos.append(2);
            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if (realX - objectWidth < 0 && realY + objectDepth >= Ybol){  // Left down corner -- add only to up and right

            listPos.append(2);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if (realX - objectWidth < 0 && realY + objectDepth < Ybol && realY - objectDepth >= 0) {  // Left boundry -- add only to up, right and down

            listPos.append(2);

            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else {

            listPos.append(2);

            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          }
          println("***********************************" +listPos);
        }
       else if (longside ==1 )
       
        //that controls if new object connects to the referenced object from longside
        {
             println("longside "+longside);
          if (realX - objectWidth < 0 && realY - objectDepth < 0) {  // Left upper corner -- add only to right and down

            listPos.append(3);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if  (realY - objectDepth < 0 && realX - objectWidth >= 0 && realX + objectWidth < Xbol) {  // Upper boundry -- add only to right, down and left
            listPos.append(3);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if  (realX + objectWidth >= Xbol && realY - objectDepth < 0) {  // Right upper corner -- add only to down and left
            listPos.append(3);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if(realX + objectWidth >= Xbol && realY + objectDepth < Ybol && realY - objectDepth >= 0) {  // Right boundry -- add only to down, left and up
            listPos.append(1);
            listPos.append(3);

            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if (realX + objectWidth >= Xbol && realY + objectDepth >= Ybol)  {  // Right down corner -- add only to left and up
            listPos.append(1);

            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if  (realY + objectDepth >= Ybol && realX - objectWidth >= 0 && realX + objectWidth < Xbol) {  // Down boundry -- add only to left, up and right
            listPos.append(1);

            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if (realX - objectWidth < 0 && realY + objectDepth >= Ybol) {  // Left down corner -- add only to up and right
            listPos.append(1);

            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else if (realX - objectWidth < 0 && realY + objectDepth < Ybol && realY - objectDepth >= 0)  {  // Left boundry -- add only to up, right and down
            listPos.append(1);

            listPos.append(3);
            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          } else {
            listPos.append(1);
            listPos.append(3);

            randOpt = int(random(0, listPos.size()));
            objectOpt = listPos.get(randOpt);
          }
        }

        if (objectOpt == 2 && matrix[realX + 1][realY] == objectZoneValue && matrix[realX + objectWidth][realY + objectDepth - 1] == objectZoneValue) {  // right

          for (int i = realX + 1; i < realX + objectWidth + 1; i++)
          {
            for (int j= realY; j< realY + objectDepth; j++)
            {
              int newi = i;
              int newj = j;
              matrix[newi][newj] = objectValue;
              fill(objectR, objectG, objectB);
              noStroke();
              rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
            }
          }
          success2++;
          success1++;
        } else if (objectOpt == 4 && matrix[realX - objectWidth][realY] == objectZoneValue && matrix[realX -1][realY + objectDepth - 1] == objectZoneValue) { // left

          for (int i = realX - objectWidth; i < realX; i++)
          {
            for (int j= realY; j< realY + objectDepth; j++)
            {
              int newi = i;
              int newj = j;
              matrix[newi][newj] = objectValue;
              fill(objectR, objectG, objectB);
              noStroke();
              rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
            }
          }
          success2++;
          success1++;
        } else if (objectOpt == 1 && matrix[realX][realY - objectDepth] == objectZoneValue && matrix[realX + objectWidth - 1][realY - 1] == objectZoneValue) {  // up

          for (int i = realX; i < realX + objectWidth; i++)
          {
            for (int j= realY - objectDepth; j< realY; j++)
            {
              int newi = i;
              int newj = j;
              matrix[newi][newj] = objectValue;
              fill(objectR, objectG, objectB);
              noStroke();
              rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
            }
          }
          success2++;
          success1++;
        } else if (objectOpt == 3 && matrix[realX][realY + 1] == objectZoneValue && matrix[realX + objectWidth - 1][realY + objectDepth] == objectZoneValue) {  // down

          for (int i = realX; i < realX + objectWidth; i++)
          {
            for (int j= realY + 1; j< realY + objectDepth; j++)
            {
              int newi = i;
              int newj = j;
              matrix[newi][newj] = objectValue;
              fill(objectR, objectG, objectB);
              noStroke();
              rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
            }
          }
          success2++;
          success1++;
        } else if (whilecnt2 >= 100) {
          success2++;
        } else
          println("failed");
      }
      if (whilecnt >= 5000) {
        success1++;
      }
    }
  }
}
