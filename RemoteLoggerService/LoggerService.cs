
// ************************************************************************************
// Windows service to handle the logs from the client
// ************************************************************************************


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;

namespace RemoteLoggerService
{
    public partial class LoggerService : ServiceBase
    {


        /// <summary>
        /// The server instance
        /// </summary>
        ILoggerServer Server = null;
        /// <summary>
        /// The connected clients dictinary and its handler
        /// </summary>
        Dictionary<string, LogMessageHandler> ConnectedClients = null;

        Log LogWriter = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerService"/> class.
        /// </summary>
        public LoggerService()
        {
            InitializeComponent();
            Server = new TcpLoggerServer(4000);
            ConnectedClients = new Dictionary<string, LogMessageHandler>();
            LogWriter = new Log();
        }


        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        /// <summary>
        /// Starts the logger server.
        /// </summary>
        public void StartLogger()
        {
            Server = new TcpLoggerServer(4000, 2000);
            ConnectedClients = new Dictionary<string, LogMessageHandler>();
            Server.ClientConnected += new SocketConnectedHandler(Server_ClientConnected);
            Server.ClientDisconnected += new SocketClosedHandler(Server_ClientDisconnected);
            Server.MessageReceived += new SocketMessageReceivedHandler(Server_MessageReceived);
            Server.StartServer();
            LogWriter.Write(ServiceName, "Starting Service at Port 4000");
        }

        /// <summary>
        /// Server_s the message received.
        /// </summary>
        /// <param name="socketServer">The socket server.</param>
        /// <param name="e">The e.</param>
        void Server_MessageReceived(ILoggerServer socketServer, SocketMessageReceivedArgs e)
        {
            ConnectedClients[e.ClientAddress](e.ClientAddress, e.Message);
        }

        /// <summary>
        /// Server_s the client disconnected.
        /// </summary>
        /// <param name="socketServer">The socket server.</param>
        /// <param name="e">The <see cref="SocketEventArgs"/> instance containing the event data.</param>
        void Server_ClientDisconnected(ILoggerServer socketServer, SocketEventArgs e)
        {
            ConnectedClients.Remove(e.ClientAddress);
        }

        /// <summary>
        /// Server_s the client connected.
        /// </summary>
        /// <param name="socketServer">The socket server.</param>
        /// <param name="e">The e.</param>
        void Server_ClientConnected(ILoggerServer socketServer, SocketConnectArgs e)
        {
            LogMessageHandler handler = new LogMessageHandler(ProcessLogMessage);
            ConnectedClients.Add(e.ClientAddress.ToString(),handler);
        }

        /// <summary>
        /// Processes the log message.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="message">The message.</param>
        public  void ProcessLogMessage(string clientAddress, string message)
        {
            LogWriter.Write(clientAddress, message);
           
        }


    }
}
