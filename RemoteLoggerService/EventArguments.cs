

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RemoteLoggerService
{
    /// <summary>
    /// Socket Connect Event Args
    /// </summary>
    public class SocketConnectArgs : SocketEventArgs
    {
        public IPAddress ClientAddress { get; set; }

    }

    /// <summary>
    /// Message Object when new message received from client
    /// </summary>
    public class SocketMessageReceivedArgs : SocketEventArgs
    {

        public string Message { get; set; }
        
    }

    /// <summary>
    /// Socket Events
    /// </summary>
    public class SocketEventArgs
    {

        public string ClientAddress { get; set; }

        public int ClientId { get; set; }

    }
}
