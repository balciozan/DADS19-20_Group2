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


  //PImage img;




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

  void referenceObject(int position) { // Function for placing reference objects. Each zone has 1 reference object.
    // position determines the relation between walls and the objects. 0 = no relation / 1 = next to wall.
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

      if (position == 0 && realX + objectWidth -1 <= listX.max() && realY + objectDepth -1 <= listY.max()) {
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
      } else if (position == 1 && (realX == 0 || realX == Xbol - objectWidth || realY == 0 || realY == Ybol - objectDepth) && realX + objectWidth -1 <= listX.max() && realY + objectDepth -1 <= listY.max()) {
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

  void placeObject(int refObject, int position, int method, int dist) { // Function for placing dependent objects.
    // refObject is the reference for the object to be placed.
    // position determines the relation between walls and the objects. 0 = no relation / 1 = next to wall.
    // method can have 3 values for now. 0 = no relation / 1 = near to / 2 = next to.
    // if the method is equal to 1, dist will determine the range.
    listX = new IntList();
    listY = new IntList();
    listSide = new IntList();
    listX.clear();
    listY.clear();
    listSide.clear();
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



    int success1 = 0;
    int whilecnt = 0;

    while (success1 == 0) {

      whilecnt++;
      //println(whilecnt);

      int rand = 0;
      int realX = 0;
      int realY = 0;


      rand = int(random(0, listX.size())); 
      realX = listX.get(rand);
      realY = listY.get(rand);

      int success2 = 0;
      int whilecnt2 = 0;

      while (success2 == 0 ) {

        whilecnt2++;
        //println(whilecnt2);

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


        if (method == 0) {
          distH = int(random(0, 12));
          distX = sqrt(random(0, sq(distH) + 1));
          distY = sqrt(sq(distH) - sq(distX));
        } else if (method == 1) {
          //distH = int(random(0,dist+1));
          distH = dist;
          distX = sqrt(random(0, sq(distH) + 1));
          distY = sqrt(sq(distH) - sq(distX));
        } else if (method == 2) {
          distH = 0;
          distX = 0;
          distY = 0;
        }

        if (position == 1 && whilecnt2 < 1000) {
          listX.clear();
          listY.clear();

          for (int j=0; j< Ybol; j++)
          {
            for (int i=0; i< Xbol; i++)
            {
              if (matrix[i][j] == refObject && (i == 0 || j == 0 || i == Xbol - 1 || j == Ybol - 1 || i - objectWidth - int(distX) == 0 || j + objectDepth + int(distY) == Ybol -1 || j - objectDepth - int(distY) == 0 || i + objectWidth + int(distX) == Xbol - 1)) {
                listX.append(i);
                listY.append(j);
              }
            }
          }
          if (listX.size() > 0 && whilecnt2 < 1000) {
            rand = int(random(0, listX.size())); 
            realX = listX.get(rand);
            realY = listY.get(rand);
            if (realX - objectWidth - int(distX) == 0) {
              if (objectOpt == 4 && realY + objectDepth  + int(distY) * randYSide <= Ybol && realY + int(distY) * randYSide >= 0 && whilecnt2 < 1000) {  // left
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
              }
            } else if (realX + objectWidth + int(distX) == Xbol - 1) {
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
              }
            } else if (realY - objectDepth - int(distY) == 0) {
              if (objectOpt == 1 && realX + objectWidth  + int(distX) * randXSide <= Xbol && realX + int(distX) * randXSide >= 0 && whilecnt2 < 1000) {  // up
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
              }
            } else if (realY + objectDepth + int(distY) == Ybol -1) {
              if (objectOpt == 3 && realX + objectWidth + int(distX) * randXSide <= Xbol && realX + int(distX) * randXSide >= 0 && whilecnt2 < 1000) {  // down
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
                }
              }
            }
          }
        }

        //println(distH);
        //println(int(distX));
        //println(int(distY));


        else if (position == 0 && whilecnt < 1000) {
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


          //println(whilecnt2);
          //println(whilecnt);

          //println("realX " + realX);
          //println("realY " + realY);
          //println("DistX " + int(distX));
          //println("DistY " + int(distY));

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
            }
          }
        } else if (whilecnt2 >= 1000) {
          success2++;
        }
      }
      if (whilecnt >= 5000) {
        success1++;
        failCounter++;
      }
    }
  }
}
