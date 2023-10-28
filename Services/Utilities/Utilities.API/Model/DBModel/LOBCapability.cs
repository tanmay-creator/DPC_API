using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class LOBCapability
    {
        [Key]
        public int LOB_ID { get; set; }
        public int CPBLTY_ID { get; set; }       
        public string Row_Act_Ind { get; set; }
        
    }
}
