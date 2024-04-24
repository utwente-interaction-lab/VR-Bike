using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

//bike input:
namespace Scripts
{
    public class BikeDynamics : MonoBehaviour
    {
        // Unity Input
        [SerializeField] private bool keyboardInput;

        //Objects

        private Rigidbody self;
        private Transform  steeringwheel;
        private Transform bikeframe;
        //private BikeClient bikeclient;
        private BikeSocketClient bikesocket;

        // Main Variables

        //public float speed;

        float maxSteeringRange = Mathf.PI / 4; //45 degrees

        public int[] bikedata = { 0, 0, 0, 0, 0, 0}; //contains bikedata

        //for brake testing
        public bool keyboardBrake = false;

        public float steerheading = 0;
        public float heading = 0;
        public float speed = 0;

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log("Hello there");
            self = GetComponent<Rigidbody>();
            steeringwheel = transform.GetChild(0);
            bikeframe = transform.GetChild(1);
            bikesocket = new BikeSocketClient();
        }

        // Update is called once per frame
        void Update()
        {
            //aesthetical update, for now it can be empty
        }

        private void FixedUpdate()
        {
            getKeyBoardInput();
            if (bikesocket.hasMessage()) bikedata = bikesocket.parseData();
            
            //int resistance = applyMovement();
            int resistance = testMovement();
            bikesocket.sendMessage("" + resistance);
            
        }

        private void getKeyBoardInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Debug.Log("space pressed");
                keyboardBrake = true;
            }
            if (Input.GetKeyUp(KeyCode.Space)) keyboardBrake = false;
        }
        private int testMovement()
        {
            //get the heading and rotate the steering wheel accordingly
            float newheading = getHeading();
            float sDiff = newheading - steerheading;
            steerheading = newheading;
            
            steeringwheel.RotateAroundLocal(new Vector3(0, -120, 90), sDiff); //not obsolete

            Vector3 spd = new Vector3(0, 0, 10);
            //Vector3 pos = self.position;
            Vector3 newpos = self.position + spd;

            self.MovePosition(newpos);
            //apply movement to the bike:

            //float inputspd = 0.1f;
            //Vector3 vel = self.GetPointVelocity(new Vector3(0, 0, 0));

            //self.
            //self.MovePosition(new Vector3())
    
            //self.move
            //Vector3 newpos = convertHeading(inputspd);

            self.MovePosition(new Vector3(0, 0, 0));
            //self.MoveRotation()
            //self.position += newpos;
            //self.position = 

            //bikeframe.po
            //self.
            //Debug.Log("spd:" + bikedata[0] + "lb" + bikedata[1] + "rb" + bikedata[2]);
            //Debug.Log(heading);
            
            //steeringwheel.ro
            //transform.localRotation = rotateSteeringWheel(testheading);
            //Quaternion forward = transform;
            //steeringwheel.(forward, new Vector3(0, 120, 0), 0);
            //steeringwheel.transform.localRotation(rotateSteeringWheel(testheading));
            //self.MovePosition()
            return 0;
        }

        public Vector3 convertHeading(float spd)
        {
            return new Vector3(Mathf.Cos(heading) * spd, 0, Mathf.Sin(heading) * spd);
        }
        public float getHeading()
        {
            float fixedheading = scale(512 - 250, 512 + 250, maxSteeringRange, -maxSteeringRange, bikedata[5]);
            return fixedheading;
        }
       
        //public float 
        public float scale(float oldMin, float oldMax, float newMin, float newMax, float input)
        {
            float oldRange = (oldMax - oldMin);
            float newRange = (newMax - newMin);
            return (((input - oldMin) * newRange) / oldRange) + newMin;
        }

        void OnApplicationQuit()
        {
            Debug.Log("Application ending after " + Time.time + " seconds");
            //bikeclient.Disconnect();
        }
    }
}
