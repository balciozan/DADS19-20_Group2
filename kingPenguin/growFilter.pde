void growFilter() {

  for (int j=1; j<Ybol-1; j++)
  {
    for (int i=1; i<Xbol-1; i++)
    {
      int newi = i;
      int newj = j;
      int filter1 = 0;
      int filter2 = 0;
      int filter3 = 0;
      int filter4 = 0;
      int filter5 = 0;


      for (int k = newi - 1; k < newi + 2; k++) {
        for (int l = newj - 1; l < newj + 2; l++) {
          if ( abs(k - newi) + abs(l - newj) != 0) {
            int newValue = 0;
            newValue = matrix[k][l];
            if (newValue == 1) {
              filter1++;
            } else if (newValue == 2) {
              filter2++;
            } else if (newValue == 3) {
              filter3++;
            } else if (newValue == 4) {
              filter4++;
            } else if (newValue == 5) {
              filter5++;
            }
          }
        }
      }
      if (filter1 >= 5) {
        matrix[newi][newi] = 1;
        fill(247, 207, 206);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter2 >= 5) {
        matrix[newi][newi] = 2;
        fill(215, 253, 209);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter3 >= 5) {
        matrix[newi][newi] = 3;
        fill(0, 0, 255);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter4 >= 5) {
        matrix[newi][newi] = 4;
        fill(255, 255, 0);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter5 >= 5) {
        matrix[newi][newi] = 5;
        fill(215, 253, 254);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      }
    }
  }
  

/*  
  for (int j=1; j<Ybol-1; j++)
  {
    for (int i=1; i<Xbol-1; i++)
    {
      int newi = i;
      int newj = j;
      int filter1 = 0;
      int filter2 = 0;
      int filter3 = 0;
      int filter4 = 0;
      int filter5 = 0;

      for (int k = newi - 1; k < newi + 2; k++) {
        for (int l = newj - 1; l < newj + 2; l++) {
          if ( abs(newi - k) + abs(newj - l) == 1) {
            int newValue = 0;
            newValue = matrix[k][l];
            if (newValue == 1) {
              filter1++;
            } else if (newValue == 2) {
              filter2++;
            } else if (newValue == 3) {
              filter3++;
            } else if (newValue == 4) {
              filter4++;
            } else if (newValue == 5) {
              filter5++;
            }
          }
        }
      }
      if (filter1 >= 2) {
        matrix[newi][newi] = 1;
        fill(247, 207, 206);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter2 >= 2) {
        matrix[newi][newi] = 2;
        fill(215, 253, 209);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter3 >= 2) {
        matrix[newi][newi] = 3;
        fill(0, 0, 255);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter4 >= 2) {
        matrix[newi][newi] = 4;
        fill(255, 255, 0);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      } else if (filter5 >= 2) {
        matrix[newi][newi] = 5;
        fill(215, 253, 254);
        noStroke();
        rect((width/Xbol)*newi, (height/Ybol)*newj, (width/Xbol), (height/Ybol));
      }
    }
  }
  */
}
