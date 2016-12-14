using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Core.Services;
using FileSysService.CleanFolder;
using FileSysService.Logging;
using Timer = System.Timers.Timer;

namespace FileSysService
{
    public partial class Service1 : ServiceBase
    {
        private readonly TimeSpan defaultRetry = new TimeSpan(0, 0, 15);
        public CleanFolderService cleanFolderService;
        public ScheduleService scheduleService;
        public Timer scheduleTimer;
        private EventLogService logService;
        private ConfigService configService;

        public Service1()
        {
            InitializeComponent();
            ServiceName = "File-system watcher";
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = false;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //Thread.Sleep(15 * 1000);
                logService = new EventLogService(eventLog1);
                //configService = new ConfigService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml"));
                configService = new ConfigService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Lubi Datakonsult\\FileSysWatcher\\config.xml"));
                configService.OnConfigChanged += Reschedule;
                cleanFolderService = new CleanFolderService(logService,
                    configService.GetNode("/FileSysWatcher/CleanFolderService"));
                scheduleService = new ScheduleService(cleanFolderService);
                scheduleTimer = new Timer
                {
                    Interval = defaultRetry.TotalMilliseconds,
                    Enabled = true
                };
                scheduleTimer.Elapsed += ScheduleTimerElapsed;
            }
            catch (Exception ex)
            {
                logService?.ErrorLog(ex.ToString());
                Stop();
            }
        }

        protected override void OnStop()
        {
        }

        private void ScheduleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            scheduleTimer.Enabled = false;
            try
            {
                var nextCheck = scheduleService.CheckAndReschedule();
                var timeToNextCheck = nextCheck - DateTime.Now;
                scheduleTimer.Interval = Math.Max(timeToNextCheck.TotalMilliseconds, 1000);
            }
            catch (Exception ex)
            {
                logService.ErrorLog(ex.ToString());
                scheduleTimer.Interval = defaultRetry.TotalMilliseconds;
            }
            scheduleTimer.Enabled = false;
        }

        private void Reschedule(object sender, FileSystemEventArgs e)
        {
            logService?.ErrorLog("Config changed, rescheduling for checking folders now");
            scheduleTimer.Interval = 2000;
        }
    }
}
