namespace LUISBankingBot.Services
{
    using LUISBankingBot.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class AuthenticationService
    {
        private static readonly object LockObject;
        private static readonly string ApiKey;
        private AccessToken token;
        private Timer timer;

        static AuthenticationService()
        {
            LockObject = new object();
            ApiKey = WebConfigurationManager.AppSettings["MicrosoftSpeechApiKey"];
        }

        private AuthenticationService()
        {

        }

        public static AuthenticationService Instance { get; } = new AuthenticationService();


        public AccessToken GetAccessToken()
        {
            if (this.token == null)
            {
                lock (LockObject)
                {
                    // This condition will be true only once in the lifetime of the application
                    if (this.token == null)
                    {
                        this.RefreshToken();
                    }
                }
            }

            return this.token;
        }

        private static AccessToken GetNewToken()
        {

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", ApiKey },
                    { "client_secret", ApiKey },
                    { "scope", "https://speech.platform.bing.com" }
                };

                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync("https://oxford-speech.cloudapp.net/token/issueToken", content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<AccessToken>(responseString);
            }
        }

        
        private void RefreshToken()
        {
            this.token = GetNewToken();
            this.timer?.Dispose();
            this.timer = new Timer(
                x => this.RefreshToken(),
                null,
                TimeSpan.FromSeconds(this.token.expires_in).Subtract(TimeSpan.FromMinutes(1)), // Specifies the delay before RefreshToken is invoked.
                TimeSpan.FromMilliseconds(-1)); // Indicates that this function will only run once     
        }
    }
}