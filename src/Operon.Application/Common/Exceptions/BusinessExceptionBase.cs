using Operon.Application.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Application.Common.Exceptions
{
    public abstract class BusinessExceptionBase : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public BusinessExceptionBase(HttpStatusCode statusCode) : this(string.Empty, statusCode)
        { }

        public BusinessExceptionBase(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public abstract FailResponseViewModel EmitResult(string message);

    }
}
