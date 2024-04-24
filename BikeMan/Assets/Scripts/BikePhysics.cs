using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{

    public class BikePhysics : MonoBehaviour
    {

        [SerializeField] private bool keyboardInput;
        [SerializeField] private float maxSpeed = 30;
        [SerializeField] private float speedFactor = 0.003f;
        [SerializeField] private int normalResitance = 50;

        public int[] bikedata = { 0, 0, 0, 0, 0, 0 }; //contains bikedata

        private Bike bike;
        private Rigidbody self;
        private BikeSocketClient bikesocket;


        private float currentAngle = 0;

        //private Transform steer

        // Start is called before the first frame update
        void Start()
        {
            bike = new Bike(transform, maxSpeed, speedFactor, normalResitance);
            self = GetComponent<Rigidbody>();
            bikesocket = new BikeSocketClient();
        }

        // Update is called once per frame (slow)

        private void FixedUpdate()
        {
            if (keyboardInput) getManualControls();
            else if (bikesocket.hasMessage()) bikedata = bikesocket.parseData();

            Debug.Log(bikedata[0]);

            int resistance = bike.move(bikedata);
            //Debug.Log(resistance);
            bikesocket.sendMessage("" + resistance);

            float spd = bike.getSpeed();
            //Debug.Log("speed (kmh): " + spd);
        }
        

        void logData()
        {
            string logMSG = "";
            for (int i = 0; i < bikedata.Length; i++)
            {
                logMSG += bikedata[i];
                logMSG += ", ";
            }
            Debug.Log(logMSG);
        }




        void getManualControls()
        {
            //int outdata = new int[6];
            if (Input.GetKeyDown(KeyCode.Space)) bikedata[1] = 255;
            if (Input.GetKeyUp(KeyCode.Space)) bikedata[1] = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) changeSteer(-5);
            if (Input.GetKeyDown(KeyCode.RightArrow)) changeSteer(5);
            if (Input.GetKeyDown(KeyCode.UpArrow)) addVelocity(1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) addVelocity(-1);
        }
        void changeSteer(int val)
        {
            //Debug.Log("thingy pressed");
            if (!(bikedata[5] + val > 45 || bikedata[5] + val < -45))
                bikedata[5] += val;
        }
        void addVelocity(int val)
        {
            if (!(bikedata[0] + val > 255 || bikedata[0] + val < 0))
                bikedata[0] += val;
        }
    }
}