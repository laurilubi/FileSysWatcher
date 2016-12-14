using System.Collections.Generic;

namespace Core.Logging
{
    public abstract class BaseLogService
    {
        protected readonly List<List<string>> subscribers;

        public BaseLogService()
        {
            subscribers = new List<List<string>>();
        }

        protected void PublishToSubscribers(string line)
        {
            foreach (var subscriber in subscribers)
            {
                if (subscriber == null) continue;
                subscriber.Add(line);
            }
        }

        public void Register(List<string> report)
        {
            if (subscribers.Contains(report)) return;
            subscribers.Add(report);
        }

        public void Unregister(List<string> report)
        {
            if (!subscribers.Contains(report)) return;
            subscribers.Remove(report);
        }
    }
}
