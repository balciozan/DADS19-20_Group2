void grow() {

  /*
  
   =================== Growing Functions =================  
   
   In growCell class, growing() function is coded. It should run for each zone.   
   */

  red.growing();
  green.growing();
  //blue.growing(); No longer needed
  //yellow.growing(); No longer needed
  cyan.growing();



  // Checks the growing if the current cell amount is equal to intended cell amount and if it is, makes an evaluation.
  growControl();
}
