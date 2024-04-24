using System;
using USB2550HidTest.Input;

namespace USB2550HidTest
{
    static class Program
    {
        [STAThread]
        
        static void Main()
        {
            SocketServer server = new SocketServer();
            //Console.WriteLine("startup!");
            InputControl inputController = new InputControl();

            //inputController.checkArduino();

            //Console.WriteLine(inputController.getStringifiedData());

            inputController.setFrequency(1 / 60); // 60hz data rate (this is fine)
            while (true)
            {
                //Console.WriteLine("Hello world!");
                if (inputController.ArduinoConnected) inputController.checkArduino();
                //Console.WriteLine("Hello world 2!");
                if (inputController.TimePassed())
                {
                    Console.WriteLine(inputController.getStringifiedData());
                    server.sendData(inputController.getStringifiedData());
                   if (server.hasData())
                    {
                   // handle data
                       inputController.counterPressure(server.getData());
                   Console.WriteLine(server.getData());
                    }
                }
            }
        }
    }
        
}

