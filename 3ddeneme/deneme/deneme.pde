//deneme 1********************************************* 
import peasy.*;
import processing.dxf.*;
PeasyCam cam;
boolean drawDxf= false;
float x,y,z;
void setup() {
  size(200,200,P3D);
   cam= new PeasyCam(this, 100);
    x = width/2;
  y = height/2;
  z = 0;
}
void draw() {
  translate(x,y,z);
  rectMode(CENTER);
  rect(0,0,100,100);

  z++; // The rectangle moves forward as z increments.
}

//deneme 2*********************************************
/*

import peasy.*;

PeasyCam cam;

void setup() {
  size(200,200,P3D);
  cam = new PeasyCam(this, 100);
  cam.setMinimumDistance(50);
  cam.setMaximumDistance(500);
}
void draw() {
//  rotateX(-.5);
//  rotateY(-.5);
  background(0);
  fill(255,0,0);
  box(30,30,30);
  pushMatrix();
  translate(0,0,20);
  fill(0,0,255);
  //box(5);
  popMatrix();
}

*/
