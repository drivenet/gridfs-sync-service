﻿using System;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GridFSSyncService.Composition
{
    internal sealed class SyncOptionsConfig : IConfigureOptions<SyncOptions>, IPostConfigureOptions<SyncOptions>
    {
        private static readonly Regex UriVars = new Regex(@"\$(\w+)\$", RegexOptions.CultureInvariant);

        private readonly IConfiguration _configuration;

        public SyncOptionsConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(SyncOptions options)
        {
            _configuration.Bind(options);
        }

        public void PostConfigure(string name, SyncOptions options)
        {
            var jobs = options.Jobs;
            if (jobs == null)
            {
                return;
            }

            var template = _configuration.GetSection("vars");
            foreach (var job in jobs)
            {
                job.Local = ReplaceAddress(job.Local, template);
                job.Remote = ReplaceAddress(job.Remote, template);
                job.Recycle = ReplaceAddress(job.Recycle, template);
            }
        }

        private static string? ReplaceAddress(string? address, IConfiguration template)
        {
            if (address is object)
            {
                address = UriVars.Replace(
                    address,
                    match =>
                    {
                        var key = match.Groups[1].Value;
                        return template.GetValue(key, key);
                    });
            }

            return address;
        }
    }
}
