using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class PaymentSubTypeMaster
    {
        [Key]

        public int PYMT_SBTYP_ID { get; set; }
        public string PYMT_SBTYP_CTGY { get; set; }

        public string PYMT_SBTYP_CD { get; set; }
        public string PYMT_SBTYP_DESC { get; set; }
        public string PYMT_SBTYP_DSPL_VAL { get; set; }
        
        public string Row_Act_Ind { get; set; }
        
    }
}
