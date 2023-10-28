using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class StateMaster
    {
        [Key]

        public int CNTRY_ST_ID { get; set; }
        public string CNTRY_ST_CD { get; set; }
        public string CNTRY_ST_CTGY { get; set; }
        public string CNTRY_ST_DESC { get; set; }
        public string ZIP_CD_RNG { get; set; }
        public string Row_Act_Ind { get; set; }

    }
}
