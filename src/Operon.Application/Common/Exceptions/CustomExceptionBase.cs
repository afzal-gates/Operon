using Operon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Application.Common.Exceptions
{
    public abstract class CustomExceptionBase : Exception
    {
        public CustomExceptionViewModel _exception = null;
        public CustomExceptionBase(CustomExceptionViewModel exception) : base(exception.Status.Message)
        {
            _exception = exception;
        }

        public abstract CustomExceptionViewModel EmitResult();

    }
}
