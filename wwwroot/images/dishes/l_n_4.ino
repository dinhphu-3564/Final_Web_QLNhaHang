#include <Wire.h>
#include <LiquidCrystal_I2C.h>

// ================= LCD =================
LiquidCrystal_I2C lcd(0x27, 16, 2);   // đổi 0x3F nếu không hiện

// ================= MPU6050 =================
#define MPU6050_ADDR 0x68
#define MPU6050_REG_ACCEL_XOUT_H 0x3B
#define MPU6050_REG_PWR_MGMT_1   0x6B

// ================= HMC5883L =================
#define HMC5883L_ADDR 0x1E

// ================= SCALE =================
#define ACCEL_SCALE 16384.0
#define MAG_SCALE   0.92

#define RAD_TO_DEG (180.0 / PI)
#define DEG_TO_MIL (6000.0 / 360.0)
#define RAD_TO_MIL (RAD_TO_DEG * DEG_TO_MIL)

// ================= OFFSET =================
double accelOffsetX = 0, accelOffsetY = 0, accelOffsetZ = 0;

int16_t magMinX = 32767, magMaxX = -32768;
int16_t magMinY = 32767, magMaxY = -32768;
int16_t magMinZ = 32767, magMaxZ = -32768;

double magOffsetX = 0, magOffsetY = 0, magOffsetZ = 0;

// ==================================================
// ===== HÀM IN GÓC MIL RA LCD (ĐÚNG CHỖ) =====
void lcdPrintAngleMil(long angleMil) {
  if (angleMil < 0) angleMil = 0;
  if (angleMil > 6000) angleMil = 6000;

  int high = angleMil / 100;
  int low  = angleMil % 100;

  if (high == 0) lcd.print('0');
  else {
    if (high < 10) lcd.print('0');
    lcd.print(high);
  }

  lcd.print('-');

  if (low < 10) lcd.print('0');
  lcd.print(low);
}

// ==================================================
long wrapMil(long angleMil) {
  while (angleMil > 6000) angleMil -= 6000;
  while (angleMil < 0)    angleMil += 6000;
  return angleMil;
}

// ==================================================
void read_HMC5883L_raw(int16_t &x, int16_t &y, int16_t &z) {
  Wire.beginTransmission(HMC5883L_ADDR);
  Wire.write(0x03);
  Wire.endTransmission(false);
  Wire.requestFrom(HMC5883L_ADDR, 6, true);

  x = (Wire.read() << 8) | Wire.read();
  z = (Wire.read() << 8) | Wire.read();
  y = (Wire.read() << 8) | Wire.read();
}

void read_HMC5883L(double &mx, double &my, double &mz) {
  int16_t x, y, z;
  read_HMC5883L_raw(x, y, z);

  mx = (x - magOffsetX) * MAG_SCALE;
  my = (y - magOffsetY) * MAG_SCALE;
  mz = (z - magOffsetZ) * MAG_SCALE;
}

// ==================================================
void read_MPU6050(double &ax, double &ay, double &az) {
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(MPU6050_REG_ACCEL_XOUT_H);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU6050_ADDR, 6, true);

  int16_t rx = (Wire.read() << 8) | Wire.read();
  int16_t ry = (Wire.read() << 8) | Wire.read();
  int16_t rz = (Wire.read() << 8) | Wire.read();

  ax = rx / ACCEL_SCALE - accelOffsetX;
  ay = ry / ACCEL_SCALE - accelOffsetY;
  az = rz / ACCEL_SCALE - accelOffsetZ;
}

// ==================================================
void MPU6050_init() {
  Wire.beginTransmission(MPU6050_ADDR);
  Wire.write(MPU6050_REG_PWR_MGMT_1);
  Wire.write(0);
  Wire.endTransmission(true);
}

void calibrate_MPU6050() {
  double ax, ay, az;
  for (int i = 0; i < 200; i++) {
    read_MPU6050(ax, ay, az);
    accelOffsetX += ax;
    accelOffsetY += ay;
    accelOffsetZ += az;
    delay(10);
  }
  accelOffsetX /= 200;
  accelOffsetY /= 200;
  accelOffsetZ /= 200;
  accelOffsetZ -= 1.0;
}

// ==================================================
void HMC5883L_init() {
  Wire.beginTransmission(HMC5883L_ADDR);
  Wire.write(0x00);
  Wire.write(0x70);
  Wire.endTransmission();

  Wire.beginTransmission(HMC5883L_ADDR);
  Wire.write(0x01);
  Wire.write(0x20);
  Wire.endTransmission();

  Wire.beginTransmission(HMC5883L_ADDR);
  Wire.write(0x02);
  Wire.write(0x00);
  Wire.endTransmission();
}

void calibrate_HMC5883L() {
  int16_t x, y, z;
  unsigned long t0 = millis();

  while (millis() - t0 < 10000) {
    read_HMC5883L_raw(x, y, z);

    magMinX = min(magMinX, x); magMaxX = max(magMaxX, x);
    magMinY = min(magMinY, y); magMaxY = max(magMaxY, y);
    magMinZ = min(magMinZ, z); magMaxZ = max(magMaxZ, z);
    delay(100);
  }

  magOffsetX = (magMinX + magMaxX) / 2.0;
  magOffsetY = (magMinY + magMaxY) / 2.0;
  magOffsetZ = (magMinZ + magMaxZ) / 2.0;
}

// ================= SETUP =================
void setup() {
  Serial.begin(115200);
  Wire.begin();

  lcd.init();
  lcd.backlight();

  MPU6050_init();
  HMC5883L_init();

  calibrate_MPU6050();
  calibrate_HMC5883L();

  lcd.clear();
}

// ================= LOOP =================
void loop() {
  double ax, ay, az;
  double mx, my, mz;

  double pitchRad, rollRad, yawRad;
  long rollMil, yawMil;

  read_MPU6050(ax, ay, az);

  pitchRad = atan2(-ax, sqrt(ay * ay + az * az));
  rollRad  = atan2(ay, az);

  read_HMC5883L(mx, my, mz);

  double Xh = mx * cos(pitchRad) + mz * sin(pitchRad);
  double Yh = mx * sin(rollRad) * sin(pitchRad)
            + my * cos(rollRad)
            - mz * sin(rollRad) * cos(pitchRad);

  yawRad = atan2(Yh, Xh);

  float declinationAngle = -0.1783;
  yawRad += declinationAngle;

  if (yawRad < 0) yawRad += 2 * PI;
  if (yawRad > 2 * PI) yawRad -= 2 * PI;

  rollMil = round(rollRad * RAD_TO_MIL);
  yawMil  = wrapMil(round(yawRad * RAD_TO_MIL));

  // ===== LCD =====
  lcd.setCursor(0, 0);
  lcd.print("Ta: ");
  lcdPrintAngleMil(abs(rollMil));
  lcd.print("   ");

  lcd.setCursor(0, 1);
  lcd.print("Phuong vi: ");
  lcdPrintAngleMil(yawMil);
  lcd.print("   ");

  delay(500);
}
