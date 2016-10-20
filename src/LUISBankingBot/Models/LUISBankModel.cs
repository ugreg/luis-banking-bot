namespace LUISBankingBot.Models
{
    using System.Collections.Generic;

    public class LUISBankModel
    {
        public List<string> intents { get; private set; }
        public List<string> entities { get; private set; }
        public List<string> composites { get; private set; }

        public LUISBankModel()
        {
            intents = new List<string>() { "Deposit", "None", "ShowBalance", "Transfer", "Withdraw" };
            entities = new List<string>() { "Account", "AccountTo", "TransactionAmount" };
        }

    }
}