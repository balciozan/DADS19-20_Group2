void objects() {
  if (objectTrigger == 1) {
    table.referenceObject();
    chair.placeObject(101, 2, 0);
    chair.placeObject(101, 2, 0);
    kitchenUnit.referenceObject();
    bed.referenceObject();

    saveFrame("Results/alternative-##.png");
    setup();
  }
}
