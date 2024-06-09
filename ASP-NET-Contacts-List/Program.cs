using ASP_NET_Contacts_List.Data;
using ASP_NET_Contacts_List.Models;
using ASP_NET_Contacts_List.Models.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace ASP_NET_Contacts_List
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UsersOnly", policy =>
                {
                    policy.RequireRole("User");
                });
            });

            //add cookie authentication and authorization services
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            builder.Services.AddAuthorization();




            // pipeline
            var app = builder.Build();




            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwaggerUI(c =>
            //    {
            //        c.RoutePrefix = string.Empty;
            //        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactsAPI v1");
            //    });
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");



            app.Run();
        }
    }
}