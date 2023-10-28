using System.ComponentModel.DataAnnotations;

namespace UserProfile.API.Domain.Entities
{
    public class PhoneNumber
    {
        [Required]
        public string[] kind { get; set; } 

        [Required]
        public string number { get; set; }

        Boolean allowSMS { get; set; } = false;

    }
}
