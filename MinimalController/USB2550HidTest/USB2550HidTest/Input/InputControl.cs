using System;
using UsbHid;
using UsbHid.USB.Classes.Messaging;
using System.IO.Ports;
using System.Diagnostics;
using System.Management;


namespace USB2550HidTest.Input
{
    /** InputControl contains the code to connect to the Arduino and the praxtour bike 
     * and to collect the data from both devices at a predefined frequency. 
     * TODO: fix left brake, then remove compensateForBrokenLeftBrake()
     * **/
    class InputControl
    {
        public UsbHidDevice Device; //praxtour
        private SerialPort _serialport; //Arduino

        //datafields: SPD LBR RBR GEAR CAD STR
        private int[] bikeData = { 0, 0, 0, 0, 0, 0 };

        //index of bike parameters in the data byte arrays from the praxtour bike:
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
            // Connect to the praxtour bike
            Console.WriteLine("Initializing praxtour...");
            InitializePraxtour();
            Console.WriteLine("Successfully connected praxtour!");

            // Connect to Arduino
            Console.WriteLine("Connecting Arduino");
            ArduinoConnected = ConnectArduino();

            //for timing:
            stopwatch.Start();
        }
        private bool ConnectArduino()
        {
            _serialport = new SerialPort();
            bool success = false;

            string portname = AutodetectArduinoPort;
            Console.WriteLine("Trying to connect Arduino on " + portname);

            if (portname != null)
            {
                try
                {
                    _serialport.PortName = portname;
                    _serialport.BaudRate = 115200;
                    _serialport.Open();
                    Console.WriteLine("Connected Arduino on " + portname);
                    success = true; //if it gets here it is a success
                }
                catch
                {
                    Console.WriteLine("Failed to connect Arduino on " + portname);
                }
            }

            if (!success) Console.WriteLine("Failure: Arduino not connected");
            return success;
        }

        private string AutodetectArduinoPort
        {
            get
            {
                ManagementScope connectionScope = new ManagementScope();
                SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

                try
                {
                    foreach (ManagementObject item in searcher.Get())
                    {
                        string desc = item["Description"].ToString();
                        string deviceId = item["DeviceID"].ToString();

                        if (desc.Contains("Arduino"))
                        {
                            Console.WriteLine("Arduino detected on " + deviceId);
                            return deviceId;
                        }
                    }
                }
                catch (ManagementException e)
                {
                    /* Do Nothing */
                }

                return null;
            }
        }

        // Read the steering rotation from the Arduino
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
                    Console.WriteLine("An error occured reading data from the Arduino...");
                }
            }
        }
        private void InitializePraxtour()
        {
            Device = new UsbHidDevice(0x1f34, 0x1000, 0);
            Device.OnConnected += DeviceOnConnected;
            Device.OnDisConnected += DeviceOnDisConnected;
            Device.DataReceived += DeviceDataReceived;

            Device.Connect();
        }

        public void setFrequency(float fraction)
        {
            timeStop = Convert.ToInt64(1000 * fraction);
        }

        public string getStringifiedData()
        {
            // 0: speed
            // 1: left brake
            // 2: right brake
            // 3: gear
            // 4: cadance
            // 5: steering

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

            Console.WriteLine(outdata);
            return outdata;
        }

        public void counterPressure(string data)
        {
            int pct = Convert.ToInt16(data);
            Console.WriteLine("Sending counter pressure to praxtour bike: " + data);
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

        // Extract the bike parameters from the received byte array, excluding steering rotation (which is read from the Arduino).
        private void DeviceDataReceived(byte[] data)
        {
            int[] praxTour = ByteArrayToData(data);
            for (int i = 0; i < 5; i++) { bikeData[i] = praxTour[i]; }

            // Double the intensity of the right brake to compensate for the unfunctional left brake
            // TODO: remove this function when left brake is fixed
            compensateForBrokenLeftBrake();
        }

        // Get each bike parameter from the data byte array using their predefined index within the array
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

        public void compensateForBrokenLeftBrake()
        {
            // left brake intensity
            bikeData[1] = 0;
            // right brake intensity
            bikeData[2] *= 2;
        }
    }
}


