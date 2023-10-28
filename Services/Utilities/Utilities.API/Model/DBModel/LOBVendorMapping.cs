using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class LOBVendorMapping
    {
        [Key]

        public int LOB_VND_MAP_ID { get; set; }
        public int LOB_ID { get; set; }

        public int VND_ID { get; set; }
        //public string VND_CD { get; set; }
        //public string VND_DESC { get; set; }
        public string Row_Act_Ind { get; set; }
        
    }
}
