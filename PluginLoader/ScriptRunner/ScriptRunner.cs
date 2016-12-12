namespace PluginLoader
{
    abstract class ScriptRunner
    {
        public string[] Errors { get; protected set; } = new string[0];
        public string[] Warnings { get; protected set; } = new string[0];
        public abstract void CreateScope(ScriptGlobal scriptGlobals);
        public abstract void ExecuteScript(string fileName);
        protected abstract object getEngine();
    }
}
