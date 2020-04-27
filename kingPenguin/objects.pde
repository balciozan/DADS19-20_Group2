void objects() {
  if (objectTrigger == 1) {
    table.referenceObject();
    chair.placeObject(101, 2, 0);

    saveFrame("Results/alternative-##.png");
    setup();
  }
}
