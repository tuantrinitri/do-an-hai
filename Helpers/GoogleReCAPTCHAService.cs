using CMS.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CMS.Helpers
{
    public class GoogleReCAPTCHAService
    {
        private readonly ReCAPTCHASettings_V2 _settings_v2;
        private readonly ReCAPTCHASettings_V3 _settings_v3;
        public double score = 0.5;
        public double timespan = 3;

        public GoogleReCAPTCHAService(IOptions<ReCAPTCHASettings_V3> settings_v3, IOptions<ReCAPTCHASettings_V2> settings_v2)
        {
            _settings_v2 = settings_v2.Value;
            _settings_v3 = settings_v3.Value;
        }
        /// <summary>
        /// Verify request token
        /// </summary>
        /// <param name="token">Google response token</param>
        /// <param name="version">Version of reCAPTCHA (default:version 3)</param>
        /// <param name="ip">Remote IP address (optional)</param>
        /// <returns></returns>
        public virtual async Task<GoogleResponse> VerifyCaptchaAsync(string token, int version = 3, string ip = "")
        {
            ReCAPTCHAData reCAPTCHAData = new ReCAPTCHAData
            {
                response = token,
                secret = version == 3 ? _settings_v3.ReCAPTCHA_Secret_Key : _settings_v2.ReCAPTCHA_Secret_Key,
                remoteip = ip
            };
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCAPTCHAData.secret}&response={reCAPTCHAData.response}");
            var responeToReturn = JsonConvert.DeserializeObject<GoogleResponse>(response);
            return responeToReturn;
        }
    }
}
