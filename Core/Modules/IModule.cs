namespace FileSysWatcher.Modules
{
    public interface IModule
    {
        ExecuteCommandResult Execute(DelayedEvent delayedEvent);
    }
}
