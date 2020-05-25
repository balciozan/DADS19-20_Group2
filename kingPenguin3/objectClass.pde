class objectClass
{
  IntList listX;
  IntList listY;
  IntList listPos;
  IntList listSide;
  
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
  int objectRotated;

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
   objectClass(String oName, int oValue, int oZoneValue, int oWidth, int oDepth, int oHeight, int oMarLeft, int oMarRight, int oMarFront, int oMarRear, int oFixed, int oSource, int oR, int oG, int oB, int rotated) {

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
    objectRotated= rotated;
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
//değişiklik: refobjecti kaldırdım objectClass other ekledim
  void placeObject( int method, int dist, int longside, int rotate, objectClass referencedObject)
  {
    //rotate 0, 1, 2 = no rotate, rotate,random
    //longside 0,1,2= random, add from long edge, add from short edge
     // refObject is the reference for the object to be placed.
    // method can have 3 values for now. 0 = no relation / 1 = near to / 2 = next to.
    // if the method is equal to 1, dist will determine the range.
   int refWidth=0;
   int refDepth=0;
   int refObject=0; 
   int ifRefRotated=0;
   
 refWidth= referencedObject.objectWidth;
 refDepth= referencedObject.objectDepth;
 refObject = referencedObject.objectValue;
 ifRefRotated = referencedObject.objectRotated; 
  
    println("referecedObject " + refObject);
  println("refWidth kontrol " + refWidth);
  println("objectWidth kontrol " + objectWidth);
  
     //if referenced object ratated pull its revised dimesions 
  if  (ifRefRotated ==1 ) 
  {
    int hiddenlayer=0;
  hiddenlayer= refWidth;
  refWidth=refDepth;
  refDepth=hiddenlayer;
   println("refence object was rotated " );
  }
  else 
   println("refence object was NOT rotated " );
   
 
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
     
          //**************ROTATEfinished**********************************
    
    listX = new IntList();
    listY = new IntList();
    listSide = new IntList();
    listX.clear();
    listY.clear();
     listSide.append(-1);
    listSide.append(1);   
   
 float distX = 0;
    float distY = 0;
    float distH = 0;
    
    
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
       
        int randXSide = 0;
        int randYSide = 0;
        int randSidePicker = 0;
        
        randSidePicker = int(random(0, 2));
        randXSide = listSide.get(randSidePicker);
        
        randSidePicker = int(random(0, 2));
        randYSide = listSide.get(randSidePicker);
        
        listPos = new IntList();
        listPos.clear();

        distH = dist;
        distX = sqrt(random(0, sq(distH) + 1));
        distY = sqrt(sq(distH) - sq(distX));

        //println(distH);
        //println(int(distX));
        //println(int(distY));

        whilecnt2++;
        
      
                println("REFDEPTH********"+refDepth);//kontrol
     //controlling if the short edge placed in y axis
        // longside =1 means place next object from long side
        if (longside==1 && refDepth > refWidth )
        {
        longside=2;
         println("longside changed 1 to 2 "+longside);//kontrol
        
        }
        //controlling if the short edge placed in y axis
        // longside =2 means place next object from short side
       else if (longside==2 && refDepth > refWidth )
        {
        longside=1;
        println("longside changed 2 to 1"+longside);//kontrol
        }
      
        
       if (longside == 0 )
         
        //default option where side of the placement does not matter
        {
           println("longside0" +longside);
           if (realX - objectWidth - int(distX) < 0 && realY - objectDepth - int(distY) < 0 && realY + objectDepth + int(distY) < Ybol && realX + objectWidth + int(distX) < Xbol) {  // Left upper corner -- add only to right and down
          listPos.append(2);
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY - objectDepth - int(distY) < 0 && realX - objectWidth - int(distX) >= 0 && realX + objectWidth + int(distX) < Xbol && realY + objectDepth + int(distY) < Ybol) {  // Upper boundry -- add only to right, down and left
          listPos.append(2);
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt); 
        } else if (realX + objectWidth + int(distX) >= Xbol && realY - objectDepth - int(distY) < 0 && realY + objectDepth + int(distY) < Ybol && realX - objectWidth - int(distX) >= 0) {  // Right upper corner -- add only to down and left
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth + int(distX) >= Xbol && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX - objectWidth - int(distX) >= 0) {  // Right boundry -- add only to down, left and up
          listPos.append(1);
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth + int(distX) >= Xbol && realY + objectDepth + int(distY) >= Ybol && realX - objectWidth - int(distX) >= 0 && realY - objectDepth - int(distY) >= 0) {  // Right down corner -- add only to left and up
          listPos.append(1);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY + objectDepth + int(distY) >= Ybol && realX - objectWidth - int(distX) >= 0 && realX + objectWidth + int(distX) < Xbol && realY - objectDepth - int(distY) >= 0) {  // Down boundry -- add only to left, up and right
          listPos.append(1);
          listPos.append(2);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) < 0 && realY + objectDepth + int(distY) >= Ybol && realX + objectWidth + int(distX) < Xbol  && realY - objectDepth - int(distY) >= 0) {  // Left down corner -- add only to up and right
          listPos.append(1);
          listPos.append(2);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) < 0 && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX + objectWidth + int(distX) < Xbol) {  // Left boundry -- add only to up, right and down
          listPos.append(1);
          listPos.append(2);
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) >= 0 && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX + objectWidth + int(distX) < Xbol) {
          listPos.append(1);
          listPos.append(2);
          listPos.append(3);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        }
        }
       else if (longside == 2  )
       
        //place short edge or right or left
        {
             println("longside2" + longside);
             
          if (realX - objectWidth - int(distX) < 0 && realY - objectDepth - int(distY) < 0 && realY + objectDepth + int(distY) < Ybol && realX + objectWidth + int(distX) < Xbol) {  // Left upper corner -- add only to right and down
          listPos.append(2);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY - objectDepth - int(distY) < 0 && realX - objectWidth - int(distX) >= 0 && realX + objectWidth + int(distX) < Xbol && realY + objectDepth + int(distY) < Ybol) {  // Upper boundry -- add only to right, down and left
          listPos.append(2);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt); 
        } else if (realX + objectWidth + int(distX) >= Xbol && realY - objectDepth - int(distY) < 0 && realY + objectDepth + int(distY) < Ybol && realX - objectWidth - int(distX) >= 0) {  // Right upper corner -- add only to down and left
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth + int(distX) >= Xbol && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX - objectWidth - int(distX) >= 0) {  // Right boundry -- add only to down, left and up
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth + int(distX) >= Xbol && realY + objectDepth + int(distY) >= Ybol && realX - objectWidth - int(distX) >= 0 && realY - objectDepth - int(distY) >= 0) {  // Right down corner -- add only to left and up
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY + objectDepth + int(distY) >= Ybol && realX - objectWidth - int(distX) >= 0 && realX + objectWidth + int(distX) < Xbol && realY - objectDepth - int(distY) >= 0) {  // Down boundry -- add only to left, up and right
          listPos.append(2);
          listPos.append(4);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) < 0 && realY + objectDepth + int(distY) >= Ybol && realX + objectWidth + int(distX) < Xbol  && realY - objectDepth - int(distY) >= 0) {  // Left down corner -- add only to up and right
          listPos.append(2);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) < 0 && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX + objectWidth + int(distX) < Xbol) {  // Left boundry -- add only to up, right and down
          listPos.append(2);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) >= 0 && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX + objectWidth + int(distX) < Xbol) {
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
           if (realX - objectWidth - int(distX) < 0 && realY - objectDepth - int(distY) < 0 && realY + objectDepth + int(distY) < Ybol && realX + objectWidth + int(distX) < Xbol) {  // Left upper corner -- add only to right and down
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY - objectDepth - int(distY) < 0 && realX - objectWidth - int(distX) >= 0 && realX + objectWidth + int(distX) < Xbol && realY + objectDepth + int(distY) < Ybol) {  // Upper boundry -- add only to right, down and left
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt); 
        } else if (realX + objectWidth + int(distX) >= Xbol && realY - objectDepth - int(distY) < 0 && realY + objectDepth + int(distY) < Ybol && realX - objectWidth - int(distX) >= 0) {  // Right upper corner -- add only to down and left
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth + int(distX) >= Xbol && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX - objectWidth - int(distX) >= 0) {  // Right boundry -- add only to down, left and up
          listPos.append(1);
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX + objectWidth + int(distX) >= Xbol && realY + objectDepth + int(distY) >= Ybol && realX - objectWidth - int(distX) >= 0 && realY - objectDepth - int(distY) >= 0) {  // Right down corner -- add only to left and up
          listPos.append(1);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realY + objectDepth + int(distY) >= Ybol && realX - objectWidth - int(distX) >= 0 && realX + objectWidth + int(distX) < Xbol && realY - objectDepth - int(distY) >= 0) {  // Down boundry -- add only to left, up and right
          listPos.append(1);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) < 0 && realY + objectDepth + int(distY) >= Ybol && realX + objectWidth + int(distX) < Xbol  && realY - objectDepth - int(distY) >= 0) {  // Left down corner -- add only to up and right
          listPos.append(1);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) < 0 && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX + objectWidth + int(distX) < Xbol) {  // Left boundry -- add only to up, right and down
          listPos.append(1);
          listPos.append(3);
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        } else if (realX - objectWidth - int(distX) >= 0 && realY + objectDepth + int(distY) < Ybol && realY - objectDepth - int(distY) >= 0 && realX + objectWidth + int(distX) < Xbol) {
          listPos.append(1);          
          listPos.append(3);         
          randOpt = int(random(0, listPos.size()));
          objectOpt = listPos.get(randOpt);
        }
        }
     

        if (objectOpt == 2 && realY + objectDepth + int(distY) * randYSide <= Ybol && realY + int(distY) * randYSide >= 0 && whilecnt2 < 1000) {  // right
          if (matrix[realX + 1 + int(distX)][realY + int(distY) * randYSide] == objectZoneValue && matrix[realX + objectWidth + int(distX)][realY + objectDepth + int(distY) * randYSide - 1] == objectZoneValue) {

            for (int i = realX + 1 + int(distX); i < realX + objectWidth + int(distX) + 1; i++)
            {
              for (int j = realY + int(distY) * randYSide; j< realY + objectDepth + int(distY) * randYSide; j++)
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
          }
        } else if (objectOpt == 4 && realY + objectDepth  + int(distY) * randYSide <= Ybol && realY + int(distY) * randYSide >= 0 && whilecnt2 < 1000) {  // left
          if (matrix[realX - objectWidth - int(distX)][realY + int(distY) * randYSide] == objectZoneValue && matrix[realX - 1 - int(distX)][realY + objectDepth + int(distY) * randYSide - 1] == objectZoneValue) {

            for (int i = realX - objectWidth - int(distX); i < realX - int(distX); i++)
            {
              for (int j = realY + int(distY) * randYSide; j< realY + objectDepth + int(distY) * randYSide; j++)
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
          }
        } else if (objectOpt == 1 && realX + objectWidth  + int(distX) * randXSide <= Xbol && realX + int(distX) * randXSide >= 0 && whilecnt2 < 1000) {  // up
          if (matrix[realX + int(distX) * randXSide][realY - objectDepth - int(distY)] == objectZoneValue && matrix[realX + objectWidth + int(distX) * randXSide - 1][realY - int(distY) - 1] == objectZoneValue) {

            for (int i = realX + int(distX) * randXSide; i < realX + objectWidth + int(distX) * randXSide; i++)
            {
              for (int j = realY - int(distY) - objectDepth; j< realY - int(distY); j++)
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
          }
        } else if (objectOpt == 3 && realX + objectWidth + int(distX) * randXSide <= Xbol && realX + int(distX) * randXSide >= 0 && whilecnt2 < 1000) {  // down
          if (matrix[realX + int(distX) * randXSide][realY + int(distY) + 1] == objectZoneValue && matrix[realX + int(distX) * randXSide + objectWidth - 1][realY + int(distY) + objectDepth] == objectZoneValue) {

            for (int i = realX + int(distX) * randXSide; i < realX + objectWidth + int(distX) * randXSide; i++)
            {
              for (int j = realY + int(distY) + 1; j < realY + objectDepth + int(distY) + 1; j++)
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
        } else if (whilecnt2 >= 1000) {
          success2++;
        } else
          println("failed");
      }
      println( "printed-succeeded" + objectValue);
      if (whilecnt >= 5000) {
        success1++;
      }
    }
  }
}
}
