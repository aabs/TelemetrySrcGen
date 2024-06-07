using Microsoft.ApplicationInsights;

namespace TelemetrySrcGen.Playground;

/// <summary>
/// Represents the telemetry for ingestion process.
/// </summary>
[TelemetrySource]
public partial class MyIngestionTelemetry
{
    private TelemetryClient telemetryClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyIngestionTelemetry"/> class.
    /// </summary>
    /// <param name="telemetryClient">The telemetry client.</param>
    public MyIngestionTelemetry(TelemetryClient telemetryClient)
    {
        this.telemetryClient = telemetryClient;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ingestion process is running.
    /// </summary>
    [Measurement(MetricKind.Duration)]
    public bool IngestionProcess ;

    /// <summary>
    /// Gets or sets the delivery time.
    /// </summary>
    [Measurement(MetricKind.Duration)]
    public bool DeliveryTime;
    /// <summary>
    /// Gets or sets the number of data points received.
    /// </summary>
    [Measurement(MetricKind.Counter)]
    public double DataPointsReceived;
    /// <summary>
    /// Gets or sets a value indicating whether the API call is made.
    /// </summary>
    [Measurement(MetricKind.Operation)]
    public bool CallAPI;
    //[Measurement(MetricKind.Event)]
    //public partial void OnPollingSucceeded(IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null); 
}
