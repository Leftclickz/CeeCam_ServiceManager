using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace CeeCam_ServiceManager
{
    public partial class CeeCam_ServiceManager : ServiceBase
    {
        System.Timers.Timer serviceRunningTimer;
        private int eventId = 1;
        private bool init = false;

        public enum CeeCam_ServiceManagerServiceCommands
        { StartService = 128 };

        public CeeCam_ServiceManager()
        {
            InitializeComponent();

            //initialize event log
            if (!System.Diagnostics.EventLog.SourceExists("CeeCam_ServiceManager"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "CeeCam_ServiceManager", "Application");
            }
            eventLog1.Source = "CeeCam_ServiceManager";
            eventLog1.Log = "Application";

            //initialize timer
            serviceRunningTimer = new System.Timers.Timer();
            serviceRunningTimer.Elapsed += StopManagedService;
            serviceRunningTimer.Stop();       
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);

            switch ((CeeCam_ServiceManagerServiceCommands)command)
            {
                case CeeCam_ServiceManagerServiceCommands.StartService:
                    serviceRunningTimer.Stop();
                    StartManagedService();
                    serviceRunningTimer.Start();
                    break;
                default:
                    break;
            }
        }

        protected override void OnStart(string[] args)
        {
            string serviceName = "";
            int interval = -1;

            if (args.Length != 4)
            {
                Environment.FailFast("Configuration is wrong.");
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--Service")
                {
                    if (i < args.Length - 1)
                    {
                        serviceName = args[i + 1];
                        i++;
                    }
                    else
                    {
                        Environment.FailFast("Configuration is wrong.");
                        return;
                    }
                }
                else if (args[i] == "--Interval")
                {
                    if (i < args.Length - 1)
                    {
                        bool result = int.TryParse(args[i + 1], out interval);
                        if (!result)
                        {
                            Environment.FailFast("Configuration is wrong.");
                            return;
                        }

                        i++;
                    }
                    else
                    {
                        Environment.FailFast("Configuration is wrong.");
                        return;
                    }
                }
            }

            if (interval == -1 || serviceName == "")
            {
                Environment.FailFast("Configuration is wrong.");
                return;
            }

            //initialize timer interval
            serviceRunningTimer.Interval = interval;

            //initialize service controller
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController i in scServices)
            {
                if (i.ServiceName == serviceName)
                {
                    serviceController1.ServiceName = i.ServiceName;

                    eventLog1.WriteEntry("Status = " + serviceController1.Status, EventLogEntryType.Information);
                    eventLog1.WriteEntry("Can Pause and Continue = " + serviceController1.CanPauseAndContinue, EventLogEntryType.Information);
                    eventLog1.WriteEntry("Can ShutDown = " + serviceController1.CanShutdown, EventLogEntryType.Information);
                    eventLog1.WriteEntry("Can Stop = " + serviceController1.CanStop, EventLogEntryType.Information);

                    init = true;
                    break;
                }
            }

            if (!init)
            {
                Environment.FailFast("Configuration is wrong.");
                return;
            }
        }

        protected override void OnStop()
        {
            StopManagedService(this, new EventArgs());
            serviceRunningTimer.Stop();
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

        private void StartManagedService()
        {
            if (init && serviceController1.Status == ServiceControllerStatus.Stopped)
            {
                eventLog1.WriteEntry("Starting " + serviceController1.ServiceName, EventLogEntryType.Information, eventId++);

                serviceController1.Start();
                while (serviceController1.Status == ServiceControllerStatus.StartPending || serviceController1.Status == ServiceControllerStatus.Stopped)
                {
                    Thread.Sleep(1000);
                    serviceController1.Refresh();
                }

                eventLog1.WriteEntry("Started " + serviceController1.ServiceName, EventLogEntryType.Information, eventId++);
            }
        }

        public void StopManagedService(object sender, EventArgs args)
        {
            if (init && serviceController1.Status == ServiceControllerStatus.Running)
            {
                eventLog1.WriteEntry("Stopping " + serviceController1.ServiceName, EventLogEntryType.Information, eventId++);

                serviceController1.Stop();
                while (serviceController1.Status == ServiceControllerStatus.StopPending || serviceController1.Status == ServiceControllerStatus.Running)
                {
                    Thread.Sleep(1000);
                    serviceController1.Refresh();
                }

                eventLog1.WriteEntry("Stopped " + serviceController1.ServiceName, EventLogEntryType.Information, eventId++);
            }
        }
    }
}
