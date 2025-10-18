using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Application.Common.ViewModels
{
    public class StatusViewModel
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
    }
}
