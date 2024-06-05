# TelemetrySrcGen

A simple source generator to create the boilerplate code needed to report performance metrics via Open Telemetry and App Insights.

## Usage

```csharp

[TelemetrySource]
public partial class MyIngestionTelemetry : TelemetrySource
{
  [TelemetryProperty]
  public double IngestionTime { get; set; }

  [Timer]
  public void Ingestion();
}

```