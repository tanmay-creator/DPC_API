namespace Transaction.API.Payment_Schema.Model.ValidationErrorModels
{
    public class Detail
    {
        public string kind { get; set; }
        public string target { get; set; }
        public Message message { get; set; }
    }
}
