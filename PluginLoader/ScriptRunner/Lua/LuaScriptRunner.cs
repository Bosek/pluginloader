using MoonSharp.Interpreter;
using System;

namespace PluginLoader
{
    class LuaScriptRunner : ScriptRunner
    {
        private static bool setUp = false;
        private bool scopeSet = false;
        private Script script = null;
        public override void CreateScope(ScriptGlobal scriptGlobal)
        {
            if (scopeSet)
                return;
            if (script == null)
                script = (Script)getEngine();

            script.Globals["Global"] = scriptGlobal;
            script.Options.ScriptLoader = new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();
            scopeSet = true;
        }

        public override void ExecuteScript(string fileName)
        {
            checkScope();

            script.DoFile(fileName);
        }

        protected override object getEngine()
        {
            if (!setUp)
            {
                UserData.RegisterType<ScriptGlobal>();
                setUp = true;
            }

            return new Script();
        }
        private void checkScope()
        {
            if (!scopeSet)
                throw new NullReferenceException("Scope is not set.");
        }
    }
}
