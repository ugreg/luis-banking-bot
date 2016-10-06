namespace LUISBankingBot.Models
{
    using System.Runtime.Serialization.Json;
    using System.Runtime.Serialization;

    [DataContract]
    public class AccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public int expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }

    }
}