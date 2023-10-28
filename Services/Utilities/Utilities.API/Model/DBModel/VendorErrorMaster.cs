using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class VendorErrorMaster
    {
        [Key]

        
        public int VND_ID { get; set; }
        public int ERR_ID { get; set; }
        public string ERR_SRC { get; set; }
        public string ERR_CTGY { get; set; }
        public string ERR_CD { get; set; }
        public string ERR_DESC { get; set; }
        public string DPC_ERR_CD { get; set; }

        public string DPC_ERR_KIND { get; set; }
        public string DPC_ERR_DESC { get; set; }
       
        public string Row_Act_Ind { get; set; }
        
    }
}
