﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Amazon.S3;

namespace Synchronizzer.Implementation
{
    internal sealed class CancelationHandlingS3Mediator : IS3Mediator
    {
        private readonly IS3Mediator _inner;

        public CancelationHandlingS3Mediator(IS3Mediator inner)
        {
            _inner = inner;
        }

        public string ServiceUrl => _inner.ServiceUrl;

        public async Task<TResult> Invoke<TResult>(Func<IAmazonS3, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
        {
            try
            {
                return await _inner.Invoke(action, cancellationToken);
            }
            catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException("S3 operation timed out.", exception);
            }
        }
    }
}
