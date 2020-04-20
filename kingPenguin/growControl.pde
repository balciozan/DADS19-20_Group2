void growControl() { 

  /*
 Sometimes, a color is tight cornered by other colors. In that case, cornered color
   can not grow as much as expected. In this scenario, results might be deceptive.
   
   In order to prevent that, growcontrol function counts the number of white cells if
   the amout is more than expected or not. Since small amount of errors acceptable,
   a tolerance is added to the function.
   
   If the error is big enough to cause deception, system reruns itself and skips the 
   undesirable result. */

  int allCells = Xbol * Ybol;
  int coloredCells = redBorder + greenBorder + cyanBorder; //+ yellowBorder + blueBorder
  int control = allCells - coloredCells + tolerance;
  int counterGrow = 0;

  loopCounter++;

  if (loopCounter >= cycleTime) 
  {
    for (int j=0; j<Ybol; j++)
    {
      for (int i=0; i<Xbol; i++)
      {
        if (matrix[i][j] == 0)
        {
          counterGrow++;
        }
      }
    }

    if (counterGrow > control) {
      setup();
    }
    if (counterGrow <= control) {
      growFilter();
      growFilter();
      red.spaceNaming();
      //yellow.spaceNaming();
      green.spaceNaming();
      cyan.spaceNaming();
      //blue.spaceNaming();

      saveFrame("Results/alternative-##.png");
      setup();
    }
  }
}
