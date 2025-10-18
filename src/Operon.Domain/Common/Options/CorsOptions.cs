using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operon.Domain.Common.Options
{
    public class CorsOptions
    {
        public string[] Origins { get; set; }
        public string PolicyName { get; set; }
    }
}
