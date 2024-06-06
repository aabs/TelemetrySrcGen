using Microsoft.ApplicationInsights;
using System.Diagnostics;
using System.Linq;

namespace TelemetrySrcGen.Abstractions;

public class StopwatchDisposable : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly TelemetryClient tc;
    private readonly string metricName;
    private readonly IDictionary<string, string> properties;

    public StopwatchDisposable(TelemetryClient tc, string metricName, IDictionary<string, string> properties = null)
    {
        if (string.IsNullOrWhiteSpace(metricName))
        {
            throw new ArgumentException($"'{nameof(metricName)}' cannot be null or whitespace.", nameof(metricName));
        }

        this.tc = tc ?? throw new ArgumentNullException(nameof(tc));
        this.metricName = metricName;
        this.properties = properties;
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        tc.TrackMetric(metricName, _stopwatch.ElapsedMilliseconds, properties);
    }
}