// ************************************************************************************
// <copyright file="EventArguments.cs" company="Bally Technologies">
//      Copyright (C) 2013  Bally Technologies Inc.
// </copyright>
// <author>Rajesh Subramanian</author>

// <email>RSubramanian2@ballytech.com</email>

// <datetime>5/6/2013 4:17:55 PM</datetime>
// ************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BallyTech.LoggerService
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
