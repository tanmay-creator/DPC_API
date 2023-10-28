using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class CountryStateMapping
    {
        [Key]

        public int CNTRY_ST_MAP_ID { get; set; }
        public int CNTRY_ID { get; set; }
        public int CNTRY_ST_ID { get; set; }        
        public string Row_Act_Ind { get; set; }
        
    }
}
