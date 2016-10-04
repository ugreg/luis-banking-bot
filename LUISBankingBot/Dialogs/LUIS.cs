using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUIS_Banking.Dialogs
{
    public class LUIS
    {
        [LuisModel("6841d389-70d6-45ec-96a9-a2893d1c778e", "5d7817feda724399aaf69441f3fb18eb")]
        [Serializable]
        public class StockDialog : LuisDialog<object>
        {
            [LuisIntent("LusiIntent")]
            public async Task RepeatLastStock(IDialogContext context, LuisResult result)
            {
                await context.PostAsync("Luis intent recognized");
                context.Wait(MessageReceived);
            }

            [LuisIntent("None")]
            public async Task NoneHandler(IDialogContext context, LuisResult result)
            {
                await context.PostAsync("I'm sorry, I don't understand");
                context.Wait(MessageReceived);
            }
        }
    }
}
