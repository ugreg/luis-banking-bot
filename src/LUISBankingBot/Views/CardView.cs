namespace LUISBankingBot.Views
{
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;

    public class CardView
    {
        public CardView() { }

        public List<Attachment> MakeHelpCards()
        {
            List<Attachment> helpCards = new List<Attachment>();

            var withdrawalCard = new ThumbnailCard
            {
                Title = "Making withdrawals",
                Subtitle = "You can say things like:",
                Text = @"• ""Hi banking bot, please withdraw 100 from my savings"".",
                Images = new List<CardImage> { new CardImage("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAzMiAzMiI+PHBhdGggZD0iTTI3LjkzOCA0Ljc1TDMuMTU1IDlIMTVsMTEuMzEzLTEuOTM4TDI2LjY1NSA5aDIuMDMybC0uNzUtNC4yNXpNMiAxMHYxNmgyOFYxMEgyem00LjkzOCAyaDE4LjEyNWMtLjAzNC4xNjMtLjA2My4zMjctLjA2My41IDAgMS4zOCAxLjEyIDIuNSAyLjUgMi41LjE3MyAwIC4zMzctLjAzLjUtLjA2M3Y2LjEyNmMtLjE2My0uMDM0LS4zMjctLjA2My0uNS0uMDYzLTEuMzggMC0yLjUgMS4xMi0yLjUgMi41IDAgLjE3My4wMy4zMzcuMDYzLjVINi45MzhjLjAzMy0uMTYzLjA2Mi0uMzI3LjA2Mi0uNUM3IDIyLjEyIDUuODggMjEgNC41IDIxYy0uMTczIDAtLjMzNy4wMy0uNS4wNjN2LTYuMTI1Yy4xNjMuMDMzLjMyNy4wNjIuNS4wNjJDNS44OCAxNSA3IDEzLjg4IDcgMTIuNWMwLS4xNzMtLjAzLS4zMzctLjA2My0uNXpNMTYgMTNjLTIuNzUgMC01IDIuMjUtNSA1czIuMjUgNSA1IDUgNS0yLjI1IDUtNS0yLjI1LTUtNS01em0wIDJjMS42NyAwIDMgMS4zMyAzIDNzLTEuMzMgMy0zIDMtMy0xLjMzLTMtMyAxLjMzLTMgMy0zeiIvPjwvc3ZnPg==") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Contact Support", value: "https://www.google.com/") }
            };
            helpCards.Add(withdrawalCard.ToAttachment());

            var depositCard = new ThumbnailCard
            {
                Title = "Making deposits",
                Subtitle = "You can say things like:",
                Text = @"• ""Hi banking bot, please deposit 100 into my checkings"".",
                Images = new List<CardImage> { new CardImage("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAzMiAzMiI+PHBhdGggZD0iTTE1IDR2Mi4wM2MtMi43Ny4yMDMtNSAyLjUyNi01IDUuMzQ1IDAgMi41ODQgMS44NjQgNC43ODggNC40MDYgNS4yNWwuNTk0LjEyNVYyNGgtLjYyNUMxMi41MDUgMjQgMTEgMjIuNDk0IDExIDIwLjYyNVYxOUg5djEuNjI1QzkgMjMuNTc1IDExLjQyNiAyNiAxNC4zNzUgMjZIMTV2Mmgydi0yaC42MjVDMjAuNTczIDI2IDIzIDIzLjU3NCAyMyAyMC42MjVjMC0yLjU4NC0xLjg2My00Ljc4OC00LjQwNi01LjI1TDE3IDE1LjA2MnYtNy4wM2MxLjY5LjE4NiAzIDEuNjAyIDMgMy4zNDNWMTNoMnYtMS42MjVjMC0yLjgyLTIuMjMtNS4xNDItNS01LjM0NFY0aC0yem0wIDQuMDN2Ni42NThsLS4yNS0uMDMyYy0xLjYwNC0uMjktMi43NS0xLjY1LTIuNzUtMy4yOCAwLTEuNzQyIDEuMzEtMy4xNTggMy0zLjM0NXptMiA5LjA5NWwxLjI1LjIyYzEuNjA1LjI5IDIuNzUgMS42NSAyLjc1IDMuMjhDMjEgMjIuNDk1IDE5LjQ5MyAyNCAxNy42MjUgMjRIMTd2LTYuODc1eiIvPjwvc3ZnPg==") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Contact Support", value: "https://www.google.com/") }
            };
            helpCards.Add(depositCard.ToAttachment());

            var balanceCard = new ThumbnailCard
            {
                Title = "Viewing your balance",
                Subtitle = "You can say things like:",
                Text = @"• ""Banking bot can you show my current savings balance"".",
                Images = new List<CardImage> { new CardImage("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAzMiAzMiI+PHBhdGggZD0iTTE2IDVjLTMuODU0IDAtNyAzLjE0Ni03IDcgMCAyLjQxIDEuMjMgNC41NTIgMy4wOTQgNS44MTNDOC41MjcgMTkuMzQzIDYgMjIuODggNiAyN2gyYzAtNC40MyAzLjU3LTggOC04czggMy41NyA4IDhoMmMwLTQuMTItMi41MjctNy42NTgtNi4wOTQtOS4xODhDMjEuNzcgMTYuNTUyIDIzIDE0LjQxIDIzIDEyYzAtMy44NTQtMy4xNDYtNy03LTd6bTAgMmMyLjc3MyAwIDUgMi4yMjcgNSA1cy0yLjIyNyA1LTUgNS01LTIuMjI3LTUtNSAyLjIyNy01IDUtNXoiLz48L3N2Zz4=") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Contact Support", value: "https://www.google.com/") }
            };
            helpCards.Add(balanceCard.ToAttachment());

            return helpCards;
        }            

        public Attachment MakeReceiptCard()
        {
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
                        "https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png",
                        "https://azure.microsoft.com/en-us/pricing/")
                }
            };

            return receiptCard.ToAttachment();
        }
    }
}
