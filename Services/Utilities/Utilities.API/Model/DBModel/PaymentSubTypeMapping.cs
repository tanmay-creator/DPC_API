using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class PaymentSubTypeMapping
    {
        [Key]

        public int PYMT_TYP_ID { get; set; }
        public int PYMT_SBTYP_ID { get; set; }

        public string Row_Act_Ind { get; set; }
        
    }
}
