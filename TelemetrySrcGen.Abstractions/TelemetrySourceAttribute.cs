#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace TelemetrySrcGen;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TelemetrySourceAttribute : Attribute
{
}
