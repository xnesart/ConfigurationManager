using ServiceConfigManager.Api.Extensions;
using ServiceConfigManager.Bll;
using ServiceConfigManager.Core.Mapping;

using Serilog;
using ServiceConfigManager.DataLayer;

namespace ServiceConfigManager.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureApiServices(builder.Configuration);
        builder.Services.ConfigureBllServices();
        builder.Services.ConfigureDalServices();
        builder.Services.AddAutoMapper(typeof(RequestMapperProfile));
        
        builder.Host.UseSerilog();
        builder.Services.AddAuthorization();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }
}