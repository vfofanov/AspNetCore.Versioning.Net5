using Microsoft.CodeAnalysis;

namespace AspNetCore.Versioning.ApiVersionsGenerator
{
    public static class SourceGeneratorExtensions
    {
        public static bool TryReadGlobalBuildProperty(this GeneratorExecutionContext context, string property, out string value)
        {
            return context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{property}", out value);
        }

        public static bool TryReadAdditionalFilesAttribute(this GeneratorExecutionContext context, AdditionalText additionalText, string property, out string value)
        {
            return context.AnalyzerConfigOptions.GetOptions(additionalText).TryGetValue($"build_metadata.AdditionalFiles.{property}", out value);
        }
    }
}