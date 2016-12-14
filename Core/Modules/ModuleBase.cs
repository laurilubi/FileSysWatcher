using System;

namespace FileSysWatcher.Modules
{
    public abstract class ModuleBase : IModule
    {
        public const string PREFIX_PRECONDITION = "PreCondition";
        public const string PREFIX_MODULE = "Module";

        protected readonly Section section;
        protected readonly Settings settings;
        protected readonly string settingsPrefix;

        protected ModuleBase(Section section, Settings settings, string settingsPrefix)
        {
            this.section = section;
            this.settings = settings;
            this.settingsPrefix = settingsPrefix;
        }

        public abstract ExecuteCommandResult Execute(DelayedEvent delayedEvent);

        public string ReadModuleSetting(string key, bool allowMissing = false)
        {
            var fullKey = settingsPrefix == null
                ? key
                : settingsPrefix + key;
            string value = settings[fullKey];
            if (value == null && !allowMissing)
                throw new Exception(string.Format("Missing configuration value for key {0} for module {1} of section {2}.",
                    key, GetType().FullName, section.SectionName));
            return value;
        }
    }
}
