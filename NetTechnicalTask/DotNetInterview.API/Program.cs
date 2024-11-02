using DotNetInterview.API;
using DotNetInterview.API.Data;
using DotNetInterview.API.Repositories;
using DotNetInterview.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class Program // Make sure this is public
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connection = new SqliteConnection("Data Source=DotNetInterview;Mode=Memory;Cache=Shared");
        connection.Open();
        builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connection));

        // Register repository and services
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddScoped<IItemService, ItemService>();

        // Configure the HTTP request pipeline.
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.Run();
    }
}
