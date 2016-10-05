using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.Bot.Builder.Dialogs;
using LUISBankingBot.Dialogs;
using System.Threading;
using System.Web;
using System.Text;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace LUISBankingBot
{
    [DataContract]
    public class AccessTokenInfo
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class Authentication
    {
        public static readonly string AccessUri = "https://oxford-speech.cloudapp.net/token/issueToken";
        private string clientId;
        private string clientSecret;
        private string request;
        private AccessTokenInfo token; // 774daf9ffd514e7dafeb592298812690
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public Authentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}",
                                              HttpUtility.UrlEncode(clientId),
                                              HttpUtility.UrlEncode(clientSecret),
                                              HttpUtility.UrlEncode("https://speech.platform.bing.com"));

            this.token = HttpPost(AccessUri, this.request);

            // renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }

        //Return the access token
        public AccessTokenInfo GetAccessToken()
        {
            return this.token;
        }

        //Renew the access token
        private void RenewAccessToken()
        {
            AccessTokenInfo newAccessToken = HttpPost(AccessUri, this.request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}",
                              this.clientId,
                              this.token.access_token));
        }
        //Call-back when we determine the access token has expired 
        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        //Helper function to get new access token
        private AccessTokenInfo HttpPost(string accessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(accessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccessTokenInfo));
                //Get deserialized object from JSON stream
                AccessTokenInfo token = (AccessTokenInfo)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }    

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        // speech to text conversation

        private string DoSpeechReco(Attachment attachment)
        {
            AccessTokenInfo token;
            string headerValue;
            // Note: Sign up at https://microsoft.com/cognitive to get a subscription key.  
            // Use the subscription key as Client secret below.
            Authentication auth = new Authentication("YOURUSERID", "<YOUR API KEY FROM MICROSOFT.COM/COGNITIVE");
            string requestUri = "https://speech.platform.bing.com/recognize";

            //URI Params. Refer to the Speech API documentation for more information.
            requestUri += @"?scenarios=smd";                                // websearch is the other main option.
            requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";   // You must use this ID.
            requestUri += @"&locale=en-US";                                 // read docs, for other supported languages. 
            requestUri += @"&device.os=wp7";
            requestUri += @"&version=3.0";
            requestUri += @"&format=json";
            requestUri += @"&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
            requestUri += @"&requestid=" + Guid.NewGuid().ToString();

            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
            var wav = HttpWebRequest.Create(attachment.ContentUrl);
            string responseString = string.Empty;

            try
            {
                token = auth.GetAccessToken();
                Console.WriteLine("Token: {0}\n", token.access_token);

                //Create a header with the access_token property of the returned token
                headerValue = "Bearer " + token.access_token;
                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);

                HttpWebRequest request = null;
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;
                request.Host = host;
                request.ContentType = contentType;
                request.Headers["Authorization"] = headerValue;

                using (Stream wavStream = wav.GetResponse().GetResponseStream())
                {
                    byte[] buffer = null;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        int count = 0;
                        do
                        {
                            buffer = new byte[1024];
                            count = wavStream.Read(buffer, 0, 1024);
                            requestStream.Write(buffer, 0, count);
                        } while (wavStream.CanRead && count > 0);
                        // Flush
                        requestStream.Flush();
                    }
                    //Get the response from the service.
                    Console.WriteLine("Response:");
                    using (WebResponse response = request.GetResponse())
                    {
                        Console.WriteLine(((HttpWebResponse)response).StatusCode);
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseString = sr.ReadToEnd();
                        }
                        Console.WriteLine(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
            }
            dynamic data = JObject.Parse(responseString);
            return data.header.name;
        }

        [Serializable]
        public class WelcomeDialog : IDialog<object>
        {
            public async Task StartAsync(IDialogContext context)
            {
                await context.PostAsync("Hi there! I'm your banking assistant.");
            }
        }

        // https://api.projectoxford.ai/luis/v1/application?id=6841d389-70d6-45ec-96a9-a2893d1c778e&subscription-key=5d7817feda724399aaf69441f3fb18eb&q={PUT_QUERY_TEXT_HERE}
        [Serializable]
        public class EchoDialog : IDialog<object>
        {
            public async Task StartAsync(IDialogContext context)
            {
                context.Wait(MessageReceivedAsync);
            }

            public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var message = await argument;
                await context.PostAsync("Completed account lookup for " + message.Text + ". Please enter another TE.");
                context.Wait(MessageReceivedAsync);
            }
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity != null)
            {
                // one of these will have an interface and process it
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        {
                            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                            Activity reply = activity.CreateReply("Working on that for you...");

                            await connector.Conversations.ReplyToActivityAsync(reply);
                            await Conversation.SendAsync(activity, () => new LuisBankingDialog.BankingDialog());

                            // text to speech
                            ConnectorClient connector2 = new ConnectorClient(new Uri(activity.ServiceUrl));

                            var text = activity.Text;

                                if (activity.Attachments.Any())
                                {
                                    var reco = DoSpeechReco(activity.Attachments.First());

                                    if (activity.Text.ToUpper().Contains("WORD"))
                                    {
                                        text = "You said : " + reco + " Word Count: " + reco.Split(' ').Count();
                                    }
                                    else if (activity.Text.ToUpper().Contains("CHARACTER"))
                                    {
                                        var nospacereco = reco.ToCharArray().Where(c => c != ' ').Count();
                                        text = "You said : " + reco + " Character Count: " + nospacereco;
                                    }
                                    else if (activity.Text.ToUpper().Contains("SPACE"))
                                    {
                                        var spacereco = reco.ToCharArray().Where(c => c == ' ').Count();
                                        text = "You said : " + reco + " Space Count: " + spacereco;
                                    }
                                    else if (activity.Text.ToUpper().Contains("VOWEL"))
                                    {
                                        var vowelreco = reco.ToUpper().ToCharArray().Where(c => c == 'A' || c == 'E' ||
                                                                                           c == 'O' || c == 'I' || c == 'U').Count();
                                        text = "You said : " + reco + " Vowel Count: " + vowelreco;
                                    }
                                    else if (!String.IsNullOrEmpty(activity.Text))
                                    {
                                        var keywordreco = reco.ToUpper().Split(' ').Where(w => w == activity.Text.ToUpper()).Count();
                                        text = "You said : " + reco + " Keyword " + activity.Text + " found " + keywordreco + " times.";
                                    }
                                    else
                                    {
                                        text = "You said : " + reco;
                                    }
                                }
                                Activity reply2 = activity.CreateReply(text);
                                await connector2.Conversations.ReplyToActivityAsync(reply2);
                        }
                        break;

                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }
            else
            {

            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}