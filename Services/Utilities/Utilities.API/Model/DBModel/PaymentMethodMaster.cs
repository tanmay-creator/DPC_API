using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class PaymentMethodMaster
    {
        [Key]

        public int PYMT_TYP_ID { get; set; }
        public string PYMT_TYP_CD { get; set; }

        public string PYMT_TYP_DESC { get; set; }
        //public string VND_CD { get; set; }
        //public string VND_DESC { get; set; }
        public string Row_Act_Ind { get; set; }
        
    }
}
