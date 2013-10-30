

using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.Sockets;
    using System.Net;


namespace RemoteLoggerService
{
  
       /// <summary>
       /// TCP Logger Server
       /// </summary>
        public class TcpLoggerServer : ILoggerServer, IDisposable
        {
            #region Properties 
            // Or whatever the name.
            const int MaxLengthOfPendingConnectionsQueue = 1000;

            /// <summary>
            /// The port number
            /// </summary>
            private int portNumber;
            /// <summary>
            /// The connections limit
            /// </summary>
            private int connectionsLimit;
            /// <summary>
            /// The connection socket
            /// </summary>
            private Socket connectionSocket;
            /// <summary>
            /// The connected clients list
            /// </summary>
            private Dictionary<string, LogClient> connectedClients = new Dictionary<string, LogClient>();

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
            #endregion

            #region Constructors
            public TcpLoggerServer(int portNumber, int connectionsLimit = 0)
            {
                this.portNumber = portNumber;
                this.connectionsLimit = connectionsLimit;
                
            }
            #endregion
         
            #region Send Messages
            /// <summary>
            /// Sends the message.
            /// </summary>
            /// <param name="messageToSend">The message to send.</param>
            /// <param name="clientID">The client ID.</param>
            public void SendMessage(string messageToSend, string clientID)
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(messageToSend + "\0");
                SendMessage(data, clientID);
            }

            /// <summary>
            /// Sends the message.
            /// </summary>
            /// <param name="messageToSend">The message to send.</param>
            /// <param name="clientID">The client ID.</param>
            public void SendMessage(byte[] messageToSend, string clientID)
            {
                LogClient client = GetClient(clientID);
                if (client != null)
                {
                    try
                    {
                        if (client.ClientSocket.Connected)
                        {
                            client.ClientSocket.Send(messageToSend);
                        }
                    }
                    catch (SocketException)
                    {
                        // TODO: sending failed; disconnect from client, or?
                    }
                }
            }
            #endregion

            #region Connection and Listening
            /// <summary>
            /// Starts the server.
            /// </summary>
            public void StartServer()
            {
                try
                {
                    // Create listening socket
                    connectionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    connectionSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, this.portNumber);
                    // Bind to local IP Address
                    connectionSocket.Bind(ipLocal);
                    // Start Listening
                    connectionSocket.Listen(MaxLengthOfPendingConnectionsQueue);
                    // Creat callback to handle client connections
                    connectionSocket.BeginAccept(new AsyncCallback(OnClientConnection), null);
                }
                catch (SocketException)
                {
                    // TODO: if we fail to start listening, is it even ok to continue?
                    // Consider that some of the bootstrapping actions might not even have been done.
                    // Thus execution will likely crash on next step.
                }
            }

            /// <summary>
            /// Stops the server.
            /// </summary>
            public void StopServer()
            {
                connectionSocket.Close();
            }

            /// <summary>
            /// Called when [client connection].
            /// </summary>
            /// <param name="asyn">The asyn.</param>
            private void OnClientConnection(IAsyncResult asyn)
            {
                try
                {
                    // Create a new StateObject to hold the connected client
                    LogClient connectedClient = new LogClient()
                    {
                        ClientSocket = connectionSocket.EndAccept(asyn),
                        ClientId = !connectedClients.Any() ? 1 : connectedClients.Count + 1
                    };

                    IPEndPoint remote = connectedClient.ClientSocket.RemoteEndPoint as IPEndPoint;
                    if (connectedClients.ContainsKey(remote.Address.ToString()))
                    {
                        closeSocket(connectedClient.UniqueAddress, true);
                        return;
                    }
                    connectedClients.Add(remote.Address.ToString(), connectedClient);

                    // TODO: consider if we can instead do this at the beginning of the method.
                    // Check against limit
                    if (connectedClients.Count > connectionsLimit)
                    {
                        // No connection event is sent so close socket silently
                        closeSocket(connectedClient.UniqueAddress, true);
                        return;
                    }

                    // Dispatch Event
                    if (ClientConnected != null)
                    {
                        SocketConnectArgs args = new SocketConnectArgs()
                        {
                            ClientAddress = IPAddress.Parse(((IPEndPoint)connectedClient.ClientSocket.RemoteEndPoint).Address.ToString()),
                            ClientId = connectedClient.ClientId
                        };
                        ClientConnected(this, args);
                    }

                    // Release connectionSocket to keep listening if limit is not reached
                    connectionSocket.BeginAccept(new AsyncCallback(OnClientConnection), null);

                    // Allow connected client to receive data and designate a callback method
                    connectedClient.ClientSocket.BeginReceive(connectedClient.buffer, 0, LogClient.BufferSize, 0, new AsyncCallback(OnDataReceived), connectedClient);
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
                            args.ClientAddress = dataSender.UniqueAddress;
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
                        closeSocket(dataSender.UniqueAddress);
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
            #endregion

            #region Socket Closing
            public void closeSocket(string socketID)
            {
                closeSocket(socketID, false);
            }

            /// <param name="silent">Whether to skip dispatching the disconnection event. Used to cancel the bootstrapping of the client-server connection.</param>
            private void closeSocket(string socketID, bool silent)
            {
                LogClient client = GetClient(socketID);
                if (client == null)
                {
                    return;
                }
                try
                {
                    client.ClientSocket.Close();
                    client.ClientSocket.Dispose();

                    if (!silent)
                    {
                        // Dispatch Event
                        if (ClientDisconnected != null)
                        {
                            SocketEventArgs args = new SocketEventArgs();
                            args.ClientId = client.ClientId;
                            args.ClientAddress = client.UniqueAddress;
                            ClientDisconnected(this, args);
                        }
                    }
                    // Moved to finnaly block: connectedClients.Remove(client.id);
                }
                catch (SocketException)
                {
                    // Don't care. Or?
                }
                finally
                {
                    connectedClients.Remove(client.UniqueAddress);
                }
            }

            public void closeAllSockets()
            {
                var keys = connectedClients.Keys;
                foreach (string key in keys)
                {
                    var client = connectedClients[key];
                    closeSocket(client.UniqueAddress);
                }
            }

            /// <summary>
            /// Gets the client.
            /// </summary>
            /// <param name="address">The address.</param>
            /// <returns></returns>
            private LogClient GetClient(string address)
            {
                LogClient client;
                if (!connectedClients.TryGetValue(address, out client))
                {
                    return null;
                }
                return client;
            }

            #endregion

            #region Dispose

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
            #endregion
        }
    }


