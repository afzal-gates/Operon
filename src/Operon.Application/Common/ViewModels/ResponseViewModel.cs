using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Application.Common.ViewModels
{
    public abstract class ResponseViewModel
    {
        public dynamic Data { get; set; }
        public StatusViewModel Status { get; set; }
    }
}
