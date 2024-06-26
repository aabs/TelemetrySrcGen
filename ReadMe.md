﻿# TelemetrySrcGen

A simple source generator to create the boilerplate code needed to report performance metrics via App Insights.

TelemetrySrcGen lets you define telemetry metrics in the form of simple fields and the source generator creates the methods to send metrics to app insights for you.

## Usage

```csharp
using Microsoft.ApplicationInsights;

[TelemetrySource]
public partial class MyIngestionTelemetry
{
    private TelemetryClient telemetryClient;
    public MyIngestionTelemetry(TelemetryClient telemetryClient)
    {
        this.telemetryClient = telemetryClient;
    }

    [Measurement(MetricKind.Duration)]
    public bool IngestionProcess ;

    [Measurement(MetricKind.Duration)]
    public bool DeliveryTime;

    [Measurement(MetricKind.Counter)]
    public double DataPointsReceived;

    [Measurement(MetricKind.Operation)]
    public bool CallAPI;
    
    [Measurement(MetricKind.Event)]
    public partial void OnPollingSucceeded(IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null); 
}
```

Will Augment your class with some extra methods to drive App Insights.

```csharp
// Generated on 2024-06-07
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS0108 // hides inherited member.

using Microsoft.ApplicationInsights;
namespace TelemetrySrcGen.Playground;

using Microsoft.ApplicationInsights.DataContracts;
public partial class MyIngestionTelemetry
{
    public void AddDataPointsReceived(Double value) => telemetryClient.TrackMetric("DataPointsReceived.Counter", value);
    public void StartCallAPI() 
      => telemetryClient.StartOperation<RequestTelemetry>("CallAPI");
    public IDisposable RecordIngestionProcessDuration(IDictionary<string, string> properties = null)
    {
        return new TelemetrySrcGen.Abstractions.StopwatchDisposable(this.telemetryClient, "IngestionProcess.Duration", properties);
    }
    public IDisposable RecordDeliveryTimeDuration(IDictionary<string, string> properties = null)
    {
        return new TelemetrySrcGen.Abstractions.StopwatchDisposable(this.telemetryClient, "DeliveryTime.Duration", properties);
    }
}
```
