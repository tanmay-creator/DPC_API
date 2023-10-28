using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class LOBMaster
    {
        [Key]
        public int LOB_ID { get; set; }
        public string LOB_CD { get; set; }
        public string LOB_Desc { get; set; }
        public string Row_Act_Ind { get; set; }
        
    }
}
