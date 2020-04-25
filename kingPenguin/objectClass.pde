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
    while (success1 == 0) {
      int rand = 0;
      int realX = 0;
      int realY = 0;
      int whilecnt = 0;

      rand = int(random(0, listX.size())); 

      realX = listX.get(rand);
      realY = listY.get(rand);

      if (realX + objectWidth <= listX.max() && realY + objectDepth <= listY.max()) {
        for (int i = realX; i < realX + objectWidth + 1; i++)
        {
          for (int j= realY; j< realY + objectDepth + 1; j++)
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
      } else if (whilecnt == 5000) {
        success1++;
      }
      whilecnt++;
    }
  }
}
