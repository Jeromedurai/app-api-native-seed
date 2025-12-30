using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Context;
using XtraChef.API.Base.Context;

namespace Tenant.Query.Context.Authentication
{
    public class AuthenticationContext : TnReadOnlyContext
    {

        #region Variables

        public DbSet<Model.Authentication.ValidTokenContext> ValidTokenContexts { get; set; }

        #endregion

        public AuthenticationContext(DbContextOptions options) : base(options)
        {
        }

        #region Overridden Methods

        /// <summary>
        /// Ons the model creating.
        /// </summary>
        /// <param name="modelBuilder">Model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base constructor
            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
