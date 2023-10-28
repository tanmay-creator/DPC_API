namespace UserProfile.API.Domain.Entities
{
    public class Error
    {
        public int status { get; set; }
        public string code { get; set; }
        public string type { get; set; }

        public string description { get; set; }
        //public Message message { get; set; }

        //public List<Details> details { get; set; } = new List<Details>();
    }
}
