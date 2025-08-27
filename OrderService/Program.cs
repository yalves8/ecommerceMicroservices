
using Microsoft.EntityFrameworkCore;
using OrderService.OrderService.Infrastructure.Data;
using OrderService.OrderService.Messaging;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(80);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); 

            // EF Core
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(conn));
            builder.Services.AddSingleton<IMessageBusClient, RabbitMQMessageBusClient>();

            var app = builder.Build();

            // middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                try
                {
                    db.Database.Migrate();
                    Console.WriteLine("Database migrated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error migrating database: {ex.Message}");
                }
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => Results.Ok("OrderService OK"));

            app.Run();
        }
    }
}
