//Service UUID:
//0000ffe0-0000-1000-8000-00805f9b34fb

//Characteristic UUID (Readable, Writable, Notifications):
//0000ffe1-0000-1000-8000-00805f9b34fb

#include <SoftwareSerial.h>

// PIN 4 = BT TX; PIN 2 = BT RX
SoftwareSerial BT(4,2);

// temperature pins
const int sensorPin = A0;

void setup()
{   
  Serial.begin(9600);
  BT.begin(9600);
}

void loop()
{
  int sensorVal = analogRead(sensorPin);
  float voltage = (sensorVal/1024.0) * 5;
  // voltage to temperatures in degrees
  float temperature = (voltage - .5) * 100;
  // write temperature to BT
  BT.print(temperature);
  
  delay(1);     
}
