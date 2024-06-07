#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace TelemetrySrcGen;

public enum MetricKind
{
    Counter,
    Gauge,
    Duration,
    Operation,
    Event,
}