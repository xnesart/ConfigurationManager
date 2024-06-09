using System.Text;
using ConfigurationManager.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ConfigurationManager.Api.Extensions;

public static class ConfigurationForServices
{
    public static void ConfigureApiServices(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configurationManager)
    {
        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "ProjectBackendLearning",
                    ValidAudience = "UI",
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345fffffa43534534523dsf"))
                };
            });

        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.ConfigureDataBase(configurationManager);
    }
    
    public static void ConfigureDataBase(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configurationManager)
    {
        services.AddDbContext<DataBaseContext>(
            options => options
                .UseNpgsql(configurationManager.GetConnectionString("MypConnection"))
                .UseSnakeCaseNamingConvention());
    }
}