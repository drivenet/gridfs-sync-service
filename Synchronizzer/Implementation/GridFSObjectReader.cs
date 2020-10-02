﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver.GridFS;

namespace Synchronizzer.Implementation
{
    internal sealed class GridFSObjectReader : IObjectReader
    {
        private readonly GridFSContext _context;

        public GridFSObjectReader(GridFSContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Stream?> Read(string objectName, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Bucket.OpenDownloadStreamByNameAsync(
                    objectName,
                    new GridFSDownloadByNameOptions { Seekable = true },
                    cancellationToken);
            }
            catch (GridFSFileNotFoundException)
            {
                return null;
            }
        }
    }
}
