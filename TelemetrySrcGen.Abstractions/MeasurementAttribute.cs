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