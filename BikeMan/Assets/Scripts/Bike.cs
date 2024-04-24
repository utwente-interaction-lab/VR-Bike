using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Bike
    {
        private Transform steer;
        private Transform frame;
        private Transform cyclist;
        private Transform self;

        private Vector3 steerNormal = new Vector3(0, -120, 90);

        private Vector3 dPos;

        private float steerangle = 0;
        private float psteerangle = 0;
        private float bikeangle = 0;

        private int lbrake = 0;
        private int rbrake = 0;
        private int normalResistance;

        private float speed = 0;
        private float maxspeed;
        private float speedFactor;

        private Vector3 pPos;
        public Bike(Transform t, float maxspd, float spdFactor, int normalR)
        {
            self = t;
            steer = t.GetChild(2);
            frame = t.GetChild(1);
            cyclist = t.GetChild(0);
            maxspeed = KMHToMS(maxspd / 50);
            speedFactor = spdFactor;
            pPos = t.position;
            normalResistance = normalR;
        }

        public int move(int[] data)
        {
            calcControls(data);
            rotateSteeringWheel();
            controlBike();
            return calcResistance();
        }

        public float MSToKMH(float ms)
        {
            return ms * 3.6f;
        }

        public float KMHToMS(float kmh)
        {
            return kmh / 3.6f;
        }

        public float getSpeed()
        {
            float dist = (self.position - pPos).magnitude;
            pPos = self.position;
            return MSToKMH(dist * 50);
        }

        private int calcResistance()
        {
            float r = 0;
            r += lbrake * 0.1f; //proportional effectiveness of the left brake
            r += rbrake * 0.1f; 
            return Mathf.RoundToInt(r) + normalResistance;
        }
        private void controlBike()
        {
            applyRotation(0.1f);
            Vector3 dir = getMovement(self.localRotation, speed);
            self.position += dir;
        }

        private void applyRotation(float rotspeed)
        {
            float rotation = speed * steerangle * rotspeed;
            self.RotateAroundLocal(new Vector3(0, 1, 0), rotation);
        }

        private Vector3 getMovement(Quaternion myRotation, float spd)
        {
            float myRot = myRotation.eulerAngles.y;

            myRot = scale(0, 180, 0, Mathf.PI, myRot);
            //Debug.Log(myRot);
            float dx = Mathf.Sin(myRot) * spd;
            float dz = Mathf.Cos(myRot) * spd;

            return new Vector3(dx, 0, dz);

        }
        
        public float getHeading(float angle, float maxAngle)
        {
            float fixedheading = scale(512 - 250, 512 + 250, -maxAngle, maxAngle, angle);
            return fixedheading;
        }

        public float getAngle()
        {
            return bikeangle;
        }

        public Vector3 getDiff()
        {
            return dPos;
        }
        private void calcControls(int[] data)
        {
            speed = data[0] * speedFactor;
            lbrake = data[1];
            rbrake = data[2];
            limitSpeed();
            steerangle = getHeading(data[5], Mathf.PI / 3);
        }

        void limitSpeed()
        {
            if (speed > maxspeed) speed = maxspeed;
        }

        private void rotateSteeringWheel()
        {
            float diff = psteerangle - steerangle;
            steer.RotateAroundLocal(steerNormal, diff);
            psteerangle = steerangle;
        }
        public float scale(float oldMin, float oldMax, float newMin, float newMax, float input)
        {
            float oldRange = (oldMax - oldMin);
            float newRange = (newMax - newMin);
            return (((input - oldMin) * newRange) / oldRange) + newMin;
        }
    }
}
