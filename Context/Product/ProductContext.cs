using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.API.Base.Context;
using XtraChef.API.Base.Context;

namespace Tenant.Query.Context.Product
{
    public class ProductContext : TnReadOnlyContext
    {
        #region Variables
        public DbSet<Model.Product.ProductMaster> productMasters { get; private set; }

        public DbSet<Model.Product.ProductImage> productImages { get; private set; }

        public DbSet<Model.Product.ProductReview> productReviews { get; private set; }

        public DbSet<Model.Product.ProductCategory> productCategories { get; private set; }

        #endregion

        public ProductContext(DbContextOptions<Context.Product.ProductContext> options) : base(options)
        {
            //var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //if (env != "Development")
            //{
            //    this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //    this.Database.SetCommandTimeout(180);
            //}
        }

        #region ValueConverter

        //long to string converter
        ValueConverter longToStringConverter = new ValueConverter<string, long>(
            v => long.Parse(v),
            v => v.ToString());

        #endregion

        #region Model builder 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //set invoice properties
            this.SetProductMasterProperties(modelBuilder);

            this.SetProductImages(modelBuilder);

            this.SetProductReviews(modelBuilder);
            
            this.SetProductCategory(modelBuilder);

            modelBuilder.Entity<Model.Product.ProductMaster>()
                          .HasMany(u => u.ProductImages)
                          .WithOne()
                          .HasForeignKey(u => u.ProductId);

            modelBuilder.Entity<Model.Product.ProductMaster>()
                          .HasMany(u => u.ProductReviews)
                          .WithOne()
                          .HasForeignKey(u => u.ProductId);
        }

        private void SetProductImages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.Product.ProductImage>()
                            .HasKey(u => new { u.Id });

            modelBuilder.Entity<Model.Product.ProductImage>()
                            .Property(u => u.ProductId)
                            .HasColumnName("ProductId");
        }

        private void SetProductMasterProperties(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Model.Product.ProductMaster>()
                        .HasKey(u => new { u.Id });

            modelBuilder.Entity<Model.Product.ProductMaster>()
                        .Property(u => u.TenantId)
                        .HasConversion(longToStringConverter);

            modelBuilder.Entity<Model.Product.ProductMaster>()
                        .Property(u => u.Price)
                        .HasPrecision(18, 2);
        }

        private void SetProductReviews(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Model.Product.ProductReview>()
                        .HasKey(u => new { u.Id });

            modelBuilder.Entity<Model.Product.ProductReview>()
                        .Property(u => u.ProductId)
                        .HasColumnName("ProductId");
        }

        private void SetProductCategory(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Model.Product.ProductCategory>()
                        .HasKey(u => new { u.CategoryId });

            modelBuilder.Entity<Model.Product.ProductCategory>()
                        .Property(u => u.TenantId)
                        .HasConversion(longToStringConverter);
        }
        #endregion
    }
}
