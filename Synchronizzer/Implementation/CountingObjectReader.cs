﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Synchronizzer.Components;

namespace Synchronizzer.Implementation
{
    internal sealed class CountingObjectReader : IObjectReader
    {
        private readonly IObjectReader _inner;
        private readonly IMetricsWriter _writer;
        private readonly string _prefix;

        public CountingObjectReader(IObjectReader inner, IMetricsWriter writer, string key)
        {
            _inner = inner;
            _writer = writer;
            _prefix = "reader.";
            if (key.Length != 0)
            {
                _prefix = _prefix + key + ".";
            }
        }

        public async Task<Stream?> Read(string objectName, CancellationToken cancellationToken)
        {
            Stream? result;
            try
            {
                result = await _inner.Read(objectName, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                _writer.Add(_prefix + "read_errors", 1);
                throw;
            }

            _writer.Add(_prefix + "reads", 1);
            if (result is object)
            {
                _writer.Add(_prefix + "reads_length", result.Length);
            }

            return result;
        }
    }
}