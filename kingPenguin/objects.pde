void objects() {
  if (objectTrigger == 1) {
    table.referenceObject();
    chair.placeObject(101, 2, 0);
    chair.placeObject(101, 2, 0);
    monitor.placeObject(111, 2, 3);

    //showerCabin.referenceObject();
    //toilet.placeObject(161, 2, 0);

    saveFrame("Results/alternative-##.png");
    setup();
  }
}
