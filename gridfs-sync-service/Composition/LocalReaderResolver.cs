﻿using System;

using GridFSSyncService.Components;
using GridFSSyncService.Implementation;

namespace GridFSSyncService.Composition
{
    internal sealed class LocalReaderResolver : ILocalReaderResolver
    {
        private readonly IMetricsWriter _metricsWriter;

        public LocalReaderResolver(IMetricsWriter metricsWriter)
        {
            _metricsWriter = metricsWriter;
        }

        public ILocalReader Resolve(string address)
        {
            if (Uri.TryCreate(address, UriKind.Absolute, out var uri))
            {
                var context = FilesystemUtils.CreateContext(uri);
                return new LocalReader(
                    new CountingObjectSource(
                        new FilesystemObjectSource(context),
                        _metricsWriter,
                        "local.fs"),
                    new FilesystemObjectReader(context));
            }

            if (address.StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase))
            {
                var context = GridFSUtils.CreateContext(address);
                return new LocalReader(
                    new CountingObjectSource(
                        new GridFSObjectSource(context),
                        _metricsWriter,
                        "local.gridfs"),
                    new GridFSObjectReader(context));
            }

            throw new ArgumentOutOfRangeException(nameof(address), "Invalid local address.");
        }
    }
}
