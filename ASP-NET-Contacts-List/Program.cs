using ASP_NET_Contacts_List.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ASP_NET_Contacts_List
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // configuring serilog logger to log to a file
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/ContactsLogs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });


            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddNewtonsoftJson();
          
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = string.Empty;
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Name of Your API v1");
                });
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}