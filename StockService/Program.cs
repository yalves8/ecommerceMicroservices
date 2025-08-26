
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

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(80); // container: porta 80
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer(); // necessário para Minimal API
            builder.Services.AddSwaggerGen(); // ativa o Swagger

            // Configuração do EF Core
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<StockDbContext>(opt => opt.UseSqlServer(conn));

            // BackgroundService do RabbitMQ (consumidor)
            builder.Services.AddHostedService<SalesEventsConsumer>();

           

            var app = builder.Build();

            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockService API V1");
                    c.RoutePrefix = string.Empty; 
                });
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => Results.Ok("StockService OK"));

            app.Run();
        }
    }
}
