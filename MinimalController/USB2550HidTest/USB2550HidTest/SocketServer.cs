using System;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace USB2550HidTest
{

    public class SocketServer
    {
        private WebSocketServer server;
        private string mail = "";
        private bool hasmail = false;
        public SocketServer()
        {
            server = new WebSocketServer("ws://localhost:5000");
            server.AddWebSocketService("/getData", () => new DataSocket(this) { });
           
            server.Start();
            Console.WriteLine("Server started");
        }
        public void addData(string data)
        {
            mail = data;
            hasmail = true;
        }
        public void sendData(string data)
        {
            //override the override, send to all
            server.WebSocketServices["/getData"].Sessions.Broadcast(data);
        }
        public bool hasData()
        {
            return hasmail;
        }
        public string getData()
        {
            hasmail = false;
            return mail;
        }
    }

    public class DataSocket : WebSocketBehavior
    {
        private SocketServer _server;
        public DataSocket(SocketServer server)
        {
            _server = server;
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            _server.addData(e.Data);
        }
    }
}
