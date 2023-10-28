namespace FundingAccount.API.FundingAccount_Schema.Model.ValidationErrorModels
{
    public class Error
    {
        public int status { get; set; }
        public string kind { get; set; }
        public Message message { get; set; }

        public List<Detail> details { get; set; } = new List<Detail>();
    }
}
