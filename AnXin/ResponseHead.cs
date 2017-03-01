using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnXinSdk
{
    [Serializable]
    public class ResponseHead
    {
        public string responseCode { get; set; }
        public string requestType { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string esbCode { get; set; }
        public string esbMessage { get; set; }
        public string signData { get; set; }
    }
}
