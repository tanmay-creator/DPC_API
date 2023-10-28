using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Newtonsoft.Json.Schema;
namespace VendorConnect.API.Schemas.Model
{
    public class SchemaValidationCustomError
    {
        public string id { get; set; }
        public class EnrolledPayer
        {

            /// <summary>
            /// Unique Profile Id of Payer.If the Payer has User Profile.
            /// </summary>
            [Required]
            public string ProfileId { get; set; }
            [Required]
            public string Kind { get; set; }
        }
        public class NonEntrolledPayer
        {
            [Required]
            public string Kind { get; set; }
        }
        public class oneOF
        {
            public EnrolledPayer enrolled { get; set; }
            public NonEntrolledPayer nonenrolled { get; set; }
        }
        public oneOF oneof { get; set; }
    }
    public class Detail
    {
        public string kind { get; set; }
        public string target { get; set; }
        public Message message { get; set; }
    }

    public class Error
    {
        public int status { get; set; }
        public string kind { get; set; }
        public Message message { get; set; }
        public List<Detail> details { get; set; } = new List<Detail>();
    }

    public class Message
    {
        public string code { get; set; }
        public string @default { get; set; }
    }

    public class Root
    {
        public Error error { get; set; }
    }


}
