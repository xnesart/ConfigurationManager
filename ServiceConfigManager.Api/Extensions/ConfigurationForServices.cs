using ServiceConfigManager.Api.Configuration.Filters;
using ServiceConfigManager.Bll.Services;

namespace ServiceConfigManager.Api.Extensions;

public static class ConfigurationForServices
{
    public static void ConfigureApiServices(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        // services.AddAuthentication(opt =>
        //     {
        //         opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //         opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //     })
        //     .AddJwtBearer(options =>
        //     {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidateAudience = true,
        //             ValidateLifetime = true,
        //             ValidateIssuerSigningKey = true,
        //             ValidIssuer = "ProjectBackendLearning",
        //             ValidAudience = "UI",
        //             IssuerSigningKey =
        //                 new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345fffffa43534534523dsf"))
        //         };
        //     });

        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.ConfigureDataBase(configurationManager);
        services.AddTransient<IIpBlockingService, IpBlockingService>();
        services.AddScoped<IpBlockActionFilter>();
    }
}