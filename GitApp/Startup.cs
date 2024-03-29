using System.Globalization;
using GitApp.Hubs;
using GitApp.Models.Db;
using GitApp.Repositories;
using GitApp.Services;
using GitTool;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GitApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddCors(options => options.AddPolicy(
                "CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader()
                        .WithOrigins("http://localhost:55830")
                        .AllowCredentials();
                }));

            RegisterDependencies(services);
            services.AddSignalR();
            services.AddMvc()
                .AddViewLocalization();
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

                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            });
            
            app.UseRouting();
            
            ConfigureSignalR(app);
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient<IGitService, GitService>();
            services.AddTransient<IVcsFilesRepository, VcsFilesRepository>();
            services.AddTransient<IDbVcsRepositoryRepository, DbVcsRepositoryRepository>();
            services.AddTransient<IRepositoryWorker, RepositoryWorker>();
        }

        private void ConfigureSignalR(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            app.UseEndpoints(routes => { routes.MapHub<UploadStatusHub>("/UploadStatus"); });
        }
    }
}