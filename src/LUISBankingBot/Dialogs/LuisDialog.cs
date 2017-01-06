namespace LUISBankingBot.Dialogs
{
    using LUISBankingBot.Models;
    using LUISBankingBot.Views;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LuisBankingDialog
    {
        [LuisModel("6841d389-70d6-45ec-96a9-a2893d1c778e", "5d7817feda724399aaf69441f3fb18eb")]
        [Serializable]
        public class BankingDialog : LuisDialog<object>
        {
            private Customer customer;  
            private LUISBankModel luisBankModel;

            [LuisIntent("Deposit")]
            public async Task DepositHandler(IDialogContext context, LuisResult result)
            {
                var utteranceEntities = new Dictionary<string, dynamic>
                {
                    { "Account", "" },
                    { "TransactionAmount", 0d },
                };
                luisBankModel = new LUISBankModel();
                                
                foreach (var entityRecommendation in result.Entities)
                {
                    if (luisBankModel.entities.Contains(entityRecommendation.Type)) 
                    {
                        utteranceEntities[entityRecommendation.Type] = entityRecommendation.Entity;
                    }
                }
                await context.PostAsync("Got it!");
                await context.PostAsync($"I'll go ahead and deposit ${utteranceEntities["TransactionAmount"]} into your {utteranceEntities["Account"]}.");
                // confirmation to select the propse intent no, yes
                // ..
                context.Wait(MessageReceived);
            }

            [LuisIntent("ShowBalance")]
            public async Task ShowBalanceHandler(IDialogContext context, LuisResult result)
            {
                await context.PostAsync("Looks like you currentlly have $x in your x account.");
                context.Wait(MessageReceived);
            }

            [LuisIntent("Transfer")]
            public async Task TransferHandler(IDialogContext context, LuisResult result)
            {
                await context.PostAsync("Luis intent recognized as Transfer");
                context.Wait(MessageReceived);
            }

            [LuisIntent("Withdraw")]
            public async Task WithdrawHandler(IDialogContext context, LuisResult result)
            {
                var utteranceEntities = new Dictionary<string, dynamic>
                {
                    { "Account", "" },
                    { "TransactionAmount", 0d },
                };
                luisBankModel = new LUISBankModel();

                foreach (var entityRecommendation in result.Entities)
                {
                    if (luisBankModel.entities.Contains(entityRecommendation.Type))
                    {
                        utteranceEntities[entityRecommendation.Type] = entityRecommendation.Entity;
                    }
                }
                
                await context.PostAsync("Sure thing!");
                await context.PostAsync($"I'll go ahead and withdraw ${utteranceEntities["TransactionAmount"]} from your {utteranceEntities["Account"]}.");
                context.Wait(MessageReceived);
            }

            [LuisIntent("None")]
            public async Task NoneHandler(IDialogContext context, LuisResult result)
            {
                string worriedFace = "\U0001F61F";
                string smilingFace = "\U0001F642";

                await context.PostAsync("I'm sorry, I didn't get that " + worriedFace + '.');
                await context.PostAsync("Here are some things I know how to talk about! " + smilingFace);

                var message = context.MakeMessage();

                var receiptCard = new ReceiptCard
                {
                    Title = "John Doe",
                    Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
                    Items = new List<ReceiptItem>
            {
                new ReceiptItem("Data Transfer", price: "$ 38.45", quantity: "368", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png")),
                new ReceiptItem("App Service", price: "$ 45.00", quantity: "720", image: new CardImage(url: "https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png")),
            },
                    Tax = "$ 7.50",
                    Total = "$ 90.95",
                    Buttons = new List<CardAction>
            {
                new CardAction(
                    ActionTypes.OpenUrl,
                    "More information",
                    "https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png",
                    "https://azure.microsoft.com/en-us/pricing/")
            }
                };

                message.Attachments = new List<Attachment>();
                message.Attachments.Add(receiptCard.ToAttachment());

                await context.PostAsync(message);
            }
        }
    }
}
