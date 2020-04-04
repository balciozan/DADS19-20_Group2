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
    int k = int(random(1, Xbol-1));
    int l = int(random(1, Ybol-1));

    int newk = 0;
    int newl = 0;

    while ( successFirst == 0) {

      if (matrix[k][l] == 0) {
        newk = k;
        newl = l;

        matrix[k][l] = colorValue;


        for (int j=1; j<Ybol-1; j++)
        {
          for (int i=1; i<Xbol-1; i++)
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
    text((float)colorSize/100 + " sqm", centerX*(400/7), (centerY*(400/7)) + 18);
  }

  void growing() {
    int whilecnt = 0;
    int i = 0;
    int j = 0;
    int success3 = 0;

    gravityCenter();
    //spaceNaming();

    while (success3 == 0) { // Limits the growing to determinated number.
      int success2 = 0;
      if ( colorSize < colorBorder) {      
        while (success2 == 0) { // Randomly checks four sides of the randomly founded red cell. If the checked cell is white - empty -, grows in that direction. If it does not white, checks another side randomly until find the white cell.         
          whilecnt++;
          int success = 0;
          while (success == 0) { //Checks cells randomly if it is red or not. When it finds the red cell, loop ends and function proceeds.

            int  randx = int(random(0, Xbol-1));
            int  randy = int(random(0, Ybol-1));
            //float dist = dist(randx, randy, 18, 1); 
            float cellRange = sqrt(colorBorder);

            if (matrix[randx][randy] == colorValue && dist(randx, randy, centerX, centerY) < cellRange) {
              i = randx;
              j = randy;
              success = 1;
            }
          }
          int newi = i;
          int newj = j;
          if (i == 0 && j > 0 && j < Ybol) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i+1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j+1;
              } else if (ekseny <= 0) {
                newj = j-1;
              }
            }
          } else if (i == Xbol && j > 0 && j < Ybol) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i-1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j+1;
              } else if (ekseny <= 0) {
                newj = j-1;
              }
            }
          } else if (i == 0 && j == 0) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i+1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j+1;
              }
            }
          } else if (i == Xbol && j == Ybol) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i-1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j-1;
              }
            }
          } else if (i == Xbol && j == 0) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i-1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j+1;
              }
            }
          } else if (i == 0 && j == Ybol) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i+1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j-1;
              }
            }
          } else if (j == Ybol && i > 0 && i < Xbol) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i+1;
              } else if (eksenx <= 0) {
                newi = i-1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j-1;
              }
            }
          } else if (j == 0 && i > 0 && i < Xbol) {


            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i+1;
              } else if (eksenx <= 0) {
                newi = i-1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j+1;
              }
            }
          } else if (i > 0 && i < Xbol && j > 0 && j < Ybol) {

            float eksen = random(-1, 1);

            if (eksen > 0) {
              float eksenx = random(-1, 1);
              if (eksenx > 0) {
                newi = i+1;
              } else if (eksenx <= 0) {
                newi = i-1;
              }
            } else if (eksen <= 0) {
              float ekseny = random(-1, 1);
              if (ekseny > 0) {
                newj = j+1;
              } else if (ekseny <= 0) {
                newj = j-1;
              }
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
          } else if (whilecnt > 5000) {
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