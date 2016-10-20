namespace LUISBankingBot.Models
{
    public class Customer
    {
        public double balance { get; }

        public Customer(double balance)
        {
            this.balance = balance;
        }
    }
}