using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Autoexec
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if DEBUG
            args = new[] { "/console" };
#endif
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Initialise service");
            Console.ForegroundColor = ConsoleColor.Gray;

            var autoexecService = new AutoexecService();

            if (args.Length > 0)
            {
                string option = args[0].ToLower();

                switch (option)
                {
                    case "/console":
                        {
                            autoexecService.RunConsole();
                            return;
                        }
                }
            }

            ServiceBase[] servicesToRun = { autoexecService };
            ServiceBase.Run(servicesToRun);
        }
    }
}
