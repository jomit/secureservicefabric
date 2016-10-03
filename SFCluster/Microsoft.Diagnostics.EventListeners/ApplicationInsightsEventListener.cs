// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Diagnostics.EventListeners
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;

    public class ApplicationInsightsEventListener : BufferingEventListener, IDisposable
    {
        private const string AppInsightsKeyName = "InstrumentationKey";
        private readonly TelemetryClient telemetry;

        public ApplicationInsightsEventListener(IConfigurationProvider configurationProvider, IHealthReporter healthReporter)
            : base(configurationProvider, healthReporter)
        {
            if (this.Disabled)
            {
                return;
            }

            Debug.Assert(configurationProvider != null);

            this.telemetry = new TelemetryClient();
            this.telemetry.Context.InstrumentationKey = configurationProvider.GetValue(AppInsightsKeyName);

            this.Sender = new ConcurrentEventSender<EventData>(
                eventBufferSize: 1000,
                maxConcurrency: 2,
                batchSize: 100,
                noEventsDelay: TimeSpan.FromMilliseconds(1000),
                transmitterProc: this.SendEventsAsync,
                healthReporter: healthReporter);
        }

        private Task SendEventsAsync(IEnumerable<EventData> events, long transmissionSequenceNumber, CancellationToken cancellationToken)
        {
            Task<int> completedTask = Task.FromResult(0);

            if (events == null)
            {
                return completedTask;
            }

            try
            {
                foreach (EventData e in events)
                {
                    Dictionary<string, string> properties = new Dictionary<string, string>
                    {
                        {nameof(e.EventName), e.EventName},
                        {nameof(e.EventId), e.EventId.ToString()},
                        {nameof(e.Keywords), e.Keywords},
                        {nameof(e.Level), e.Level},
                        {nameof(e.Message), e.Message},
                        {nameof(e.ProviderName), e.ProviderName}
                    };


                    foreach (KeyValuePair<string, object> item in e.Payload)
                    {
                        properties.Add(item.Key, item.Value.ToString());
                    }

                    this.telemetry.TrackEvent(e.EventName, properties);
                }
                this.telemetry.Flush();
                this.ReportListenerHealthy();
            }
            catch (Exception e)
            {
                this.ReportListenerProblem("Diagnostics data upload has failed." + Environment.NewLine + e.ToString());
            }

            return completedTask;
        }
    }
}