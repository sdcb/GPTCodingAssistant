using GPTCodingAssistant.Hubs;

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