using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using XtraChef.API.Base.Context;

namespace Tenant.Query.Context.Content
{
    public class ContentContext : TnReadOnlyContext
    {
        #region Constractor
        public ContentContext(DbContextOptions<ContentContext> options) : base(options)
        {

        }
        #endregion

        public DbSet<Model.AppNotification.AppConfig> appConfigs { get; private set; }

        #region Conversion

        //long to string converter
        readonly ValueConverter<string, long> longToStringConverter = new ValueConverter<string, long>(
            v => long.Parse(v),
            v => v.ToString());

        #endregion


        #region Overridden

        /// <summary>
        /// Context Model creation 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.AppNotification.AppConfig>()
                        .HasNoKey();
        }

        #endregion
    }
}
