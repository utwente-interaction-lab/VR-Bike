#include <Servo.h>

Servo left;
Servo right;

const int leftoffset = 0;
const int rightoffset = 15;

int angle = 180;

long timer = 0;
const int updateTime = 50;

char buff[30];

void setup() {
  Serial.begin(115200);
  right.attach(10);
  left.attach(9);
  pinMode(7, OUTPUT);
  digitalWrite(7, HIGH);
  emptyBuffer();
  updateServos();
}

void loop() {
  if (checkSerial())
  {
    parseBuffer();
    updateServos();
  }
  if (updateNeeded()) updateWheel();
}

void updateWheel()
{
  int val = analogRead(A0) + 14;
  Serial.print(val);
  Serial.print("e");
}

void updateWheelWrite()
{
  int val = analogRead(A0) + 14;
  char cstr[16];
  itoa(val, cstr, 10);
  Serial.write(cstr);
  //Serial.write('\n');
  //Serial.write(10);
  //Serial.write(13);
}
bool updateNeeded()
{
  if (millis() > timer + updateTime)
  {
    timer = millis();
    return true;
  }
  return false;
}
void updateServos()
{
 left.write(leftoffset + angle);
 right.write(180 - (rightoffset + angle)); 
}

void emptyBuffer()
{
  for (int i = 0; i < 30; i++)
  {
    buff[i] = 0;
  }
}

void parseBuffer()
{
  int val = 0;
  int len = 0;
  for (int i = 0; i < 30; i++)
  {
    if (buff[i] != 0) len = i;
    else break;
  }
  for (int i = 0; i < len; i++)
  {
    val += factor(buff[i], (len - 1) - i);
  }
  angle = 180 - val;
}

int factor(char c, int f)
{
  return round((c - 48) * pow(10, f));
}

void printBuffer()
{
  Serial.print("got message: ");
  for (int i = 0; i < 30; i++)
  {
    if (buff[i] == 0) break;
    Serial.print(buff[i]);
  }
  Serial.println();
}
bool checkSerial()
{
  bool message = false;
  if (Serial.available())
  {
    message = true;
    delay(10);
    int pos = 0;
    emptyBuffer();
    while(Serial.available() > 0)
    {
      buff[pos] = Serial.read();
      pos ++;
    }
  }
  return message;
}
