﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Synchronizzer.Composition
{
    internal sealed class SyncInfoResolver : IEnumerable<SyncInfo>
    {
        private readonly IOptionsMonitor<SyncOptions> _options;
        private readonly ILogger _logger;

        public SyncInfoResolver(IOptionsMonitor<SyncOptions> options, ILogger<SyncInfoResolver> logger)
        {
            _options = options;
            _logger = logger;
        }

        public IEnumerator<SyncInfo> GetEnumerator()
            => (_options.CurrentValue.Jobs ?? Array.Empty<SyncJob>())
                .Select(CreateSyncInfo)
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static SyncInfo CreateSyncInfo(SyncJob job)
            => new SyncInfo(
                job.Name ?? throw new ArgumentNullException(nameof(job), "Invalid sync job name."),
                job.Local ?? throw new ArgumentNullException(nameof(job), FormattableString.Invariant($"Missing sync job local address for job \"{job.Name}\".")),
                job.Remote ?? throw new ArgumentNullException(nameof(job), FormattableString.Invariant($"Missing sync job remote address for job \"{job.Name}\".")),
                job.Recycle);
    }
}
