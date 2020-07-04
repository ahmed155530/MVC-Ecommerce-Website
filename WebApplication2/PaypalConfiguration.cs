using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebApplication2
{
    public class PaypalConfiguration
    {
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        static PaypalConfiguration()
        {
            var config = GetConfig();
            ClientId = "AZhAjuCn2bxyjCogK3pmQc2uc1F_pBWEmEPNmaQ60qLjnR2Xh5KnptxfmI-iimuArjtduWKgFta8OP1T";
            ClientSecret = "EEhgtQgNDFf9Mwy5eJPNebqLEA3OwxtN45tM5Udw8pCPHYYxRPFQ3Q9lNTluB3GlMqRnoZMvKYPyiEEi";
        }
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        private static string GetAccessToken()
        {
            // getting accesstocken from paypal
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext()
        {
            
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}