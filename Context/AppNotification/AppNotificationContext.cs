using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tenant.API.Base.Context;
using XtraChef.API.Base.Context;

namespace Tenant.Query.Context.AppNotification
{
    public class AppNotificationContext : TnReadOnlyContext
    {

        #region Variables
        public DbSet<Model.AppNotification.AppNotification> InAppNotifications { get; private set; }

        public DbSet<Model.AppNotification.NotificationMessage> NotificationMessages { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XtraChef.System.Query.Context.InAppNotification.InAppNotification"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        public AppNotificationContext(DbContextOptions options) : base(options)
        {
        }
        #endregion

        #region Value Converter

        //long to string converter
        readonly ValueConverter<string, long> longToStringConverter = new ValueConverter<string, long>(
            v => long.Parse(v),
            v => v.ToString());

        //int to long converter
        readonly ValueConverter<long, Int32> intToLongConverter = new ValueConverter<long, Int32>(
            v => Int32.Parse(v.ToString()),
            v => long.Parse(v.ToString()));

        #endregion

        #region Overridden Methods

        /// <summary>
        /// On model creating.
        /// </summary>
        /// <param name="modelBuilder">Model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base constructor
            base.OnModelCreating(modelBuilder);
            this.SetInAppNotificationProperty(modelBuilder);
        }

        #endregion

        #region Private 

        /// <summary>
        /// set property for the inapp notification
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void SetInAppNotificationProperty(ModelBuilder modelBuilder)
        {

            //Fire api config
            modelBuilder.Entity<Model.AppNotification.AppNotification>()
                         .HasKey(x => new { x.Id });

            modelBuilder.Entity<Model.AppNotification.AppNotification>()
                        .Property(x => x.TenantId)
                        .HasConversion(longToStringConverter);

            modelBuilder.Entity<Model.AppNotification.AppNotification>()
                       .Property(x => x.CreatedBy)
                       .HasConversion(longToStringConverter);
        }

        #endregion
    }
}
