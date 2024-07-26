using System;
using USB2550HidTest.Input;

namespace USB2550HidTest
{
    static class Program
    {
        [STAThread]
        
        static void Main()
        {
            // Used to send/receive data to/from the Unity project
            SocketServer server = new SocketServer();

            // Connects and starts reading the data from the Praxtour bike and the Arduino
            InputControl inputController = new InputControl();

            inputController.setFrequency(1 / 60); // 60hz data rate (this is fine)
            while (true)
            {
                //if (inputController.ArduinoConnected) inputController.checkArduino();

                if (inputController.TimePassed())
                {

                    if (inputController.ArduinoConnected) inputController.checkArduino();

                    server.sendData(inputController.getStringifiedData());
                   if (server.hasData())
                    {
                        Console.WriteLine("Received data from the SocketServer!");
                        // send counter pressure to the bike from the websocket client (Unity Project)
                        inputController.counterPressure(server.getData());
                   Console.WriteLine(server.getData());
                    }
                }
            }
        }
    }
        
}

