# DESOFS2025_WED_NAP_1 - Phase 2 - Sprint 2

This `README` file contains all information about developments that were done during the 2nd Sprint of the 2nd Phase of
the DESOFS course unit Project.

## Development

### Authentication - MFA

### Authorization - RBAC

In this sprint we implemented authorization for our use-cases, meaning that each use case can only be executed by the role(s) permitted to do it. This heavily relies on Supabase, which is the service we chose for authentication and authorization handling.

During user creation - notably, in the [`UserService`](../../../ParkingSystem/ParkingSystem/ParkingSystem.Application/Services/UserService.cs) - the [`IAuthenticationService`](../../../ParkingSystem/ParkingSystem/ParkingSystem.Application/Interfaces/ISupabaseAuthService.cs) interface is called. This interface maps to the actual [`SupabaseAuthenticationService`](../../../ParkingSystem/ParkingSystem/ParkingSystem.Application/Services/SupabaseAuthService.cs) via dependency injection. In this iteration, this service was changed to include a `CreateUser` function, which persists the desired user in the Supabase database. This user creation also populates the `user_metadata` key of the request with the user's role, which is also persisted.

This `user_metadata` is then included in JWT tokens, which are used for authentication and consequently authorization. However, our code needs to be able to interpret this metadata, which by default it isn't. To accomplish this, a custom class [`SupabaseClaimsTransformer`](../../../ParkingSystem/ParkingSystem/ParkingSystem.Infrastructure/Authentication/SupabaseClaimsTransformer.cs) was developed. What it does is check the claim fields present in the token and, if `user_metadata` is present, it sources the role from that JSON document and sets it as a claim in the token. This makes it so that the user role - which is included in a non-default field by Supabase - is interpreted for authorization. 

Then, all that's necessary is to add the `Authorize` annotation to Controller methods - which receive requests from the client side. This was done for all implemented use cases. Below is a practical example (it's choice was arbitrary, since this is present on all use cases):

```C#
[HttpPost("add")]
[Authorize(Roles = RoleNames.Client)]
public async Task<IActionResult> AddVehicleAsync(VehicleDTO vehicleDto)
```

Having this, RBAC is operational on the defined endpoints. If a user doesn't provide an Authorization: Bearer header with a token, a 401 error code is returned and logged by the backend:

![authNoTokenProvided.png](./img/authNoTokenProvided.png)

If a user does provide a valid authentication token, but is not allowed to access a specific endpoint, a 403 error code is returned and logged:

![authNoPermission.png](./img/authNoPermission.png)

It's also worth noting that tokens are validated before validating RBAC permissions. This is configued via the following block in [`Program.cs`](../../../ParkingSystem/ParkingSystem/Program.cs):

```C#
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllRolesExceptUnauthenticated", policy =>
    {
        policy.RequireRole(RoleNames.Client, RoleNames.ParkManager, RoleNames.Admin);
    });
});
```

As an example, below we present the logs - and, consequently, the response - of a request made using an expired token:

![authJWTOutOfDate.png](./img/authJWTOutOfDate.png)

This showcases how RBAC authorization controls were implemented in our project.

### Developed Use Cases

### Logging Mechanism

An addition for this development iteration was the implementation of a **logging mechanism** to be used throughout the code. In [`Program.cs`](../../../ParkingSystem/ParkingSystem/Program.cs), we configure the usage of Serilog, an external logging library:

```C#
builder.Host.UseSerilog((hostingContext, loggingConfig) =>
{
    loggingConfig.ReadFrom.Configuration(hostingContext.Configuration);
});
```

This loads logging configurations from our [`appsettings.json`](../../../ParkingSystem/ParkingSystem/appsettings.json):

```JSON
"Serilog": {
    "MinimumLevel": "Information",
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "Enrich": "FromLogContext",
    "WriteTo": [
        { "Name": "Console" },
        { "Name": "File", 
        "Args": {
            "path": "./app.log",
            "rollingInterval": "Day"
        }
        }
    ]
}
```

We chose to use an external library because of its features of logging to files, most notably log rotation - in our case, per day, but this could easily be changed. Additionally, being able to configure the logging mechanism on the `appsettings.json` presents an advantage when considering the future-proofing of the applciation, as requirements could change.

Having this implemented, logs are written to a file in the application's directory, with the name of each log file being prefixed with `app`, and the current date is appended to it.

With this setup, logs are written to the file. However, the code also had to be changed to actually make use of the logging functionalities, so that we can log our custom messages to comply with requirements. For that effect, the logger is injected via DI into each class. Below we present an example for [`ParkService`](../../../ParkingSystem/ParkingSystem/ParkingSystem.Application/Services/ParkService.cs), although this is implemented in several other classes:

```C#
public ParkService(IParkRepository parkRepository, ILogger<ParkService> logger)
{
    _parkRepository = parkRepository;
    _logger = logger;
}
```

With the logger instantiated, it can then be used to log messages in the class. For example (again in [`ParkService`](../../../ParkingSystem/ParkingSystem/ParkingSystem.Application/Services/ParkService.cs), although this was chosen arbitrarily):

```C#
_logger.LogInformation("Fetching available parks");
try
{
    var parks = await _parkRepository.GetAvailableParks();
    var parkDtos = new List<ParkDTO>();
    foreach (var park in parks)
    {
        parkDtos.Add(ParkMapper.ToParkDto(park));
    }
    _logger.LogInformation("Found {Count} available parks", parkDtos.Count);
    return parkDtos;
}
catch (Exception ex)
{
    _logger.LogError("Error fetching Parks: " + ex.Message);
    // preserve the exception after logging - it's caught on the controller for return purposes
    throw;
}
```

These messages are then appropriately written to the specified log file:

```log
2025-06-04 16:52:17.198 +01:00 [INF] Found 3 available parks
2025-06-04 16:52:17.198 +01:00 [INF] Successfully gathered available parks
```



## ASVS Checklist

For this Phase and Sprint, we completed the ASVS checklist, highlighting the applicability and validity of each item with a comprehensive explanation of how it relates to our application. This artifact can be found in [`v4-ASVS-checklist-en-phase2-sprint2.xlsx`](v4-ASVS-checklist-en-phase2-sprint2.xlsx).

Below, we present the summary of this checklist in the form of a table and the generated graph:
<!-- 
![asvs-phase2-sprint2-table.png](./img/asvs-phase2-sprint2-table.png)

![asvs-phase2-sprint2-graph.png](./img/asvs-phase2-sprint2-graph.png) -->
