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

    public partial void CallApi();
}