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




  growCell(String cName, int cValue, int cSize, int cBorder, int cR, int cG, int cB) {

    colorName = cName;
    colorValue = cValue;
    colorR = cR;
    colorG = cG;
    colorB = cB;
    colorSize = cSize;
    colorBorder = cBorder;
  }

  void firstCell() {
    int successFirst = 0;


    while ( successFirst == 0) {
      m = int(random(1, 5));
      if (m == 1) {
        k = 1;
        l = int(random(1, Ybol-1));

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=1; j<Ybol; j++)
          {
            for (int i=1; i<Xbol; i++)
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
      } else if (m == 2) {
        k = Xbol-2;
        l = int(random(1, Ybol-1));

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=1; j<Ybol; j++)
          {
            for (int i=1; i<Xbol; i++)
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
      } else if (m == 3) {
        k = int(random(1, Xbol-1));
        l = 1;

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=1; j<Ybol; j++)
          {
            for (int i=1; i<Xbol; i++)
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
      } else if (m == 4) {
        k = int(random(1, Xbol-1));
        l = Ybol-2;

        if (matrix[k][l] == 0) {

          matrix[k][l] = colorValue;


          for (int j=1; j<Ybol; j++)
          {
            for (int i=1; i<Xbol; i++)
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


  void gravityCenter() {
    int newi = 0;
    int newj = 0;

    for (int j=1; j<Ybol; j++)
    {
      for (int i=1; i<Xbol; i++)
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


  void spaceNaming() { 
    gravityCenter();
    textSize(16);
    textAlign(CENTER);
    fill(1);
    text(colorName, centerX*(400/7), centerY*(400/7));
    text((float)colorSize/4 + " sqm", centerX*(400/7), (centerY*(400/7)) + 18);
  }


  void cornerCell() {

    if (matrix[0][0] == 0 && matrix[1][0] == colorValue && matrix[0][1] == colorValue) {
      matrix[0][0] = colorValue;
      fill(colorR, colorG, colorB);
      noStroke();
      rect((0), (0), (width/Xbol), (height/Ybol));
    }
    if (matrix[0][Ybol-1] == 0 && matrix[1][Ybol-1] == colorValue && matrix[0][Ybol-2] == colorValue) {
      matrix[0][Ybol-1] = colorValue;
      fill(colorR, colorG, colorB);
      noStroke();
      rect((0), (height/Ybol)*(Ybol-1), (width/Xbol), (height/Ybol));
    }
    if (matrix[Xbol-1][0] == 0 && matrix[Xbol-1][1] == colorValue && matrix[Xbol-2][0] == colorValue) {
      matrix[Xbol-1][0] = colorValue;
      fill(colorR, colorG, colorB);
      noStroke();
      rect((width/Xbol)*(Xbol-1), (0), (width/Xbol), (height/Ybol));
    }
    if (matrix[Xbol-1][Ybol-1] == 0 && matrix[Xbol-2][Ybol-1] == colorValue && matrix[Xbol-1][Ybol-2] == colorValue) {
      matrix[Xbol-1][Ybol-1] = colorValue;
      fill(colorR, colorG, colorB);
      noStroke();
      rect((width/Xbol)*(Xbol-1), (height/Ybol)*(Ybol-1), (width/Xbol), (height/Ybol));
    }
  }


  void growing() {
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

            int  randx = int(random(1, Xbol - 1));
            int  randy = int(random(1, Ybol - 1));
            //float dist = dist(randx, randy, 18, 1); 
            float cellRange = sqrt(colorBorder * (height/Ybol) * (width/Xbol) / PI) * 1; //dynamic center input
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

          float axis = random(-1, 1);

          if (axis > 0) {
            float axisX = random(-1, 1);
            if (axisX > 0) {
              newi = i+1;
            } else if (axisX <= 0) {
              newi = i-1;
            }
          } else if (axis <= 0) {
            float axisY = random(-1, 1);
            if (axisY > 0) {
              newj = j+1;
            } else if (axisY <= 0) {
              newj = j-1;
            }
          }
          if ((matrix[newi][newj] == 0) && (newj <= Ybol) && (newj >= 0) && (newi <= Xbol) && (newi >= 0)) {
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
