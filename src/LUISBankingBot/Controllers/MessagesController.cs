namespace LUISBankingBot
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Connector;
    using System.Diagnostics;
    using Microsoft.Bot.Builder.Dialogs;
    using LUISBankingBot.Dialogs;
    using LUISBankingBot.Models;   
    using System.IO;
    using LUISBankingBot.Services;
    using System.Net.Http.Headers;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly MicrosoftSpeechService speechService = new MicrosoftSpeechService();

        [Serializable]
        public class WelcomeDialog : IDialog<object>
        {
            public async Task StartAsync(IDialogContext context)
            {
                await context.PostAsync("Hi there! I'm your banking assistant.");
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
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        {
                            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                            Activity reply = activity.CreateReply("Working on that for you...");

                            await connector.Conversations.ReplyToActivityAsync(reply);
                            await Conversation.SendAsync(activity, () => new LuisBankingDialog.BankingDialog());

                            // speech to text
                            string message = "";
                            try
                            {
                                var audioAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Equals("audio/wav") || a.ContentType.Equals("application/octet-stream"));
                                if (audioAttachment != null)
                                {
                                    var stream = await GetImageStream(connector, audioAttachment);
                                    var text = await this.speechService.GetTextFromAudioAsync(stream);
                                    message = ProcessText(activity.Text, text);
                                }
                                else
                                {
                                    message = "I only support wav audio files at the moment. Try to upload anohter file with this format.";
                                }
                            }
                            catch (Exception e)
                            {
                                message = "Oops! Something went wrong. Try again later.";

                                Trace.TraceError(e.ToString());
                            }

                            reply = activity.CreateReply(message);
                            await connector.Conversations.ReplyToActivityAsync(reply);
                        }
                        break;


                    case ActivityTypes.ConversationUpdate:
                        {
                            break;
                        }
                    case ActivityTypes.ContactRelationUpdate:
                        {
                            break;
                        }
                    case ActivityTypes.Typing:
                        {
                            break;
                        }
                    case ActivityTypes.DeleteUserData:
                        {
                            break;
                        }
                    default:
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        // audio file text to speech helper
        private static string ProcessText(string input, string text)
        {
            string message = "You said : " + text + ".";

            input = input?.Trim();

            if (!string.IsNullOrEmpty(input))
            {
                var normalizedInput = input.ToUpper();

                if (normalizedInput.Equals("WORD"))
                {
                    var wordCount = text.Split(' ').Count(x => !string.IsNullOrEmpty(x));
                    message += " Word Count: " + wordCount;
                }
                else if (normalizedInput.Equals("CHARACTER"))
                {
                    var characterCount = text.Count(c => c != ' ');
                    message += " Character Count: " + characterCount;
                }
                else if (normalizedInput.Equals("SPACE"))
                {
                    var spaceCount = text.Count(c => c == ' ');
                    message += " Space Count: " + spaceCount;
                }
                else if (normalizedInput.Equals("VOWEL"))
                {
                    var vowelCount = text.ToUpper().Count("AEIOU".Contains);
                    message += " Vowel Count: " + vowelCount;
                }
                else
                {
                    var keywordCount = text.ToUpper().Split(' ').Count(w => w == normalizedInput);
                    message += " Keyword " + input + " found " + keywordCount + " times.";
                }
            }

            return message;
        }

        private static async Task<Stream> GetImageStream(ConnectorClient connector, Attachment imageAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(imageAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }
                else
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(imageAttachment.ContentType));
                }

                return await httpClient.GetStreamAsync(uri);
            }
        }

        /// <summary>
        /// Gets the JwT token of the bot. 
        /// </summary>
        /// <param name="connector"></param>
        /// <returns>JwT token of the bot</returns>
        private static async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
        }
    }
}