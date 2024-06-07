#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace TelemetrySrcGen;

[System.AttributeUsage(AttributeTargets.Field|AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class MeasurementAttribute : Attribute
{
    private readonly MetricKind metricKind;

    public MeasurementAttribute(MetricKind positionalString)
    {
        this.metricKind = positionalString;
    }

    public MetricKind MetricKind
    {
        get { return metricKind; }
    }
}