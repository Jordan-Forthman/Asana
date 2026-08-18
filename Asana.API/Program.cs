
namespace Asana.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Decide where the JSON stores live and create those folders
            // before any request can reach them. Override with Storage:Root in
            // appsettings.json or the ASANA_DATA_DIR environment variable.
            Database.FileStorage.Initialize(
                builder.Configuration["Storage:Root"]
                    ?? Environment.GetEnvironmentVariable("ASANA_DATA_DIR"),
                builder.Environment.ContentRootPath);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
