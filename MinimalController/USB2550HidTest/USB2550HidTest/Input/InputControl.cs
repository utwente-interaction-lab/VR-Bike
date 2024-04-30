using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsbHid;
using UsbHid.USB.Classes.Messaging;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;


namespace USB2550HidTest.Input
{
    class InputControl
    {
        public UsbHidDevice Device; //praxtour
        private SerialPort _serialport; //Arduino

        //datafields: SPD LBR RBR GEAR CAD STR
        private int[] bikeData = { 0, 0, 0, 0, 0, 0};
        //bike parameters:
        int speedADR = 3;
        int leftbrakeADR = 6;
        int rightbrakeADR = 7;
        int gearADR = 4;
        int cadanceADR = 8;


        // debug variables:
        private Stopwatch stopwatch = new Stopwatch();
        private long timeStop = 2000;
        private long checkpoint = 0;
        private int pressure = 0; //for feedback
        //string lastcommand = null;

        public bool ArduinoConnected = false;


        public InputControl()
        {
            Console.WriteLine("Initializing praxtour...");
            InitializeComponent();
            Console.WriteLine("Successfull");

            //Connect the arduino
            Console.WriteLine("Connecting Arduino");
            ArduinoConnected = ConnectArduino();
            //for timing:
            stopwatch.Start();
        }
        private bool ConnectArduino()
        {
            _serialport = new SerialPort();
            bool success = false;

            for (int i = 1; i < 8; i++)
            {
                string portname = "COM" + i; // try all port combinations 1-7
                Console.WriteLine("Trying to connect Arduino on " + portname);
                try
                {
                    _serialport.PortName = portname;
                    _serialport.BaudRate = 115200;
                    _serialport.Open();
                    Console.WriteLine("Connected Arduino on " + portname);
                    success = true; //if it gets here it is a success
                    break;
                }
                catch
                {
                    Console.WriteLine("Faield to connect Arduino on " + portname);
                    // do nothing, next loop will try again
                }
            }

            if (!success) Console.WriteLine("Failure: Arduino not connected");
            return success;
        }
        
        public void checkArduino()
        {
            string line = _serialport.ReadLine();
            if (line != "")
            {
                try
                {
                    int val = Convert.ToInt16(line);
                    bikeData[5] = val;
                }
                catch
                {
                    Console.WriteLine("An error occured reading data...");
                }
                //Console.WriteLine(bikeData[5]);
            }
        }
        private void InitializeComponent()
        {
            Device = new UsbHidDevice(0x1f34, 0x1000, 0);
            Device.OnConnected += DeviceOnConnected;
            Device.OnDisConnected += DeviceOnDisConnected;
            Device.DataReceived += DeviceDataReceived;

            Device.Connect();
        }
        public int[] getData()
        { 
            return bikeData;
        }
        public void setFrequency(float fraction)
        {
            timeStop = Convert.ToInt64(1000 * fraction);
        }
        public string getStringifiedData()
        {
            // 0: speed 1: left brake 2: right brake 3: gear 4: cadance 5: steeringwheel
            string outdata = "";
            outdata += bikeData[0];
            outdata += ",";
            outdata += bikeData[1];
            outdata += ",";
            outdata += bikeData[2];
            outdata += ",";
            outdata += bikeData[3];
            outdata += ",";
            outdata += bikeData[4];
            outdata += ",";
            outdata += bikeData[5];
            //Console.WriteLine(bikeData[5]);
            //Console.WriteLine(outdata);
            return outdata;
            //return "hello this is a test";
        }
        
        public void counterPressure(string data)
        {
            int pct = Convert.ToInt16(data);
            Console.WriteLine(data);
            if (pct > 100) pct = 100; //limit to 100
            var command = new CommandMessage(0, new byte[] { Convert.ToByte(pct), 0, 0, 0, 0, 0, 0, 0 });
            Device.SendMessage(command);
        }
        public bool TimePassed()
        {
            if (stopwatch.ElapsedMilliseconds - timeStop > checkpoint)
            {
                checkpoint = stopwatch.ElapsedMilliseconds;
                return true;
            }
            return false;
        }
        private void DeviceOnConnected()
        {
            Console.WriteLine("bike connected");
        }
        private void DeviceOnDisConnected()
        {
            Console.WriteLine("bike disconnected");
        }
        private void DeviceDataReceived(byte[] data)
        {
            int[] praxTour = ByteArrayToData(data);
            for(int i = 0; i < 5; i++)
            {
                Console.Write(praxTour[i] +", ");
                bikeData[i] = praxTour[i];
            }
            //Console.WriteLine("");
        }
        private int[] ByteArrayToData(byte[] data)
        {

            int[] res = new int[5];
            if (data != null)
            {
                res[0] = Convert.ToInt16(data[speedADR]);
                res[1] = Convert.ToInt16(data[leftbrakeADR]);
                res[2] = Convert.ToInt16(data[rightbrakeADR]);
                res[3] = Convert.ToInt16(data[gearADR]);
                res[4] = Convert.ToInt16(data[cadanceADR]);
            }
            return res;
        }
    }
}


