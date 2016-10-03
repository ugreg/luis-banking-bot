using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Bot.Builder.Dialogs;

namespace LUISBankingBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        
        [Serializable]
        public class WelcomeDialog : IDialog<object>
        {
            public async Task StartAsync(IDialogContext context)
            {
                await context.PostAsync("Hi there! I'm your banking assistant.");
            }
        }

        // https://api.projectoxford.ai/luis/v1/application?id=6841d389-70d6-45ec-96a9-a2893d1c778e&subscription-key=5d7817feda724399aaf69441f3fb18eb&q={PUT_QUERY_TEXT_HERE}
        // [LuisModel("6841d389-70d6-45ec-96a9-a2893d1c778e", "5d7817feda724399aaf69441f3fb18eb")]
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
                            int length = (activity.Text ?? string.Empty).Length;
                            Activity reply = activity.CreateReply($"Looking up account information for {activity.Text}...");
                            await connector.Conversations.ReplyToActivityAsync(reply);

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