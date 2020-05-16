void objects() {
 if (objectTrigger == 1 ) {
 //  (int refObject, int method, int dist, int longside, int rotate)**info
 //rotate 0, 1, 2 = no rotate, rotate,random**info
 //longside 0,1,2= random, add to up/down, add to right/left**info
    kitchenUnit.referenceObject();
      table.placeObject(121,1,1,1,0);
   chair.placeObject(101, 2, 0,2,0);
   chair.placeObject(101, 2, 0,2,0);
   bed.referenceObject();
   monitor.referenceObject();
    //bed.placeObject(111,1,1,2,1);
   // monitor.placeObject(141,1,1,false);
   
    showerCabin.referenceObject();
    toilet.placeObject(201, 2, 0,0,0);

    saveFrame("Results/alternative-##.png");
    println("success");
    setup();
 }
}
//yedekler

/*
   //controlling if the short edge placed in y axis
        // longside =1 means place next object from long side
        if (longside==1 && objectDepth > objectWidth )
        {
        longside=2;
        }
        //controlling if the short edge placed in y axis
        // longside =2 means place next object from short side
        if (longside==2 && objectDepth > objectWidth )
        {
        longside=1;
        }
       // referans objenin ölçüsünü almalı
        */
        

 /*   rotate =true;  
{ 
  int hiddenlayer=0;
  hiddenlayer=objectDepth;
  objectDepth= objectWidth;
  objectWidth = hiddenlayer;
  
  
  table.referenceObject();
    chair.placeObject(101, 2, 0,false);
    chair.placeObject(101, 2, 0,true);
   kitchenUnit.placeObject(101,1,1,false);
   
} */
