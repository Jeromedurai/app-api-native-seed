using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenant.API.Base.Startup;
using Tenant.API.Base.Util;
using Sa.Common.ADO.DataAccess;
using Microsoft.OpenApi.Models;
using System.Data.SqlClient;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Tenant.Query
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XtraChef.Product.Startup"/> class.
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            //initialize the startup
            IConfigurationBuilder builder = TnBaseStartup.InitializeStartup(env);

            /*** Begin: API Specific Configuration ***/

            //set version number
            TnBaseStartup.Version = "1.0";

            //add user secrets
            builder.AddUserSecrets<Startup>();

            Configuration = builder.Build();
            // GJGwAWjB7C2tSSad65NO
            // I0FEO09BK0VEEN9DGJ9FGU8BMQ8DKTSP
            if (Configuration["ConnectionStrings:Default"] != null && Configuration["ConnectionStrings:Default"].Contains("MultipleActiveResultSets"))
            {
                Configuration["ConnectionStrings:Default"] = Configuration["ConnectionStrings:Default"].Replace("MultipleActiveResultSets=True;", "");
            }

            if (Configuration["ConnectionStrings:Default_SSL"] != null && Configuration["ConnectionStrings:Default_SSL"].Contains("MultipleActiveResultSets"))
            {
                Configuration["ConnectionStrings:Default_SSL"] = Configuration["ConnectionStrings:Default_SSL"].Replace("MultipleActiveResultSets=True;", "");
            }
            string pwd = "esFpBxEH7Q8CBr93";
            string psw = EnDecrypt(pwd, false);
            
            //Configuration["AWS.Logging:LogGroup"] = "xtraCHEF.Api.Product";

            /*** End: API Specific Configuration ***/
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //initialize base services
            TnBaseStartup.InitializeServices(Configuration, services);
            services.AddMvc().AddNewtonsoftJson();
            services.AddSingleton(Configuration);
            services.AddMvc().
                AddNewtonsoftJson(x => 
                {
                    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                    x.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            //menu contrext 
            services.AddDbContextPool<Context.UserContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("Default_SSL"));
                options.UseSqlServer(SetConnectionString(Configuration["ConnectionStrings:Default"]),
                sqlServerOptions => sqlServerOptions.CommandTimeout(60));
                options.EnableSensitiveDataLogging(true);
                options.UseLoggerFactory(TnBaseStartup.LoggerFactory);
            });

            services.AddDbContextPool<Context.AppNotification.AppNotificationContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("Default_SSL"));
                options.UseSqlServer(SetConnectionString(Configuration["ConnectionStrings:Default"]),
                sqlServerOptions => sqlServerOptions.CommandTimeout(60));
                options.EnableSensitiveDataLogging(true);
                options.UseLoggerFactory(TnBaseStartup.LoggerFactory);
            });

            services.AddDbContextPool<Context.Content.ContentContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("Default_SSL"));
                options.UseSqlServer(SetConnectionString(Configuration["ConnectionStrings:Default"]),
                sqlServerOptions => sqlServerOptions.CommandTimeout(60));
                options.EnableSensitiveDataLogging(true);
                options.UseLoggerFactory(TnBaseStartup.LoggerFactory);
            });

            //adding DbContext 'Invoice'
            services.AddDbContext<Context.Product.ProductContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("Default_SSL"));
                options.UseSqlServer(SetConnectionString(Configuration["ConnectionStrings:Default"]),
                sqlServerOptions => sqlServerOptions.CommandTimeout(60));
                options.EnableSensitiveDataLogging(true);
                options.UseLoggerFactory(TnBaseStartup.LoggerFactory);
            });

            services.AddDbContext<Context.Authentication.AuthenticationContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("Default_SSL"));
                options.UseSqlServer(SetConnectionString(Configuration["ConnectionStrings:Default"]),
                sqlServerOptions => sqlServerOptions.CommandTimeout(60));
                options.EnableSensitiveDataLogging(true);
                options.UseLoggerFactory(TnBaseStartup.LoggerFactory);
            });

            //initialize scoped instances 
            services.AddScoped(typeof(Tenant.Query.Service.User.UserService), typeof(Tenant.Query.Service.User.UserService));
            services.AddScoped(typeof(Tenant.Query.Repository.User.UserRepository), typeof(Tenant.Query.Repository.User.UserRepository));

            services.AddScoped(typeof(Tenant.Query.Service.AppNotification.AppNotificationService), typeof(Tenant.Query.Service.AppNotification.AppNotificationService));
            services.AddScoped(typeof(Tenant.Query.Repository.AppNotification.AppNotificationRepository), typeof(Tenant.Query.Repository.AppNotification.AppNotificationRepository));

            //initialize scoped instance for product
            services.AddScoped(typeof(Service.Product.ProductService), typeof(Service.Product.ProductService));
            services.AddScoped(typeof(Repository.Product.ProductRepository), typeof(Repository.Product.ProductRepository));

            services.AddScoped(typeof(Service.Authentication.AuthenticationService), typeof(Service.Authentication.AuthenticationService));
            services.AddScoped(typeof(Repository.Authentication.AuthenticationRepository), typeof(Repository.Authentication.AuthenticationRepository));

            //initialize scoped instance for coupon
            services.AddScoped(typeof(Service.Coupon.CouponService), typeof(Service.Coupon.CouponService));
            services.AddScoped(typeof(Repository.Coupon.CouponRepository), typeof(Repository.Coupon.CouponRepository));

            // Initialize scoped instance for product reviews
            services.AddScoped(typeof(Service.Product.ProductReviewService), typeof(Service.Product.ProductReviewService));
            services.AddScoped(typeof(Repository.Product.ProductReviewRepository), typeof(Repository.Product.ProductReviewRepository));

            // Initialize scoped instance for dashboard
            services.AddScoped(typeof(Service.Dashboard.DashboardService), typeof(Service.Dashboard.DashboardService));
            services.AddScoped(typeof(Repository.Dashboard.DashboardRepository), typeof(Repository.Dashboard.DashboardRepository));

            // Initialize scoped instance for shipping
            services.AddScoped(typeof(Service.Shipping.ShippingService), typeof(Service.Shipping.ShippingService));
            services.AddScoped(typeof(Repository.Shipping.ShippingRepository), typeof(Repository.Shipping.ShippingRepository));

            // Initialize scoped instance for address
            services.AddScoped(typeof(Service.Address.AddressService), typeof(Service.Address.AddressService));
            services.AddScoped(typeof(Repository.Address.AddressRepository), typeof(Repository.Address.AddressRepository));

            //Register the swagger generator, defining one or more swagger documnets
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(TnBaseStartup.Version, new OpenApiInfo { Title = "Tenant Query", Version = TnBaseStartup.Version });
                c.CustomSchemaIds(i => i.FullName);
            });

            services.AddSingleton<DataAccess>(_ => new DataAccess(SetConnectionString(Configuration["ConnectionStrings:Default"])));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //initialize application with base pipeline
            TnBaseStartup.InitializeApplication(Configuration, app, env, loggerFactory);

            /*** Begin: API Specific Configuration ***/

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{TnBaseStartup.Version}/swagger.json", $"Product API v{TnBaseStartup.Version}");
            });

            /*** End: API Specific Configuration ***/
        }

        public static string SetConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.Password = Service.Product.ProductService.EnDecrypt(builder.Password, true);
            return builder.ToString();
        }
        
        public static string EnDecrypt(string input, bool decrypt = false)
        {
            string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ984023";

            if (decrypt)
            {
                Dictionary<string, uint> _index = null;
                Dictionary<string, Dictionary<string, uint>> _indexes =
                    new Dictionary<string, Dictionary<string, uint>>(2, StringComparer.InvariantCulture);

                if (_index == null)
                {
                    Dictionary<string, uint> cidx;

                    string indexKey = "I" + _alphabet;

                    if (!_indexes.TryGetValue(indexKey, out cidx))
                    {
                        lock (_indexes)
                        {
                            if (!_indexes.TryGetValue(indexKey, out cidx))
                            {
                                cidx = new Dictionary<string, uint>(_alphabet.Length, StringComparer.InvariantCulture);
                                for (int i = 0; i < _alphabet.Length; i++)
                                {
                                    cidx[_alphabet.Substring(i, 1)] = (uint)i;
                                }

                                _indexes.Add(indexKey, cidx);
                            }
                        }
                    }

                    _index = cidx;
                }

                MemoryStream ms = new MemoryStream(Math.Max((int)Math.Ceiling(input.Length * 5 / 8.0), 1));

                for (int i = 0; i < input.Length; i += 8)
                {
                    int chars = Math.Min(input.Length - i, 8);

                    ulong val = 0;

                    int bytes = (int)Math.Floor(chars * (5 / 8.0));

                    for (int charOffset = 0; charOffset < chars; charOffset++)
                    {
                        uint cbyte;
                        if (!_index.TryGetValue(input.Substring(i + charOffset, 1), out cbyte))
                        {
                            throw new ArgumentException(string.Format("Invalid character {0} valid characters are: {1}",
                                input.Substring(i + charOffset, 1), _alphabet));
                        }

                        val |= (((ulong)cbyte) << ((((bytes + 1) * 8) - (charOffset * 5)) - 5));
                    }

                    byte[] buff = BitConverter.GetBytes(val);
                    Array.Reverse(buff);
                    ms.Write(buff, buff.Length - (bytes + 1), bytes);
                }

                return System.Text.ASCIIEncoding.ASCII.GetString(ms.ToArray());
            }
            else
            {
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(input);

                StringBuilder result = new StringBuilder(Math.Max((int)Math.Ceiling(data.Length * 8 / 5.0), 1));

                byte[] emptyBuff = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                byte[] buff = new byte[8];

                for (int i = 0; i < data.Length; i += 5)
                {
                    int bytes = Math.Min(data.Length - i, 5);

                    Array.Copy(emptyBuff, buff, emptyBuff.Length);
                    Array.Copy(data, i, buff, buff.Length - (bytes + 1), bytes);
                    Array.Reverse(buff);
                    ulong val = BitConverter.ToUInt64(buff, 0);

                    for (int bitOffset = ((bytes + 1) * 8) - 5; bitOffset > 3; bitOffset -= 5)
                    {
                        result.Append(_alphabet[(int)((val >> bitOffset) & 0x1f)]);
                    }
                }

                return result.ToString();
            }
        }
    }
}
