﻿using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GridFSSyncService.Composition
{
    internal sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOptions<SyncOptionsConfig>();
            services.AddSingleton<Components.IMetricsReader>(Components.NullMetricsReader.Instance);
            services.AddSingleton<Components.ITimeSource>(Components.SystemTimeSource.Instance);
            services.AddSingleton<Components.MetricsContainer>();
            services.AddSingleton<Components.IMetricsReader>(provider => provider.GetRequiredService<Components.MetricsContainer>());
            services.AddSingleton<Components.IMetricsWriter>(provider => provider.GetRequiredService<Components.MetricsContainer>());
            services.AddHostedService<Implementation.SyncService>();
            services.AddSingleton(Implementation.SyncTimeHolder.Instance);
            services.AddSingleton<Implementation.ISynchronizer, ScopingSynchronizer>();
            services.AddScoped<Implementation.ISynchronizer, CompositeSynchronizer>();
            services.AddScoped<IEnumerable<Implementation.ISynchronizer>, SynchronizerSource>();
            services.AddSingleton<ISynchronizerBuilder, SynchronizerBuilder>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Map("/metrics", builder => builder.UseMiddleware<Middleware.MetricsReportingMiddleware>());
            app.Map("/version", builder => builder.UseMiddleware<Middleware.VersionMiddleware>());
        }
    }
}
