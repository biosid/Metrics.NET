﻿using System;
using Nancy.Metrics;

namespace Metrics
{
    /// <summary>
    /// Helper class to register a few NancyFx related metrics.
    /// </summary>
    public static class NancyMetrics
    {
        internal static NancyMetricsConfig CurrentConfig { get; private set; }

        internal static MetricContext Context { get { return CurrentConfig.Context; } }

        /// <summary>
        /// Configure NancyFx integration
        /// </summary>
        /// <param name="config">Chainable configuration object.</param>
        /// <returns>Chainable configuration object.</returns>
        public static MetricsConfig WithNancy(this MetricsConfig config)
        {
            return config.WithNancy(c => { });
        }

        public static MetricsConfig WithNancy(this MetricsConfig config, Action<NancyMetricsConfig> nancyConfig, string contextName = "NancyFx")
        {
            CurrentConfig = config.Configure((ctx, hs) => new NancyMetricsConfig(ctx.Context(contextName), hs));
            nancyConfig(CurrentConfig);
            return config;
        }
    }
}
