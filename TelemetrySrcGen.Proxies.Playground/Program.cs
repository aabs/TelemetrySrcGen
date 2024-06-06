// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TelemetrySrcGen.Abstractions.Playground;

var x = new MyIngestionTelemetry(new Microsoft.ApplicationInsights.TelemetryClient());

using (x.RecordIngestionProcessDuration())
{
    Console.WriteLine("Hello World");
}
