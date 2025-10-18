using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

namespace Operon.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OperonDbContext>(o =>
            o.UseSqlServer(config.GetConnectionString("Sql") ?? "Data Source=.\\MSSQLSERVER19;Initial Catalog=Operon.Main;Integrated Security=True;Max Pool Size=5000;MultipleActiveResultSets=True"));

        services.AddSingleton<IMongoClient>(_ => new MongoClient(config["Mongo:ConnectionString"] ?? "mongodb://localhost:27017"));
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(config["Mongo:Database"] ?? "Operon");
        });

        services.AddStackExchangeRedisCache(o =>
            o.Configuration = config["Redis:ConnectionString"] ?? "localhost:6379");

        services.AddHttpClient("external")
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreaker());

        return services;
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(1)
            });

    static IAsyncPolicy<HttpResponseMessage> GetCircuitBreaker() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
