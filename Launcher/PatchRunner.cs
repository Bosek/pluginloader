using Launcher.Patches;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Launcher
{
    static class Helpers
    {
        public static byte[] ToBytes(this Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            return bytes;
        }
    }
    class PatchRunner : MarshalByRefObject
    {
        private PatcherData patcherData;

        public PatchRunner(string path, Log log)
        {
            Log = log;
            WorkingPath = path;
        }

        public static string[] RequiredFiles { get; } = new string[]
                        {
            "IR.exe"
        };
        public Log Log { get; private set; }
        public string WorkingPath { get; private set; }
        public string GetIRVersion()
        {
            var irGlobalsType = patcherData.IRData.Assembly.GetType("Game.Configuration.Globals");
            return (string)irGlobalsType.GetField("Version").GetValue(null);
        }

        public MemoryStream GetOutputStream()
        {
            var assemblyStream = new MemoryStream();
            patcherData.IRData.AssemblyDef.Write(assemblyStream);
            return assemblyStream;
        }

        public void LoadData()
        {
            patcherData = new PatcherData();
            Directory.SetCurrentDirectory(WorkingPath);

            patcherData.Load(loadData("IR.exe"), loadData("PluginLoader.dll"));
        }

        public bool Patch()
        {
            if (patcherData == null)
                LoadData();

            try
            {
                if (!callPatches(patcherData))
                    return false;
            }
            catch (Exception exception)
            {
                Log.ErrorLine("An error occurred during patching");
                Log.ErrorLine(exception.ToString());
                return false;
            }
            return true;
        }

        private bool callPatches(PatcherData patcherData)
        {
            checkIfAlreadyPatched();

            logIRVersion();

            var patches = new List<Type>();
            patches.Add(typeof(AccessModifierPatch));
            patches.Add(typeof(ProgramPatch));

            //patches.Add(typeof(AgosGuiPatch));
            patches.Add(typeof(ChatControllerPatch));
            //patches.Add(typeof(ClientDeviceFactoryPatch));
            //patches.Add(typeof(DeviceFactoryPatch));
            //patches.Add(typeof(GameClientPatch));
            //patches.Add(typeof(GameEditorPatch));
            //patches.Add(typeof(GameMenuPatch));
            patches.Add(typeof(GameServerPatch));

            try
            {
                var irData = patcherData.IRData;
                var plData = patcherData.PLData;

                irData.AssemblyDef.Name.Name = "patchedIR";

                patches.ForEach((patch) => executePatch(patch, patcherData));

                saveAssembly(irData.AssemblyDef, "patchedIR.exe");
            }
            catch (Exception exception)
            {
                Log.ErrorLine("An error occurred during launching");
                Log.ErrorLine(exception.ToString());
                return false;
            }
            Log.InfoLine("Patching done");
            return true;
        }

        private void executePatch(Type patch, PatcherData patcherData)
        {
            Log.Info($"Applying {patch.Name} ");
            Activator.CreateInstance(patch, patcherData);
            Log.SuccessLine("done");
        }

        private void checkIfAlreadyPatched()
        {
            var irAssemblyReferences = patcherData.IRData.Module.AssemblyReferences;

            if (irAssemblyReferences.Count(assembly =>
                assembly.FullName.StartsWith(patcherData.PLData.AssemblyDef.Name.Name)) > 0)
            {
                Log.ErrorLine("IR.exe seem to be already patched");
                throw new Exception("IR.exe seem to be already patched");
            }
        }

        private PatcherTargetData loadData(string assemblyFileName)
        {
            var data = new PatcherTargetData
            {
                Path = Path.Combine(WorkingPath, assemblyFileName)
            };
            data.Assembly = Assembly.Load(File.OpenRead(data.Path).ToBytes());
            using (var assemblyStream = File.OpenRead(data.Path))
                data.AssemblyDef = AssemblyDefinition.ReadAssembly(assemblyStream);
            data.Module = data.AssemblyDef.MainModule;

            return data;
        }
        private void logIRVersion()
        {
            try
            {
                Log.Info("Interstellar Rift version ");
                Log.SuccessLine(GetIRVersion());
            }
            catch (Exception)
            {
                Log.ErrorLine("could not be detected");
            }
        }

        private void saveAssembly(AssemblyDefinition assemblyDef, string path)
        {
            Log.Info($"Saving {Path.GetFileName(path)} ");
            assemblyDef.Write(File.OpenWrite(path));
            Log.SuccessLine("done");
        }
    }
}
