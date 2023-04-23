using GPTCodingAssistant.DB;
using GPTCodingAssistant.Hubs;
using GPTCodingAssistant.Services;
using Microsoft.EntityFrameworkCore;

namespace GPTCodingAssistant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR(x =>
            {
                x.EnableDetailedErrors = true;
            });
            builder.Services.AddSingleton<IPAccessor>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<ChatGPTDB>(x =>
            {
                x.UseSqlServer(builder.Configuration["ChatGPTSqlServerConnectionString"]);
            });
            builder.Services.AddScoped<ChatGPTRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");

            app.MapHub<ChatHub>("/chatHub");
            app.MapFallbackToFile("index.html");
            app.Run();
        }
    }
}