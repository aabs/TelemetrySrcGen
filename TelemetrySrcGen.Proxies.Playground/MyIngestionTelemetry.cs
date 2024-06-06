using Microsoft.ApplicationInsights;

namespace TelemetrySrcGen.Abstractions.Playground;

[TelemetrySource]
public partial class MyIngestionTelemetry
{
    private TelemetryClient telemetryClient;

    public MyIngestionTelemetry(TelemetryClient telemetryClient)
    {
        this.telemetryClient = telemetryClient;
    }

    [Measurement(MetricKind.Duration)]
    public bool IngestionProcess;

    [Measurement(MetricKind.Duration)]
    public bool DeliveryTime;

    [Measurement(MetricKind.Counter)]
    public double DataPointsReceived;

    [Measurement(MetricKind.Operation)]
    public bool CallApi;

    [Measurement(MetricKind.Event)]
    public partial void OnPollingSucceeded(IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
}