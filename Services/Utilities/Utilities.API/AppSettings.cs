using Microsoft.OpenApi.Models;

namespace Utilities.API
{
    public class AppSettings
    {
        public double APIErrorRedisTimeout { get; set; }
        public double UIErrorRedisTimeout { get; set; }
        public double FileErrorRedisTimeout { get; set; }
        public double IsLobEnrolledRedisTimeout { get; set; }
        public double LobPaymentMappingRedisTimeout { get; set; }
        public double BankAccListRedisTimeout { get; set; }
        public double CountryListRedisTimeout { get; set; }
        public double StateListRedisTimeout { get; set; }
        public double CityListRedisTimeout { get; set; }
       
        


    }
}
