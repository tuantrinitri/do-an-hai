using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Models
{
    public class ReCAPTCHAData
    {
        public string secret { get; set; }
        public string response { get; set; }
        public string remoteip { get; set; }
    }

    public class GoogleResponse
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        public double score { get; set; }
        public string[] error_codes { get; set; }
        public string action { get; set; }
    }
}
