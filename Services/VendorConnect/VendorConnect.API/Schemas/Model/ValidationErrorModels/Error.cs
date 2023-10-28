

namespace VendorConnect.API.Schemas.Model.ValidationErrorModels
{
    public class Error
    {
        public int? status { get; set; }

        #nullable enable
        public string? kind { get; set; }

        #nullable enable
        public Message? message { get; set; }

        public List<Detail> details { get; set; } = new List<Detail>();
    }
}
