class growCell
{

  int colorValue;
  int colorR;
  int colorG;
  int colorB;
  int colorSize;
  int colorBorder;
  String colorName;

  int firsti;
  int firstj;

  float centerCounter = 0;
  float centerX = 0;
  float centerY = 0;
  float totalX = 0;
  float totalY = 0;

  int k;
  int l;
  int m;

  IntList listPos;



  growCell(String cName, int cValue, int cSize, int cBorder, int cR, int cG, int cB) {

    colorName = cName;
    colorValue = cValue;
    colorR = cR;
    colorG = cG;
    colorB = cB;
    colorSize = cSize;
    colorBorder = cBorder;
  }

  void firstCell() { // Function for creating the first cells. Runs only one time in each organization alternative.
    int successFirst = 0;

    while ( successFirst == 0) {
      m = int(random(1, 5)); // According to the 'm' value, first cell will be placed either left, right, up or down.

      if (m == 1) { // Left border
        k = 0;
        l = int(random(0, Ybol-1));

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=0; j<Ybol; j++)
          {
            for (int i=0; i<Xbol; i++)
            {
              if (matrix[i][j]== colorValue)
              {

                fill(colorR, colorG, colorB);
                noStroke();
                rect((width/Xbol)*i, (height/Ybol)*j, (width/Xbol), (height/Ybol));
                successFirst = 1;
              }
            }
          }
        }
      } else if (m == 2) { // Right border
        k = Xbol-1;
        l = int(random(0, Ybol-1));

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=0; j<Ybol; j++)
          {
            for (int i=0; i<Xbol; i++)
            {
              if (matrix[i][j]== colorValue)
              {

                fill(colorR, colorG, colorB);
                noStroke();
                rect((width/Xbol)*i, (height/Ybol)*j, (width/Xbol), (height/Ybol));
                successFirst = 1;
              }
            }
          }
        }
      } else if (m == 3) { // Upper border
        k = int(random(0, Xbol-1));
        l = 0;

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=0; j<Ybol; j++)
          {
            for (int i=0; i<Xbol; i++)
            {
              if (matrix[i][j]== colorValue)
              {

                fill(colorR, colorG, colorB);
                noStroke();
                rect((width/Xbol)*i, (height/Ybol)*j, (width/Xbol), (height/Ybol));
                successFirst = 1;
              }
            }
          }
        }
      } else if (m == 4) { // Down border
        k = int(random(0, Xbol-1));
        l = Ybol-1;

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=0; j<Ybol; j++)
          {
            for (int i=0; i<Xbol; i++)
            {
              if (matrix[i][j]== colorValue)
              {

                fill(colorR, colorG, colorB);
                noStroke();
                rect((width/Xbol)*i, (height/Ybol)*j, (width/Xbol), (height/Ybol));
                successFirst = 1;
              }
            }
          }
        }
      }
    }
  }


  void gravityCenter() { // Calculates the gravity center of the zones each time a new cell added.
    int newi = 0;
    int newj = 0;

    for (int j=0; j<Ybol; j++)
    {
      for (int i=0; i<Xbol; i++)
      {
        if (matrix[i][j]== colorValue)
        {
          centerCounter++;

          newi = i;
          newj = j;

          totalX = totalX + newi;
          totalY = totalY + newj;
          centerX = totalX / centerCounter;
          centerY = totalY / centerCounter;
        }
      }
    }
  }


  void spaceNaming() { // Names the spaces at the end of the process.
    gravityCenter();
    textSize(16);
    textAlign(CENTER);
    fill(1);
    text(colorName, (centerX)*(width/Xbol), (centerY)*(height/Ybol));
    text((float)colorSize*0.36 + " sqm", (centerX)*(width/Xbol), ((centerY)*(height/Ybol)) + 18);
  }


  /*
void cornerCell() {     NO LONGER NEEDED
   
   if (matrix[0][0] == 0 && matrix[1][0] == colorValue && matrix[0][1] == colorValue) {
   matrix[0][0] = colorValue;
   fill(colorR, colorG, colorB);
   noStroke();
   rect((0), (0), (width/Xbol), (height/Ybol));
   colorSize++;
   }
   if (matrix[0][Ybol-1] == 0 && matrix[1][Ybol-1] == colorValue && matrix[0][Ybol-2] == colorValue) {
   matrix[0][Ybol-1] = colorValue;
   fill(colorR, colorG, colorB);
   noStroke();
   rect((0), (height/Ybol)*(Ybol-1), (width/Xbol), (height/Ybol));
   colorSize++;
   }
   if (matrix[Xbol-1][0] == 0 && matrix[Xbol-1][1] == colorValue && matrix[Xbol-2][0] == colorValue) {
   matrix[Xbol-1][0] = colorValue;
   fill(colorR, colorG, colorB);
   noStroke();
   rect((width/Xbol)*(Xbol-1), (0), (width/Xbol), (height/Ybol));
   colorSize++;
   }
   if (matrix[Xbol-1][Ybol-1] == 0 && matrix[Xbol-2][Ybol-1] == colorValue && matrix[Xbol-1][Ybol-2] == colorValue) {
   matrix[Xbol-1][Ybol-1] = colorValue;
   fill(colorR, colorG, colorB);
   noStroke();
   rect((width/Xbol)*(Xbol-1), (height/Ybol)*(Ybol-1), (width/Xbol), (height/Ybol));
   colorSize++;
   }
   }
   */

  void growing() { // Function lets the zones grow one by one.
    int whilecnt = 0;
    int i = 0;
    int j = 0;
    int success3 = 0;

    gravityCenter();
    //firstCell();
    //spaceNaming();

    while (success3 == 0) { // Limits the growing to determinated number.
      int success2 = 0;
      if ( colorSize < colorBorder) {      
        while (success2 == 0) { // Randomly checks four sides of the randomly founded red cell. If the checked cell is white - empty -, grows in that direction. If it does not white, checks another side randomly until find the white cell.         
          whilecnt++;
          int success = 0;
          while (success == 0) { //Checks cells randomly if it is red or not. When it finds the red cell, loop ends and function proceeds.

            int  randx = int(random(0, Xbol));
            int  randy = int(random(0, Ybol));
            //float dist = dist(randx, randy, 18, 1); 
            float cellRange = sqrt(colorBorder * (height/Ybol) * (width/Xbol) / PI) * 0.5; //dynamic center input
            //float cellRange = sqrt(colorBorder * (height/Ybol) * (width/Xbol) / PI) * 0.038 ; //static center input

            if (matrix[randx][randy] == colorValue && dist(randx, randy, centerX, centerY) < cellRange) { //dynamic center
              //if (matrix[randx][randy] == colorValue && dist(randx, randy, k, l) < cellRange) { //static center

              i = randx;
              j = randy;
              success = 1;
            }
          }

          int newi = i;
          int newj = j;


          int randOpt = 0;
          int zoneOpt = 0;
          listPos = new IntList();
          listPos.clear();

          if (i == 0 && j == 0) {  // Left upper corner -- add only to right and down
            listPos.append(2);
            listPos.append(3);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else if (j == 0 && i != 0 && i != Xbol - 1) {  // Upper boundry -- add only to right, down and left
            listPos.append(2);
            listPos.append(3);
            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else if (i == Xbol - 1 && j == 0) {  // Right upper corner -- add only to down and left
            listPos.append(3);
            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else if (i == Xbol - 1 && j != 0 && j != Ybol-1) {  // Right boundry -- add only to down, left and up
            listPos.append(1);
            listPos.append(3);
            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else if (i == Xbol - 1 && j == Ybol-1) {  // Right down corner -- add only to left and up
            listPos.append(1);
            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else if (j == Ybol - 1 && i != 0 && i != Xbol - 1) {  // Down boundry -- add only to left, up and right
            listPos.append(1);
            listPos.append(2);
            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else if (i == 0 && j == Ybol - 1) {  // Left down corner -- add only to up and right
            listPos.append(1);
            listPos.append(2);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else if (i == 0 && j != 0 && j != Ybol - 1) {  // Left boundry -- add only to up, right and down
            listPos.append(1);
            listPos.append(2);
            listPos.append(3);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          } else {
            listPos.append(1);
            listPos.append(2);
            listPos.append(3);
            listPos.append(4);
            randOpt = int(random(0, listPos.size()));
            zoneOpt = listPos.get(randOpt);
          }


          if (zoneOpt == 2) {
            newi = i+1;
          } else if (zoneOpt == 4) {
            newi = i-1;
          } else if (zoneOpt == 3) {
            newj = j+1;
          } else if (zoneOpt == 1) {
            newj = j-1;
          }

          if ((matrix[newi][newj] == 0) && (newj < Ybol) && (newj >= 0) && (newi < Xbol) && (newi >= 0)) {
            success2 = 1;
            success3 = 1;
            colorSize++;
            matrix[newi][newj] = colorValue;
            fill(colorR, colorG, colorB);
            noStroke();
            rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
          } else if (whilecnt > 8000) {
            success2 = 1;
            success3 = 1;
          }
        }
      } else if (colorSize >= colorBorder) {
        success3 = 1;
      }
    }
  }
}
