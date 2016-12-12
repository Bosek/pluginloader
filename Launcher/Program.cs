using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

namespace Launcher
{
    class Program
    {
        public static Log Log { get; private set; }
        private static void setupLog()
        {
            Log.OnInfo += (string text) => Console.Write(text);
            Log.OnError += (string text) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(text);
                Console.ResetColor();
            };
            Log.OnSuccess += (string text) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(text);
                Console.ResetColor();
            };
        }

        private static bool checkFiles(string path)
        {
            var notFound = false;
            PatchRunner.RequiredFiles
                .Where(file => !File.Exists(Path.Combine(path, file))).ToList()
                .ForEach(file =>
                {
                    Log.ErrorLine($"File {Path.Combine(path, file)} could not be found.");
                    notFound = true;
                });
            return !notFound;
        }

        static void Main(string[] args)
        {
            Log = new Log();
            setupLog();

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!checkFiles(currentDirectory))
                return;

            var patcherDomain = AppDomain.CreateDomain("PL PatchRunner");
            var patchRunner = (PatchRunner)patcherDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().GetName().Name, $"{nameof(Launcher)}.{nameof(PatchRunner)}",
                true, BindingFlags.Default, null,
                new object[] { currentDirectory, Log },
                null, new object[0]);

            if (patchRunner.Patch())
            {
                var irAssemblyData = patchRunner.GetOutputStream().ToArray();
                AppDomain.Unload(patcherDomain);

                var irAssembly = Assembly.LoadFile(Path.Combine(currentDirectory, "patchedIR.exe"));

                var startupArgs = args.Length > 0 ? args : new string[] { "-mainmenu" };
                irAssembly.EntryPoint.Invoke(null, new object[] { startupArgs });
            }
            else
                Log.ErrorLine("Something went wrong while patching. Please, report any issues to developer.");
        }
    }
}
