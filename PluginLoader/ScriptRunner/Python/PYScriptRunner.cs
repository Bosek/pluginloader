using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Linq;

namespace PluginLoader
{
    class PYScriptRunner : ScriptRunner
    {
        private static ScriptEngine engine = null;

        private ScriptScope scope = null;

        public override void CreateScope(ScriptGlobal scriptGlobal)
        {
            scope = ((ScriptEngine)getEngine()).Runtime.CreateScope();
            scope.SetVariable("Global", scriptGlobal);
        }

        public override void ExecuteScript(string fileName)
        {
            checkScope();

            var errorListener = new PYErrorListener();
            var compiled = ((ScriptEngine)getEngine()).CreateScriptSourceFromFile(fileName).Compile(errorListener);

            if (errorListener.Errors.Any())
            {
                Errors = errorListener.Errors.ToArray();
                throw new InvalidOperationException();
            }
            else if (errorListener.Warnings.Any())
                Warnings = errorListener.Warnings.ToArray();

            compiled.Execute(scope);
        }

        protected override object getEngine()
        {
            if (engine == null)
                engine = Python.CreateEngine();
            return engine;
        }
        private void checkScope()
        {
            if (scope == null)
                throw new NullReferenceException("Scope is not set.");
        }
    }
}
