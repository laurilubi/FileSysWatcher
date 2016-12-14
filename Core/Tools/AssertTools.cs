using System;
using Core.Exceptions;
using JetBrains.Annotations;

namespace Core.Tools
{
    public static class AssertTools
    {
        [ContractAnnotation("halt <= condition: false")]
        public static void Assert(bool condition)
        {
            if (condition) return;
            throw new Exception();
        }

        [ContractAnnotation("halt <= condition: false")]
        public static void Assert(bool condition, string message)
        {
            if (condition) return;
            throw new Exception(message);
        }

        [ContractAnnotation("halt <= condition: false")]
        public static void Assert<TException>(bool condition, string message = "") where TException : Exception, new()
        {
            if (condition) return;
            var exception = new TException();
            exception.SetFieldOrPropertyValue("_message", message);
            throw exception;
        }

        /// <param name="field">Field is only used for ModelValidationException</param>
        public static TException CreateException<TException>(string message, string field = null) where TException : Exception, new()
        {
            string type;
            var exception = new TException();
            exception.SetFieldOrPropertyValue("_message", message);
            var mve = exception as ModelValidationException;
            if (mve != null && field != null)
                mve.Field = field;

            return exception;
        }
    }
}
