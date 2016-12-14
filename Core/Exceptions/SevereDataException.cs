using System;

namespace Core.Exceptions
{
    [Serializable]
    public class SevereDataException : Exception
    {
        public SevereDataException() { }

        public SevereDataException(string message) : base(message) { }

        public SevereDataException(string message, Exception exception) : base(message, exception) { }
    }
}
