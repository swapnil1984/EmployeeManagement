using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement
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
            //services.AddRazorPages();

            // Following is the line where Database setup/configuration is done. The connection string comes from
            // appsettings.json file
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("EmployeeDBConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                config.EnableEndpointRouting = false;
            });

            // In .Net Core versions like 2.0 and lower, refreshing the page used to compile the project in runtime.
            // Added following line in order to enable runtime compilation in .Net 5.0 (We are using .Net 5.0.)
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            // Following line is an example of how Dependency Injection can be configured
            // It tells the system that whenever someone asks for IEmployeeRepository, provide them an instance of SQLEmployeeRepository class
            // When there are multiple implementations of same interface, this is where dependency gets injected.
            // Notice, we have used AddScoped lifetime of the service while using SQLRepository because according to .Net Documentation,
            // Entity Framework contexts are added to the services container using the Scoped lifetime
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
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
                // For handling Global Exceptions
                app.UseExceptionHandler("/Error");
                // This middleware as opposed to useStatusCodePagesWithRedirect provides proper errors on a HttpRequest.
                // It does not redirect to a given page rather it reverses the execution of middlewares.
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            //app.UseRouting();
            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            
            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response
            //    .WriteAsync("Hello World!");
            //});
        }
    }
}
