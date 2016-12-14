using System.IO;
using System.Threading;

namespace FileSysWatcher
{
    public class DelayedEvent
    {
        public FileSystemEventArgs FileSystemEvent { get; set; }

        public Timer Timer { get; set; }
    }
}
