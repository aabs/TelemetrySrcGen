namespace TelemetrySrcGen.Abstractions.Playground;

[TelemetrySource]
public partial class MyIngestionTelemetry
{
  [TelemetryProperty]
  public double IngestionTime { get; set; }

  [Timer]
  public double IngestionDuration { get; set; }
}