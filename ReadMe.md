# TelemetrySrcGen

A simple source generator to create the boilerplate code needed to report performance metrics via Open Telemetry and App Insights.

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
