
using Microsoft.EntityFrameworkCore;
using StockService.Infrastructure;
using StockService.Messaging;

namespace StockService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuração do EF Core
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<StockDbContext>(opt => opt.UseSqlServer(conn));

            // Controllers
            builder.Services.AddControllers();

            // BackgroundService do RabbitMQ (consumidor)
            builder.Services.AddHostedService<SalesEventsConsumer>();

            var app = builder.Build();

            // Aplica migrações automaticamente na subida
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
                db.Database.Migrate();
            }

            app.MapControllers();
            app.MapGet("/", () => Results.Ok("StockService OK"));

            app.Run();
        }
    }
}
