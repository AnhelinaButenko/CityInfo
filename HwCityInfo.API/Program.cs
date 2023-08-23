using HwCityInfo.API.DbContexts;
using HwCityInfo.API.Servises;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

namespace HwCityInfo.API;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File("logs/hwcityinfo.txt", rollingInterval: RollingInterval.Month) //logging is written here in this file
        .CreateLogger();

        var builder = WebApplication.CreateBuilder(args); //application is being built here

        builder.Host.UseSerilog(); //call will redirect all log events through your Serilog pipeline

        // Add services to the container.
        // This call registers services that are typiically required when building APIs,
        // like support for controllers, model binding, data annotations and formatters.
        builder.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        }).AddNewtonsoftJson()
            .AddXmlDataContractSerializerFormatters(); //adds XML input and output formatters to our API.

        // These statements register the required services on the container that
        // are needed Swagger implementation.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
        builder.Services.AddTransient<IMailService, LocalMailService>(); //call Transient create each time they are requeste
                                                                         //// i set up servises collection of the application. It is essential part of applicatian dependency injection container.
#else
        builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

        builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(
            builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

        builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>(); //register repository 
                                                                               //throught the contract ICityInfoRepository and implementation CityInfoRepository

        // Using AutoMapper greatly reduces error - prone mapping code
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MustBeFromAntwerp", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("city", "Antwerp");
            });
        });

        builder.Services.AddApiVersioning(setupAction =>
        {
            setupAction.AssumeDefaultVersionWhenUnspecified = true;
            setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            setupAction.ReportApiVersions = true;
        });

        // When all these services have been registered and potentially configured
        // the web application can build.
        var app = builder.Build();

        //Configure the HTTP request pipeline.           
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();     //those ensure that a request to Swaggers index page
                                  //will be handled by the code generate the documentation UI.   
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection(); // a few middleware are being added that will potentially
                                   // result in documentation being shown 

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization(); // HTTP calls being redirected to HTTPS, authorization being set up
                                  // or mappings to other parts of our code being set up.

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // it adds endpoints for controller action without specifying routes
                                        // (what we will do with attributes).
        });

        app.Run(); //Runs the application's standard message loop on the current thread, without a form
    }
}