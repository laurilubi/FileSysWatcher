using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Exceptions
{
    public class ApiProtocolException : Exception
    {
        public ApiProtocolException(string code = "", string message = "")
            : base(message)
        {
            Code = code;
        }

        public ApiProtocolException()
            : this("", "")
        { }

        public string Code { get; set; }
    }
}
