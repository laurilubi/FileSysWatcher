using System;
using System.Runtime.Serialization;

namespace Core.Exceptions
{
    [Serializable]
    public class ModelValidationException : Exception
    {
        public string Field { get; set; }

        public string Code { get; set; }

        public ModelValidationException() { }

        public ModelValidationException(string message) : base(message) { }

        public ModelValidationException(string code, string message)
            : base(message)
        {
            Code = code;
        }

        public ModelValidationException(string field, string code, string message)
            : this(code, message)
        {
            Field = field;
        }

        public ModelValidationException(string message, Exception exception) : base(message, exception) { }

        #region Serialization

        public ModelValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Field = (string)info.GetValue("Field", typeof(string));
            Code = (string)info.GetValue("Code", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Field", Field);
            info.AddValue("Code", Code);
        }

        #endregion
    }
}
