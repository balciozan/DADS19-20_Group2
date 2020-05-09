void objects() {
  if (objectTrigger == 1) {
    table.referenceObject();
    chair.placeObject(101, 2, 0);
    //chair.placeObject(101, 2, 0);
    //kitchenUnit.referenceObject();
    //bed.referenceObject();
    monitor.placeObject(101, 2, 0);

    showerCabin.referenceObject();
    toilet.placeObject(161, 2, 0);

    //saveFrame("Results/alternative-##.png");
    setup();
  }
}
