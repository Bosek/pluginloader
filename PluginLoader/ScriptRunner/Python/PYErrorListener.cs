using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System.Text;

namespace PluginLoader
{
    class PYErrorListener : ErrorListener
    {
        public List<string> Errors { get; private set; } = new List<string>();
        public List<string> Warnings { get; private set; } = new List<string>();
        public override void ErrorReported(ScriptSource source, string message, SourceSpan span, int errorCode, Severity severity)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{severity.ToString()} {errorCode}: {message} [line:{span.Start.Line} col:{span.Start.Column}]");
            stringBuilder.AppendLine($"{source.GetCodeLine(span.Start.Line)}");

            if (severity == Severity.Error || severity == Severity.FatalError)
                Errors.Add(stringBuilder.ToString());
            else if (severity == Severity.Warning)
                Warnings.Add(stringBuilder.ToString());
        }
    }
}
