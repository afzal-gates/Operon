using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Application.Common.ViewModels
{
    public class SuccessResponseViewModel : ResponseViewModel
    {
        public SuccessResponseViewModel(string message = "success", HttpStatusCode code = HttpStatusCode.OK)
        {
            Status = new StatusViewModel
            {
                Code = code,
                Message = message
            };
        }
    }
}
