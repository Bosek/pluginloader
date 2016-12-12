using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Linq;

namespace PluginLoader
{
    class PYScriptRunner : ScriptRunner<ScriptEngine>
    {
        private static ScriptEngine engine = null;

        private ScriptScope scope = null;

        public override void CreateScope(ScriptGlobal scriptGlobal)
        {
            scope = getEngine().Runtime.CreateScope();
            scope.SetVariable("Global", scriptGlobal);
        }

        public override void ExecuteScript(string fileName)
        {
            checkScope();

            var errorListener = new PYErrorListener();
            var compiled = getEngine().CreateScriptSourceFromFile(fileName).Compile(errorListener);

            if (errorListener.Errors.Any())
            {
                Errors = errorListener.Errors.ToArray();
                throw new InvalidOperationException();
            }
            else if (errorListener.Warnings.Any())
                Warnings = errorListener.Warnings.ToArray();

            compiled.Execute(scope);
        }

        protected override ScriptEngine getEngine()
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
