using ParkingSystem.Application.Interfaces;
using ParkingSystem.Application.Services;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ParkingSystem.Infrastructure.Data;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using dotenv.net;
using Microsoft.AspNetCore.Authentication;
using ParkingSystem.Infrastructure.Authentication;
using System.Net.Http.Headers;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// builder.Logging.ClearProviders();
// Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().WriteTo.File("app.log").CreateLogger();
builder.Host.UseSerilog((hostingContext, loggingConfig) =>
{
    loggingConfig.ReadFrom.Configuration(hostingContext.Configuration);
});

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var connectionStringName = Environment.GetEnvironmentVariable("CONNECTION_STRING_ID") ?? "DefaultConnection";

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString(connectionStringName)?.Replace("__PASSWORD_PLACEHOLDER__", dbPassword)));

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString(connectionStringName)?.Replace("__PASSWORD_PLACEHOLDER__", dbPassword)));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IParkRepository, ParkRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IParkService, ParkService>();

builder.Services.AddTransient<IClaimsTransformation, SupabaseClaimsTransformer>();

builder.Services.AddScoped<SupabaseAuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Supabase:Url"] + "/auth/v1";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Supabase:Url"] + "/auth/v1",
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", builder.Configuration["Supabase:ApiKey"]);
                // or if Supabase expects the 'apikey' header:
                client.DefaultRequestHeaders.Add("apikey", builder.Configuration["Supabase:ApiKey"]);

                var response = client.GetAsync(builder.Configuration["Supabase:Url"] + "/auth/v1/keys").Result;
                response.EnsureSuccessStatusCode();

                var jwksJson = response.Content.ReadAsStringAsync().Result;
                var jwks = new JsonWebKeySet(jwksJson);

                return jwks.Keys;
            }
        };
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();