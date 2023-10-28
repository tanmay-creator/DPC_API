using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class CapabilityMaster
    {
        [Key]
        public int CPBLTY_ID { get; set; }
        public string CPBLTY_CD { get; set; }
        public string CPBLTY_DESC { get; set; }
        public string CPBLTY_FREQ { get; set; }
        public string Row_Act_Ind { get; set; }
        
    }
}
