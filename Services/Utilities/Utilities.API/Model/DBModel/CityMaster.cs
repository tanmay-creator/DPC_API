using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace Utilities.API.Model.DBModel
{
    //[Keyless]
    public class CityMaster
    {
        [Key]

        public int CITY_ID { get; set; }
        public string CITY_CD { get; set; }
        public string CITY_CTGY { get; set; }
        public string CITY_DESC { get; set; }      
        public string Row_Act_Ind { get; set; }

    }
}
