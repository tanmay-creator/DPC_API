using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class CountryMaster
    {
        [Key]

        public int CNTRY_ID { get; set; }
        public string CNTRY_CD { get; set; }
        public string CNTRY_DESC { get; set; }

        public string Row_Act_Ind { get; set; }
        
    }
}
