using System.ComponentModel.DataAnnotations;

namespace UserProfile.API.Domain.Entities
{
    public class UserBillerAccount
    {
        [Required]
        public string kind { get; set; }
        [Required]
        public string billerId { get; set; } 
        [Required]
        public string billerAccountId { get; set; } 
    }
}