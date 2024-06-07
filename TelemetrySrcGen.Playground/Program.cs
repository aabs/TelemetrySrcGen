using Microsoft.ApplicationInsights.Extensibility;
using TelemetrySrcGen.Playground;

var configuration = new TelemetryConfiguration
{
    ConnectionString = $"InstrumentationKey={Guid.NewGuid()};IngestionEndpoint=https://westeurope-2.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/",
    TelemetryInitializers = { new OperationCorrelationTelemetryInitializer() }
};

var x = new MyIngestionTelemetry(new Microsoft.ApplicationInsights.TelemetryClient(configuration));

//using (x.RecordIngestionProcessDuration())
//{
//    Console.WriteLine("Hello World");
//}
