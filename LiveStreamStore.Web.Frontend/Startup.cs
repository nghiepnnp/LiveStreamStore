using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LiveStreamStore.Lib.Core;
using LiveStreamStore.Lib.Services.ApiDashBoard;
using LiveStreamStore.Lib.Services.Caching;
using LiveStreamStore.Lib.Services.Cart;
using LiveStreamStore.Lib.Services.Customers;
using LiveStreamStore.Lib.Services.Geos;
using LiveStreamStore.Lib.Services.Livestreams;
using LiveStreamStore.Lib.Services.Logs;
using LiveStreamStore.Lib.Services.Orders;
using LiveStreamStore.Lib.Services.Products;
using LiveStreamStore.Lib.Services.UploadFiles;
using LiveStreamStore.Lib.Services.Users;
using LiveStreamStore.Lib.Services.WorkContext;

namespace LiveStreamStore.Web.Frontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = FacebookDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Facebook:AppSecret"];
                facebookOptions.SaveTokens = true;
                facebookOptions.Scope.Add("email");
                //facebookOptions.Scope.Add("user_birthday");
                //facebookOptions.Scope.Add("Live Video API");
                facebookOptions.Scope.Add("user_videos ");
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddSession();

            // Dependecy
            //services.AddTransient<IServiceProvider, ServiceProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IWorkContext, WebWorkContext>();
            services.AddSingleton<ILivestreamService, LivestreamServices>();
            services.AddSingleton<IFileService, FileServices>();
            services.AddTransient<IUserService, UserServices>();
            services.AddSingleton<IProductService, ProductServices>();
            services.AddTransient<ICustomerService, CustomerServices>();
            services.AddSingleton<IOrderService, OrderServices>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<ICacheKeyService, CacheKeyService>();
            services.AddSingleton<IAddressService, AddressService>();
            services.AddSingleton<ILocationAddressService, LocationAddressService>();
            services.AddSingleton<IApiDashBoard, ApiDashBoard>();
            services.AddTransient<ILogService, LogServices>();

            // resolve
            EngineContext.SetServiceProvider(services.BuildServiceProvider());
            // log4net
            Log4NetLoader.AddLog4NetConfiguration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "StoreCode",
                   pattern: "{StoreCode}/{controller=Livestream}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Livestream}/{action=Index}/{id?}");
            });
        }
    }
}
