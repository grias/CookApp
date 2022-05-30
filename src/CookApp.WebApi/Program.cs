using CookApp.Domain.Core;
using CookApp.Domain.DataAccessInterfaces;
using CookApp.Infrastructure.Repositories;
using CookApp.Domain.UtilityClasses;

namespace CookApp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string connectionString = GetConnectionStringFromEnvironmentVariable();

            builder.Services.AddTransient<IRecipeCollection, RecipeCollection>();
            builder.Services.AddTransient<IRecipeRepository>(sp => new RecipeRepository(connectionString));
            builder.Services.AddTransient<ICategoryRepository>(sp => new CategoryRepository(connectionString));
            builder.Services.AddTransient<IDataMapper, DataMapper>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static string GetConnectionStringFromEnvironmentVariable()
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Can't find CONNECTION_STRING env var");
            return connectionString;
        }
    }
}
