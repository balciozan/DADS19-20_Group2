class objectClass
{
  IntList listX;
  IntList listY;
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

  void referenceObject() {
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
          }
        }
        success1++;
      } else if (whilecnt == 1000) {
        success1++;
      }
    }
  }

  void placeObject(int refObject, int method, int dist) {
    // refObject is the reference for the object to be placed.
    // method can have 3 values for now. 0 = no relation / 1 = near to / 2 = next to.
    // if the method is equal to 1, dist will determine the range.
    listX = new IntList();
    listY = new IntList();
    listX.clear();
    listY.clear();

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
      int rand = 0;
      int realX = 0;
      int realY = 0;

      whilecnt++;

      rand = int(random(0, listX.size())); 
      realX = listX.get(rand);
      realY = listY.get(rand);

      int success2 = 0;
      int whilecnt2 = 0;
      while (success2 == 0 ) {
        whilecnt2++;
        float axis = random(-1, 1); // decides randomly where to put the object

        if (axis > 0) {
          float axisX = random(-1, 1);
          if (axisX > 0 && matrix[realX + 1][realY] == objectZoneValue && matrix[realX + objectWidth][realY + objectDepth - 1] == objectZoneValue) {  // right
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
          } else if (axisX <= 0 && matrix[realX - objectWidth][realY] == objectZoneValue && matrix[realX -1][realY + objectDepth - 1] == objectZoneValue) { // left
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
          }
        } else if (axis <= 0) {
          float axisY = random(-1, 1);
          if (axisY > 0 && matrix[realX][realY - objectDepth] == objectZoneValue && matrix[realX + objectWidth - 1][realY - 1] == objectZoneValue) {  // up
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
          } else if (axisY <= 0 && matrix[realX][realY + 1] == objectZoneValue && matrix[realX + objectWidth - 1][realY + objectDepth] == objectZoneValue) {  // down
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
          }
        } else if (whilecnt2 >= 10000) {
          success2++;
        }
      }
      if (whilecnt >= 5000) {
        success1++;
      }
    }
  }
}