﻿
using System;
using Metrics.Reports;
using Metrics.Visualization;

namespace Metrics
{
    public sealed class MetricsConfig : IDisposable, Utils.IHideObjectMembers
    {
        private readonly MetricContext context;
        private readonly MetricsReports reports;

        private Func<HealthStatus> healthStatus;
        private MetricsHttpListener listener;

        internal MetricsConfig(MetricContext context)
        {
            this.context = context;
            this.reports = new MetricsReports(this.context.MetricsData, this.healthStatus);
            this.healthStatus = () => HealthChecks.GetStatus();
        }

        public T Configure<T>(Func<MetricContext, T> config)
        {
            return config(this.context);
        }

        public void Configure(Action<MetricContext> config)
        {
            config(this.context);
        }

        public void Configure(Action<MetricContext, Func<HealthStatus>> config)
        {
            config(this.context, this.healthStatus);
        }

        public T Configure<T>(Func<MetricContext, Func<HealthStatus>, T> config)
        {
            return config(this.context, this.healthStatus);
        }

        /// <summary>
        /// Create HTTP endpoint where metrics will be available in various formats:
        /// GET / => visualization application
        /// GET /json => metrics serialized as JSON
        /// GET /text => metrics in human readable text format
        /// </summary>
        /// <param name="httpUriPrefix">prefix where to start HTTP endpoint</param>
        public MetricsConfig WithHttpEndpoint(string httpUriPrefix)
        {
            using (this.listener) { }
            this.listener = new MetricsHttpListener(httpUriPrefix, this.context.MetricsData, this.healthStatus);
            this.listener.Start();
            return this;
        }

        /// <summary>
        /// Configure Metrics library to use a custom health status reporter. By default HealthChecks.GetStatus() is used.
        /// </summary>
        /// <param name="healthStatus">Function that provides the current health status.</param>
        /// <returns>Chain-able configuration object.</returns>
        public MetricsConfig WithHealthStatus(Func<HealthStatus> healthStatus)
        {
            this.healthStatus = healthStatus;
            return this;
        }

        /// <summary>
        /// Error handler for the metrics library. If a handler is registered any error will be passed to the handler.
        /// </summary>
        /// <param name="errorHandler">Action with will be executed with the exception.</param>
        /// <returns>Chain able configuration object.</returns>
        public MetricsConfig WithErrorHandler(Action<Exception> errorHandler)
        {
            this.ErrorHandler = errorHandler;
            return this;
        }

        /// <summary>
        /// Configure the way metrics are reported
        /// </summary>
        /// <param name="reportsConfig">Reports configuration action</param>
        /// <returns>Chain-able configuration object.</returns>
        public MetricsConfig WithReporting(Action<MetricsReports> reportsConfig)
        {
            reportsConfig(this.reports);
            return this;
        }

        internal void DisableAllReports()
        {
            this.reports.StopAndClearAllReports();
            using (this.listener) { }
            this.listener = null;
        }

        public void Dispose()
        {
            this.reports.Dispose();
            using (this.listener) { }
            this.listener = null;
        }



        /// <summary>
        /// Configured error handler
        /// </summary>
        internal Action<Exception> ErrorHandler { get; private set; }
    }
}
