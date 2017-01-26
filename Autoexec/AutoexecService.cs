using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Core;

namespace Autoexec
{
    public partial class AutoexecService : ServiceBase
    {
        private Thread _thread;
        private bool _shutdownEvent;

        public AutoexecService()
        {
            InitializeComponent();
            SetupLogger();
        }

        public void RunConsole()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Starting service");
            Console.ForegroundColor = ConsoleColor.Gray;

            OnStart(null);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press enter to stop the service");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.ReadLine();
            OnStop();
        }

        private void SetupLogger()
        {
            var path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logging.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        //private UnityContainer SetupServiceLocator()
        //{
        //    var container = new UnityContainer();

        //    var locator = new UnityServiceLocator(container);
        //    ServiceLocator.SetLocatorProvider(() => locator);

        //    return container;
        //}

        /// <summary>When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.</summary>
        /// <param name="args">Data passed by the start command. </param>
        protected override void OnStart(string[] args)
        {
            try
            {
                //var container = SetupServiceLocator();
                //Execute
                ExecuteExecutable(false);
                _thread = new Thread(WorkerThreadFunc)
                {
                    Name = "Running thread",
                    IsBackground = true
                };
                _thread.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.Gray;
                LogManager.GetLogger("AutoexecAppender").Logger.Log(typeof(AutoexecService), Level.Error, "Coudn't initialize Autoexec", ex);
                throw;
            }
        }

        private void WorkerThreadFunc()
        {
            while (true)
            {
                if (_shutdownEvent) return;
            }
        }

        private void ExecuteExecutable(bool isStop)
        {

            var fullPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.ExecutableFullpath);
            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = fullPath,
                //WindowStyle = ProcessWindowStyle.Hidden
            };
            if (isStop)
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ExecutableArgumentsOnStop))
                {
                    processStartInfo.Arguments = Properties.Settings.Default.ExecutableArgumentsOnStop;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ExecutableArgumentsOnStart))
                {

                    processStartInfo.Arguments = Properties.Settings.Default.ExecutableArgumentsOnStart;
                }
            }

            try
            {
                //using (var executableProcess = Process.Start(processStartInfo))
                //{
                //    executableProcess?.WaitForExit();
                //}
                Process.Start(processStartInfo);
                LogManager.GetLogger("AutoexecAppender").Logger.Log(typeof(AutoexecService), Level.Info, "Autoexec started properly", null);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("AutoexecAppender").Logger.Log(typeof(AutoexecService), Level.Fatal, "Exception happened", ex);
            }
        }



        protected override void OnStop()
        {
            try
            {
                ExecuteExecutable(true);
                _shutdownEvent = true;
                LogManager.GetLogger("AutoexecAppender").Logger.Log(typeof(AutoexecService), Level.Info, "Autoexec stopped properly", null);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.Gray;
                LogManager.GetLogger("AutoexecAppender").Logger.Log(typeof(AutoexecService), Level.Error, "Coudn't initialize Autoexec", ex);
                throw;
            }
        }
    }
}
