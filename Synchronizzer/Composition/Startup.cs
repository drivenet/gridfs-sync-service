﻿using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Synchronizzer.Composition
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
            services.Configure<SyncOptions>(_configuration);
            services.ConfigureOptions<SyncOptionsConfig>();
            services.AddSingleton<Components.MetricsContainer>();
            services.AddSingleton<Components.IMetricsReader>(provider => provider.GetRequiredService<Components.MetricsContainer>());
            services.AddSingleton<Components.IMetricsWriter>(provider => provider.GetRequiredService<Components.MetricsContainer>());
            services.AddHostedService<Implementation.SyncService>();
            services.AddSingleton<ISyncTimeHolderResolver>(SyncTimeHolderResolver.Instance);
            services.AddSingleton<Implementation.ISynchronizer, JobManagingSynchronizer>();
            services.AddSingleton<ISynchronizationJobFactory, SynchronizationJobFactory>();
            services.AddSingleton<IEnumerable<SyncInfo>, SyncInfoResolver>();
            services.AddSingleton<ISynchronizerFactory, SynchronizerFactory>();
            services.AddSingleton<ILocalReaderResolver, LocalReaderResolver>();
            services.AddSingleton<IRemoteWriterResolver, RemoteWriterResolver>();
            services.AddSingleton<IQueuingTaskManagerSelector, QueuingTaskManagerSelector>();
            services.AddSingleton<Implementation.IQueuingSettings, QueuingSettings>();
        }

#pragma warning disable CA1822 // Mark members as static -- future-proofing
        public void Configure(IApplicationBuilder app)
#pragma warning restore CA1822 // Mark members as static
        {
            app.Map("/metrics", builder => builder.UseMiddleware<Middleware.MetricsReportingMiddleware>());
            app.Map("/version", builder => builder.UseMiddleware<Middleware.VersionMiddleware>());
        }
    }
}
