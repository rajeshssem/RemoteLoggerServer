using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace BallyTech.LoggerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            LoggerService service = new LoggerService(); 

            if (Environment.UserInteractive)
            {
                service.StartLogger();
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                Console.Read();
               // service.Stop();
            }
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			{ 
				service
			};
                ServiceBase.Run(ServicesToRun);
            }
            
        }
    }
}
