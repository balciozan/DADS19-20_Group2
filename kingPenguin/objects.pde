void objects() {
  if (objectTrigger == 1) {
    table.referenceObject(0);
    chair.placeObject(101, 1, 2, 0);
    //chair.placeObject(101, 0, 2, 0);
    //monitor.placeObject(111, 0, 1, 3);

    //showerCabin.referenceObject(0);
    //toilet.placeObject(161, 0, 2, 0);

    if(failCounter == 0){
      saveFrame("Results/alternative-##.png");
    }
    //setup();
  }
}
