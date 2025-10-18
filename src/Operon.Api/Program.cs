using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Operon.Api.Endpoints;
using Operon.Api.Extensions;
using Operon.Api.Middlewares;
using Operon.Application.Extensions;
using Operon.Application.Features.Dtos;
using Operon.Application.Features.Validators;
using Operon.Application.Handlers;
using Operon.Infrastructure;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

var cfg = builder.Configuration.GetSection("IdentitySettings");

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Operon.Application.Handlers.GetTodosQuery).Assembly));

//// Add API versioning
//builder.Services.AddApiVersioning(options =>
//{
//    options.AssumeDefaultVersionWhenUnspecified = true; // default version if not specified
//    options.DefaultApiVersion = new ApiVersion(1, 0); // default version 1.0
//    options.ReportApiVersions = true; // adds "api-supported-versions" header in responses

//    // Optional: accept version in query string or header
//    options.ApiVersionReader = ApiVersionReader.Combine(
//        new QueryStringApiVersionReader("v"),          // /endpoint?v=1.0
//        new HeaderApiVersionReader("x-api-version")    // or header x-api-version
//    );
//});

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddTokenBucketLimiter("api", options =>
    {
        options.TokenLimit = 50;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        options.TokensPerPeriod = 50;
        options.AutoReplenishment = true;
    });
});

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueLimit = 0;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = cfg["Jwt:Issuer"],
          ValidAudience = cfg["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]))
      };
  });

builder.Services.AddAuthorization();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
//});

// Redis IDistributedCache already added via AddStackExchangeRedisCache
builder.Services.AddHealthChecks();

// OPTIONAL: FastEndpoints
//builder.Services.AddFastEndpoints().SwaggerDocument(); ;

//builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoDto>();
builder.Services.AddValidatorsFromAssembly(typeof(CreateTodoDto).Assembly);

builder.Services.AddCorsServices(builder.Configuration);
//builder.Services.AddCors();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OperonDbContext>();
    dbContext.Database.Migrate();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
//app.UseExceptionHandler();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseCors();

app.UseCorsServices(builder.Configuration);
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok("Operon API running"));

TodoEndpoints.Map(app);
app.Run();

