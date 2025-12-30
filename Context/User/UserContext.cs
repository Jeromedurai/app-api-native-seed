using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tenant.API.Base.Context;
using XtraChef.API.Base.Context;

namespace Tenant.Query.Context
{
    public class UserContext : TnReadOnlyContext
    {
        #region Variables

        public DbSet<Model.User.User> Users { get; private set; }
        public DbSet<Model.User.UserRole> UserRoles { get; private set; }
        public DbSet<Model.User.Role> Roles { get; private set; }

        #endregion

        private ValueConverter longToStringConverter = new ValueConverter<string, long>(
                                    v => long.Parse(v),
                                    v => v.ToString());

        public UserContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// User property
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void SetUserProperties(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Model.User.User>()
                        .HasKey(u => new { u.UserId });

            //User Property convertions
            modelBuilder.Entity<Model.User.User>()
                .Property(u => u.UserId)
                .HasConversion(longToStringConverter);

            modelBuilder.Entity<Model.User.User>()
                .Property(u => u.TenantId)
                .HasConversion(longToStringConverter);
        }

        /// <summary>
        /// User role property
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void SetUserRoleProperties(ModelBuilder modelBuilder)
        {
            //User Role property convertion
            modelBuilder.Entity<Model.User.UserRole>()
                .Property(ur => ur.Guid)
                .HasConversion(longToStringConverter)
                .HasColumnName("USER_ROLE_ID");

            modelBuilder.Entity<Model.User.UserRole>()
                .Property(ur => ur.UserId)
                .HasConversion(longToStringConverter)
                .HasColumnName("USER_ID");

            modelBuilder.Entity<Model.User.UserRole>()
                .Property(ur => ur.RoleId)
                .HasConversion(longToStringConverter)
                .HasColumnName("ROLE_ID");

            modelBuilder.Entity<Model.User.UserRole>()
                .Property(ur => ur.GroupId)
                .HasConversion(longToStringConverter)
                .HasColumnName("GROUP_ID");

            modelBuilder.Entity<Model.User.UserRole>()
                .Property(ur => ur.Created)
                .HasColumnName("CREATED_ON");

            modelBuilder.Entity<Model.User.UserRole>()
                .Property(ur => ur.LastModified)
                .HasColumnName("MODIFIED_ON");

        }

        /// <summary>
        /// Role property
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void SetRoleProperties(ModelBuilder modelBuilder)
        {
            //Role Property convertion
            modelBuilder.Entity<Model.User.Role>()
                .Property(ur => ur.Guid)
                .HasConversion(longToStringConverter)
                .HasColumnName("ROLE_ID");

            modelBuilder.Entity<Model.User.Role>()
                .Property(r => r.RoleName)
                .HasColumnName("ROLE_NAME");

            modelBuilder.Entity<Model.User.Role>()
                .Property(r => r.RoleDescription)
                .HasColumnName("ROLE_DESCRIPTION");

            modelBuilder.Entity<Model.User.Role>()
                .Property(r => r.Active)
                .HasColumnName("ACTIVE");

            modelBuilder.Entity<Model.User.Role>()
                .Property(r => r.AuthorityLevel)
                .HasColumnName("AUTHORITY_LEVEL");

            modelBuilder.Entity<Model.User.Role>()
                .Property(ur => ur.Created)
                .HasColumnName("CREATED_ON");

            modelBuilder.Entity<Model.User.Role>()
                .Property(ur => ur.LastModified)
                .HasColumnName("MODIFIED_ON");


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

            //User property convertion
            SetUserProperties(modelBuilder);
            SetUserRoleProperties(modelBuilder);
            SetRoleProperties(modelBuilder);

            //User mapping
            modelBuilder.Entity<Model.User.User>()
                .HasMany(u => u.UserRoles)
                .WithOne()
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<Model.User.UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(r => r.RoleId);

        }

        #endregion
    }
}
