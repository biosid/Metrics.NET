﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Metrics.Json;
using Newtonsoft.Json;

namespace Metrics.Central
{
    public class MetricsService
    {
        private const string remotesFile = "remotes.txt";

        public bool Start()
        {
            Metric.Config
                .WithJsonDeserializer(JsonConvert.DeserializeObject<JsonMetricsContext>)
#if NET45
                .WithAllCounters();
#else
                .WithHttpEndpoint("");
#endif
            var remotes = ReadRemotesFromConfig();

            foreach (var uri in remotes)
            {
                Metric.Config.RegisterRemote(uri.ToString(), uri, TimeSpan.FromSeconds(1));
            }

            return true;
        }

        private IEnumerable<Uri> ReadRemotesFromConfig()
        {
            if (!File.Exists(remotesFile))
            {
                yield break;
            }

            var remotes = File.ReadAllLines("remotes.txt")
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => !l.StartsWith("#"));

            foreach (var remote in remotes)
            {
                Uri uri = null;
                try
                {
                    uri = new Uri(remote, UriKind.Absolute);
                }
                catch (Exception x)
                {
                    MetricsErrorHandler.Handle(x, "Unable to read uri from remotes.txt file");
                }

                if (uri != null)
                {
                    yield return uri;
                }
            }
        }

        public bool Stop()
        {
            return true;
        }
    }
}
