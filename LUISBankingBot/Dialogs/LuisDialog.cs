namespace LUISBankingBot.Dialogs
{
    using LUISBankingBot.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LuisBankingDialog
    {
        // app id, subscription key
        [LuisModel("6841d389-70d6-45ec-96a9-a2893d1c778e", "5d7817feda724399aaf69441f3fb18eb")]
        [Serializable]
        public class BankingDialog : LuisDialog<object>
        {
            private LUISBankModel luisBankModel;

            [LuisIntent("Deposit")]
            public async Task DepositHandler(IDialogContext context, LuisResult result)
            {
                var depositValues = new Dictionary<string, dynamic>
                {
                    { "Account", "" },
                    { "TransactionAmount", 17.48 },
                };
                luisBankModel = new LUISBankModel();

                await context.PostAsync("Luis intent recognized as Deposit");                
                foreach (var entityRecommendation in result.Entities)
                {
                    if (luisBankModel.entities.Contains(entityRecommendation.Type))
                    {
                        depositValues[entityRecommendation.Type] = entityRecommendation.Entity;
                    }
                }
                context.Wait(MessageReceived);
            }

            [LuisIntent("ShowBalance")]
            public async Task ShowBalanceHandler(IDialogContext context, LuisResult result)
            {
                await context.PostAsync("Luis intent recognized as ShowBalance");
                context.Wait(MessageReceived);
            }

            [LuisIntent("Withdraw")]
            public async Task WithdrawHandler(IDialogContext context, LuisResult result)
            {
                await context.PostAsync("Luis intent recognized as Withdraw");
                context.Wait(MessageReceived);
            }

            [LuisIntent("None")]
            public async Task NoneHandler(IDialogContext context, LuisResult result)
            {
                string worriedFace = "\U0001F61F";
                await context.PostAsync("I'm sorry, I don't understand " + worriedFace);
                context.Wait(MessageReceived);
            }
        }
    }
}
