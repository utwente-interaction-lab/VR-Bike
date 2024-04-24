using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Pipes;
using System;

namespace Scripts
{
    public class BikeClient
    {
        private StreamReader reader;
        private StreamWriter writer;
        private NamedPipeClientStream client;
        private NamedPipeClientStream client2;

        public BikeClient()
        {
            client = new NamedPipeClientStream("bikepipe");
            client2 = new NamedPipeClientStream("bikepipe2");
            client.Connect();
            client2.Connect();
            reader = new StreamReader(client);
            writer = new StreamWriter(client2);
            //writer = new StreamWriter(client);
        }
        public string readData()
        {
            return reader.ReadLine();
        }
        public int[] readFields()
        {
            string[] lines = reader.ReadLine().Split(',');
            int[] fields = new int[lines.Length];
            //Debug.Log(lines.Length);
            for(int i = 0; i < lines.Length; i++)
            {
                //Debug.Log(lines[i]);
                fields[i] = Convert.ToInt16(lines[i]);
                //Debug.Log(fields[i]);
            }
            return fields;
        }
        public void sendData(string data)
        {
           writer.WriteLine(data);
           writer.Flush();
        }
        public void Disconnect()
        {
            client.Dispose();
        }
    }
}
