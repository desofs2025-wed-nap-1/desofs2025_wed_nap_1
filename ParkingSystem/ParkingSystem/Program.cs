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
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggingConfig) =>
{
    loggingConfig.ReadFrom.Configuration(hostingContext.Configuration);
});

var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var connectionStringName = Environment.GetEnvironmentVariable("CONNECTION_STRING_ID") ?? "DefaultConnection";
var supabaseAPIKey = Environment.GetEnvironmentVariable("SUPABASE_API_KEY")!;
var supabaseJWTToken = Environment.GetEnvironmentVariable("SUPABASE_JWT_VALIDATE_TOKEN")!;

var jwtKeyBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJWTToken));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(connectionStringName)?.Replace("__PASSWORD_PLACEHOLDER__", dbPassword)));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IParkRepository, ParkRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IParkService, ParkService>();

builder.Services.AddScoped<IClaimsTransformation, SupabaseClaimsTransformer>();
builder.Services.AddScoped<SupabaseAuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Parking System API",
        Version = "v1",
        Description = "API for a parking system with 2FA authentication"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. Type 'Bearer' [space] and then your token. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

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
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = jwtKeyBytes
        };
        options.RequireHttpsMetadata = true;
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Auth failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parking System API V1");
    c.RoutePrefix = string.Empty;
});



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
