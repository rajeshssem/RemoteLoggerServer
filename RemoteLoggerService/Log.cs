

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using NLog;

namespace RemoteLoggerService
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
