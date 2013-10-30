

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteLoggerService
{

    /// <summary>
    /// When new connection
    /// </summary>
    /// <param name="socketServer">The socket server.</param>
    /// <param name="e">The e.</param>
    public delegate void SocketConnectedHandler(ILoggerServer socketServer, SocketConnectArgs e);

    /// <summary>
    /// Handler to call when new log message received
    /// </summary>
    /// <param name="socketServer">The socket server.</param>
    /// <param name="e">The e.</param>
    public delegate void SocketMessageReceivedHandler(ILoggerServer socketServer, SocketMessageReceivedArgs e);

    /// <summary>
    /// Handler to call when Socket Closed
    /// </summary>
    /// <param name="socketServer">The socket server.</param>
    /// <param name="e">The <see cref="SocketEventArgs"/> instance containing the event data.</param>
    public delegate void SocketClosedHandler(ILoggerServer socketServer, SocketEventArgs e);

    /// <summary>
    /// Handles the log messages
    /// </summary>
    /// <param name="clientAddress">The client address.</param>
    /// <param name="message">The message.</param>
    public delegate void LogMessageHandler(string clientAddress, string message);


    /// <summary>
    /// Concrete Logger Server type
    /// </summary>
    public interface ILoggerServer
    {

        /// <summary>
        /// Occurs when [client connected].
        /// </summary>
        event SocketConnectedHandler ClientConnected;

        /// <summary>
        /// Occurs when [client disconnected].
        /// </summary>
        event SocketClosedHandler ClientDisconnected;

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        event SocketMessageReceivedHandler MessageReceived;

        /// <summary>
        /// Starts the server.
        /// </summary>
        void StartServer();

        /// <summary>
        /// Stops the server.
        /// </summary>
        void StopServer();


    }
}
