


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RemoteLoggerService
{

    /// <summary>
    /// Instance for Each logger client
    /// </summary>
    public class LogClient
    {

        /// <summary>
        /// The socket
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Gets or sets the client socket.
        /// </summary>
        /// <value>
        /// The client socket.
        /// </value>
        public Socket ClientSocket
        {
            get { return socket; }
            set { socket = value; }
        }


        /// <summary>
        /// The client id
        /// </summary>
        private int clientId;

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public int ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }


        /// <summary>
        /// The buffer size
        /// </summary>
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder StrBuilder = new StringBuilder();

        /// <summary>
        /// Gets the unique address of the logger client .
        /// </summary>
        /// <value>
        /// The unique address.
        /// </value>
        public string UniqueAddress
        {
            get
            {

                return ((IPEndPoint)ClientSocket.RemoteEndPoint).Address.ToString();
            }

        }

    }
}
