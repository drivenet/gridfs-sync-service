﻿using System.Globalization;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Synchronizzer.Middleware
{
    internal sealed class MetricsReportingMiddleware
    {
        private static readonly char[] PathChars = new[] { '/' };

        private readonly RequestDelegate _next;

        private readonly Components.IMetricsReader _metricsReader;

        public MetricsReportingMiddleware(RequestDelegate next, Components.IMetricsReader metricsReader)
        {
            _next = next;
            _metricsReader = metricsReader;
        }

        public async Task Invoke(HttpContext context)
        {
            var metricName = context.Request.Path.Value.TrimStart(PathChars);
            var metricValue = _metricsReader.GetValue(metricName);
            string metricString;
            if (metricValue != null)
            {
                if (context.Request.Method != "GET")
                {
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    return;
                }

                metricString = ((double)metricValue).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                metricString = "0";
            }

            await context.Response.WriteAsync(metricString);
        }
    }
}
