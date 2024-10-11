using ArmyGrievances.Generics;
using ArmyGrievances.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace ArmyGrievances
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigurationBuilder configBuilderForMain = new ConfigurationBuilder();
            ConfigureConfiguration(configBuilderForMain);
            IConfiguration configForMain = configBuilderForMain.Build();
            //var host = new WebHostBuilder()
            //.ConfigureAppConfiguration(ConfigureConfiguration).Build();
            var builder = WebApplication.CreateBuilder(args);
            //host.Run();
            // Add services to the container.
            builder.Services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddTransient<ICommonGenericFunction, CommonGenericFunction>();
            builder.Services.AddTransient<IOperationRepository, OperationRepository>();
            var connectionString = builder.Configuration.GetConnectionString("DB_Connection");
            builder.Services.AddDbContext<AD_DBContext>(x => x.UseSqlServer(connectionString));
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

            app.MapControllerRoute(
                name: "Home",
                pattern: "{controller=Home}/{action=DashBoard}/{id?}");

            app.Run();
        }
        public static void ConfigureConfiguration(IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        }

    }
}
