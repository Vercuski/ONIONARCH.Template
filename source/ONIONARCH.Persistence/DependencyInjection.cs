using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Options;
using ONIONARCH.Persistence.Contexts;

namespace ONIONARCH.Persistence;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPersistenceRegistrations(this IHostApplicationBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var connectionStringOptions = serviceProvider.GetService<IOptions<ConnectionStringOptions>>()!.Value;
        builder.Services.AddDbContext<SampleCommandDbContext>(options =>
            options
                .UseSqlServer(connectionStringOptions.CommandDbConnection)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging(), ServiceLifetime.Transient
        );
        builder.Services.AddDbContext<SampleQueryDbContext>(options =>
            options
                .UseSqlServer(connectionStringOptions.QueryDbConnection)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging(), ServiceLifetime.Transient
        );

        builder.Services.AddTransient<ICommandDbContext>(serviceProvider => serviceProvider.GetRequiredService<SampleCommandDbContext>());
        builder.Services.AddTransient<IQueryDbContext>(serviceProvider => serviceProvider.GetRequiredService<SampleQueryDbContext>());
        builder.Services.AddTransient<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<SampleCommandDbContext>());

        return builder;
    }
}
