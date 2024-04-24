using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Scripts
{

    public class BikeSocketClient
    {
        private WebSocket ws;
        private bool inboxFull = false;
        private string mail = "";
        private bool connected = false;

        public BikeSocketClient()
        {
            ws = new WebSocket("ws://localhost:5000/getData");

            ws.OnMessage += OnMessage;

            ws.Connect();

        }
        public void sendMessage(string msg)
        {
            ws.Send(msg);
        }

        public bool hasMessage()
        {
            return inboxFull;
        }
        public string getMessage()
        {
            inboxFull = false;
            return mail;
        }
        public int[] parseData()
        {
            string raw = getMessage();
            string[] lines = raw.Split(',');
            int[] outdata = new int[6];
            for (int i = 0; i < lines.Length; i++)
            {
                outdata[i] = Convert.ToInt16(lines[i]);
            }
            return outdata;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            inboxFull = true;
            mail = e.Data;
            //Debug.Log("data:" + e.Data);
        }
    }
}
