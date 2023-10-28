using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
//using RedisService.API.Model.R;
using System.Collections.Specialized;
using System.Reflection.Metadata;
using System.Web;
using Utilities.API.Model.DBModel;

namespace RedisService.API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        #region Variables
        //NameValueCollection queryStringCollection;
        RouteValueDictionary queryStringCollection;
        string lobSchemaName = "lob-name";
        string schemaName = string.Empty;
        #endregion

        protected IHttpContextAccessor HttpContextAccessor { get; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this.HttpContextAccessor = httpContextAccessor;
        }
       
        public DbSet<LOBMaster> T_LOB_LKUP { get; set; }

        public DbSet<LOBVendorMapping> T_LOB_VND_MAP { get; set; }
        public DbSet<VendorMaster> T_VND_LKUP { get; set; }
        public DbSet<CapabilityMaster> T_DPC_CPBLTY_LKUP { get; set; }
        public DbSet<LOBCapability> T_LOB_CPBLTY_MAP { get; set; }

        public DbSet<LOBVendorPaymentMethod> T_LOB_VND_PYMT_MAP { get; set; }
        public DbSet<PaymentMethodMaster> T_PYMT_TYP_LKUP { get; set; }

        public DbSet<PaymentSubTypeMaster> T_PYMT_SBTYP_LKUP { get; set; }

        public DbSet<PaymentSubTypeMapping> T_PYMT_SBTYP_MAP { get; set; }

        public DbSet<CountryMaster> T_CNTRY_LKUP { get; set; }
        public DbSet<StateMaster> T_CNTRY_ST_LKUP { get; set; }
        public DbSet<CountryStateMapping> T_CNTRY_ST_MAP { get; set; }
        public DbSet<CityMaster> T_CITY_LKUP { get; set; }

        public DbSet<StateCityMapping> T_ST_CITY_MAP { get; set; }

        public DbSet<VendorErrorMaster> T_VND_ERR_LKUP { get; set; }       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CTDPC_SFD");

            //HttpContext? httpContext = this.HttpContextAccessor.HttpContext;
            //if (httpContext != null)
            //{
            //    //if (httpContext.Request.QueryString.HasValue)
            //    if(httpContext.Request.RouteValues.Count > 0 && httpContext.Request.RouteValues.ContainsKey(lobSchemaName)) 
            //    {
            //        schemaName = Convert.ToString(httpContext.Request.RouteValues[lobSchemaName]);
            //    }

            //    if (!String.IsNullOrEmpty(schemaName))
            //    {
            //        modelBuilder.HasDefaultSchema(schemaName);
            //    }
            //}
        }
    }
}
