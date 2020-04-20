class objectClass
{

  String objectName;
  int objectValue;
  int objectR;
  int objectG;
  int objectB;
  int objectWidth;
  int objectHeight;
  int objectDepth;
  int objectX;
  int objectY;
  int objectZ;
  int objectMarLeft;
  int objectMarRight;
  int objectMarFront;
  int objectMarRear;
  int objectSource;
  int objectFixed;

  objectClass(String oName, int oValue, int oWidth, int oHeight, int oDepth, int oMarLeft, int oMarRight, int oMarFront, int oMarRear, int oX, int oY, int oZ, int oFixed, int oSource, int oR, int oG, int oB) {

    objectName = oName;
    objectValue = oValue;
    objectWidth = oWidth;
    objectHeight = oHeight;
    objectDepth = oDepth;
    objectMarLeft = oMarLeft;
    objectMarRight = oMarRight;
    objectMarFront = oMarFront;
    objectMarRear = oMarRear;
    objectX = oX;
    objectY = oY;
    objectZ = oZ;
    objectFixed = oFixed;
    objectSource = oSource;
    objectR = oR;
    objectG = oG;
    objectB = oB;
  }
}
