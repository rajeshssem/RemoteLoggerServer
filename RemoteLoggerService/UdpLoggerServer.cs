

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RemoteLoggerService
{
    public class UdpLoggerServer : ILoggerServer, IDisposable
    {


        /// <summary>
        /// The connection socket
        /// </summary>
        private Socket connectionSocket;
        private int connectedClients;
        private int portNumber;

        /// <summary>
        /// Occurs when [client connected].
        /// </summary>
        public event SocketConnectedHandler ClientConnected;
        /// <summary>
        /// Occurs when [client disconnected].
        /// </summary>
        public event SocketClosedHandler ClientDisconnected;
        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        public event SocketMessageReceivedHandler MessageReceived;

        public void StartServer()
        {
            try
            {
                //We are using UDP sockets
                connectionSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);

                //Assign the any IP of the machine and listen on port number 1000
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1000);

                //Bind this address to the server
                connectionSocket.Bind(ipEndPoint);
                

                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                //The epSender identifies the incoming clients
                EndPoint epSender = (EndPoint)ipeSender;

                //Start receiving data
               
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// Called when [data received].
        /// </summary>
        /// <param name="asyn">The asyn.</param>
        private void OnDataReceived(IAsyncResult asyn)
        {
            // Receive stateobject of the client that sent data
            LogClient dataSender = (LogClient)asyn.AsyncState;

            try
            {
                // Complete aysnc receive method and read data length
                int bytesRead = dataSender.ClientSocket.EndReceive(asyn);

                if (bytesRead > 0)
                {
                    // More data could be sent so append data received so far
                    dataSender.StrBuilder.Append(Encoding.UTF8.GetString(dataSender.buffer, 0, bytesRead));
                    if (dataSender.StrBuilder.Length != 0
                        && MessageReceived != null
                        )
                    {
                        // TODO: is it possible that multiple messages are in the sb?
                        // Consider whether it's necessary to replace with newline.
                        dataSender.StrBuilder.Replace("\0", null); // Removes them.

                        // Dispatch Event
                        SocketMessageReceivedArgs args = new SocketMessageReceivedArgs();
                        args.Message = dataSender.StrBuilder.ToString();
                        args.ClientId = dataSender.ClientId;

                        MessageReceived(this, args);

                        dataSender.StrBuilder.Clear();
                    }
                    try
                    {
                        dataSender.ClientSocket.BeginReceive(dataSender.buffer, 0, LogClient.BufferSize, 0, new AsyncCallback(this.OnDataReceived), dataSender);
                    }
                    catch (SocketException) { }
                }
                else
                {
                    
                    //closeSocket(dataSender.UniqueAddress);
                }
            }
            catch (SocketException)
            {
                // TODO: should we closeSocketSilent()? Or?
            }
            catch (ObjectDisposedException)
            {
                // TODO: should we closeSocketSilent()? Or?
            }
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ClientConnected = null;
            ClientDisconnected = null;
            MessageReceived = null;

            connectionSocket.Close();
        }

        public void StopServer()
        {
            throw new System.NotImplementedException();
        }

        public void CloseAllSockets()
        {
            throw new System.NotImplementedException();
        }

      

    }
}