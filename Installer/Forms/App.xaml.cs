using System;
using System.Reflection;
using System.Windows;

namespace Installer
{
    partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resource) =>
            {
                if (resource.Name.Contains("Newtonsoft.Json"))
                    return Assembly.Load(Helpers.LoadFileFromResources("Newtonsoft.Json.dll").ToBytes());
                return null;
            };
        }
    }
}
