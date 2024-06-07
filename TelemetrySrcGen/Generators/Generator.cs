#pragma warning disable HAA0301 // Closure Allocation Source
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TelemetrySrcGen.Helpers;

namespace TelemetrySrcGen;

[Generator]
public partial class Generator : IIncrementalGenerator
{
    protected const string MethodFieldAttribute = "MeasurementAttribute";
    protected const string TargetAttribute = "TelemetrySourceAttribute";
    /// <summary>
    /// Called to initialize the generator and register generation steps via callbacks
    /// on the <paramref name="context" />
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.CodeAnalysis.IncrementalGeneratorInitializationContext" /> to register callbacks on</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<SyntaxAndSymbol> classDeclarations =
                context.SyntaxProvider
                    .CreateSyntaxProvider(
                        predicate: AttributePredicate,
                        transform: static (ctx, _) => ToGenerationInput(ctx))
                    .Where(static m => m is not null);

        IncrementalValueProvider<(Compilation, ImmutableArray<SyntaxAndSymbol>)> compilationAndClasses
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        // register a code generator for the triggers
        context.RegisterSourceOutput(compilationAndClasses, Generate);

        static SyntaxAndSymbol ToGenerationInput(GeneratorSyntaxContext context)
        {
            var declarationSyntax = (TypeDeclarationSyntax)context.Node;

            var symbol = context.SemanticModel.GetDeclaredSymbol(declarationSyntax);
            if (symbol is not INamedTypeSymbol namedSymbol) throw new NullReferenceException($"Code generated symbol of {nameof(declarationSyntax)} is missing");
            return new SyntaxAndSymbol(declarationSyntax, namedSymbol);
        }

        void Generate(
                       SourceProductionContext spc,
                       (Compilation compilation,
                       ImmutableArray<SyntaxAndSymbol> items) source)
        {
            var (compilation, items) = source;
            foreach (SyntaxAndSymbol item in items)
            {
                OnGenerate(spc, compilation, item);
            }
        }
    }

    private static bool AttributePredicate(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        return syntaxNode.MatchAttribute(TargetAttribute, cancellationToken);
    }
    private void GenerateCounters(SourceProductionContext context, INamedTypeSymbol typeSymbol, TypeDeclarationSyntax syntax, StringBuilder builder)
    {
        foreach (var item in typeSymbol.GetMembers().Where(m => m.HasMeasurementAttribute(MetricKind.Counter)))
        {
            try
            {
                if (item is not IFieldSymbol fieldSymbol)
                    continue;

                string fieldName = fieldSymbol.Name;
                string fieldType = fieldSymbol.Type.Name;
                builder.AppendLine($$"""
                    public void Add{{fieldName}}({{fieldType}} value) => telemetryClient.TrackMetric("{{fieldName}}.Counter", value);
                """);
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor("TSG004", "Error while generating Counter.",
                    $"Unknown error during generation: {e.Message}", "CustomErrorCategory",
                    DiagnosticSeverity.Error, isEnabledByDefault: true),
                    Location.None);
                context.ReportDiagnostic(diagnostic);
            }


        }
    }

    private void GenerateEvents(SourceProductionContext context, INamedTypeSymbol typeSymbol, TypeDeclarationSyntax syntax, StringBuilder builder)
    {
        // events make use of partial methods to define what invoking the event would look like. It must have a null result type
        foreach (var item in typeSymbol.GetMembers().Where(m => m.HasMeasurementAttribute(MetricKind.Event)))
        {
            try
            {
                if (item is not IMethodSymbol methodSymbol)
                    continue;
                if (!methodSymbol.IsPartialDefinition)
                {
                    continue;
                }

                string methodName = methodSymbol.Name;
                builder.AppendLine($$"""
                        public partial void {{methodName}}(IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null) 
                          => telemetryClient.TrackEvent({{methodName}}, properties, metrics);
                    """);
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor("TSG006", "Error while generating Event.",
                    $"Unknown error during generation: {e.Message}", "CustomErrorCategory",
                    DiagnosticSeverity.Error, isEnabledByDefault: true),
                    Location.None);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void GenerateGauges(SourceProductionContext context, INamedTypeSymbol typeSymbol, TypeDeclarationSyntax syntax, StringBuilder builder)
    {
        foreach (var item in typeSymbol.GetMembers().Where(m => m.HasMeasurementAttribute(MetricKind.Gauge)))
        {
            try
            {
                if (item is not IFieldSymbol fieldSymbol)
                    continue;

                string fieldName = fieldSymbol.Name;
                string fieldType = fieldSymbol.Type.Name;
                builder.AppendLine($$"""
                        public void Set{{fieldName}}({{fieldType}} value) => telemetryClient.TrackMetric("{{fieldName}}.Counter", value);
                    """);
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor("TSG002", "Error while generating Gauge.",
                    $"Unknown error during generation: {e.Message}", "CustomErrorCategory",
                    DiagnosticSeverity.Error, isEnabledByDefault: true),
                    Location.None);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void GenerateMetricClass(SourceProductionContext context, INamedTypeSymbol typeSymbol, TypeDeclarationSyntax syntax)
    {
        StringBuilder builder = new StringBuilder();
        var name = typeSymbol.Name;
        var asm = GetType().Assembly.GetName();
        builder.AppendHeader(syntax, typeSymbol);
        builder.AppendLine();
        builder.AppendLine("using Microsoft.ApplicationInsights.DataContracts;");

        builder.AppendLine($"[System.CodeDom.Compiler.GeneratedCode(\"{asm.Name}\",\"{asm.Version}\")]");
        builder.AppendLine($"public partial class {name} //blah");
        builder.AppendLine("{");
        GenerateCounters(context, typeSymbol, syntax, builder);
        GenerateGauges(context, typeSymbol, syntax, builder);
        GenerateOperations(context, typeSymbol, syntax, builder);
        GenerateTimers(context, typeSymbol, syntax, builder);
        GenerateEvents(context, typeSymbol, syntax, builder);
        builder.AppendLine("}");
        context.AddSource($"{name}.g.cs", builder.ToString());
#if DEBUG
        Debug.WriteLine(builder.ToString());
#endif
    }
    private void GenerateOperations(SourceProductionContext context, INamedTypeSymbol typeSymbol, TypeDeclarationSyntax syntax, StringBuilder builder)
    {
        foreach (var item in typeSymbol.GetMembers().Where(m => m.HasMeasurementAttribute(MetricKind.Operation)))
        {
            try
            {
                if (item is not IFieldSymbol fieldSymbol)
                    continue;

                string fieldName = fieldSymbol.Name;
                string fieldType = fieldSymbol.Type.Name;
                builder.AppendLine($$"""
                        public void Start{{fieldName}}() 
                          => telemetryClient.StartOperation<RequestTelemetry>("{{fieldName}}");
                    """);
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor("TSG005", "Error while generating Operation.",
                    $"Unknown error during generation: {e.Message}", "CustomErrorCategory",
                    DiagnosticSeverity.Error, isEnabledByDefault: true),
                    Location.None);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void GenerateTimers(SourceProductionContext context, INamedTypeSymbol typeSymbol, TypeDeclarationSyntax syntax, StringBuilder builder)
    {
        foreach (var item in typeSymbol.GetMembers().Where(m => m.HasMeasurementAttribute(MetricKind.Duration)))
        {
            try
            {
                if (item is not IFieldSymbol fieldSymbol)
                    continue;

                string fieldName = fieldSymbol.Name;
                string fieldType = fieldSymbol.Type.Name;
                builder.AppendLine($$"""
                        public IDisposable Record{{fieldName}}Duration(IDictionary<string, string> properties = null)
                        {
                            return new TelemetrySrcGen.Abstractions.StopwatchDisposable(this.telemetryClient, "{{fieldName}}.Duration", properties);
                        }
                    """);
            }
            catch (Exception e)
            {
                var diagnostic = Diagnostic.Create(
                    new DiagnosticDescriptor("TSG003", "Error while generating Timer.",
                    $"Unknown error during generation: {e.Message}", "CustomErrorCategory",
                    DiagnosticSeverity.Error, isEnabledByDefault: true),
                    Location.None);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
    private void OnGenerate(
                    SourceProductionContext context,
            Compilation compilation,
            SyntaxAndSymbol input)
    {
        INamedTypeSymbol typeSymbol = input.Symbol;
        TypeDeclarationSyntax syntax = input.Syntax;
        var cancellationToken = context.CancellationToken;
        if (cancellationToken.IsCancellationRequested)
            return;

        try
        {
            GenerateMetricClass(context, typeSymbol, syntax);
        }
        catch (Exception e)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor("TSG001", "Unknown error during generation",
                $"Unknown error during generation: {e.Message}", "CustomErrorCategory",
                DiagnosticSeverity.Warning, isEnabledByDefault: true),
                Location.None);
            context.ReportDiagnostic(diagnostic);
        }
    }
}

#pragma warning restore HAA0401 // Possible allocation of reference type enumerator