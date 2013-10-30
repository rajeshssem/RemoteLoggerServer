// ************************************************************************************
// <copyright file="Log.cs" company="Bally Technologies">
//      Copyright (C) 2013  Bally Technologies Inc.
// </copyright>
// <author>Rajesh Subramanian</author>

// <email>RSubramanian2@ballytech.com</email>

// <datetime>5/7/2013 3:44:25 PM</datetime>
// ************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using NLog;

namespace BallyTech.LoggerService
{
    /// <summary>
    /// Logger class
    /// </summary>
    public  class Log
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        public   Log()
        {
            FileTarget target = new FileTarget();
            target.Layout = "${longdate} ${logger} ${message}";
            target.FileName = "${basedir}/logs/${logger}/Log_${logger}.txt";
            target.KeepFileOpen = true;
            target.Encoding = Encoding.UTF8;

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Debug);
        }

        /// <summary>
        /// Writes the specified client addres.
        /// </summary>
        /// <param name="clientAddres">The client addres.</param>
        /// <param name="message">The message.</param>
        public void Write(string clientAddres, string message)
        {
            Logger log = LogManager.GetLogger(clientAddres);
            log.Debug(message);
        }
    }
}
