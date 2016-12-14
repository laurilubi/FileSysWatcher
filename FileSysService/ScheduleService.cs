using System;
using Core;

namespace FileSysService
{
    public class ScheduleService
    {
        private readonly TimeSpan minInternal = new TimeSpan(0, 2, 0);
        private DateTime lastRunTime;
        private readonly IExecutableService subService;

        public ScheduleService(IExecutableService subService)
        {
            this.subService = subService;
        }

        public DateTime CheckAndReschedule()
        {
            if (DateTime.Now < lastRunTime + minInternal)
                return lastRunTime + minInternal;

            subService.Execute();
            lastRunTime = DateTime.Now;
            return lastRunTime + minInternal;
        }
    }
}
