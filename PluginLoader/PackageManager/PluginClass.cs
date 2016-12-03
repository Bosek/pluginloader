using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLoader
{
    public abstract class PluginClass
    {
        public readonly PluginGlobal Global;
        
        /// <summary>
        /// The path to the plugin directory
        /// </summary>
        public String PluginDir { get; internal set; }

        public PluginClass(ref PluginGlobal Global)
        {
            this.Global = Global;
        }

        /// <summary>
        /// The main function of the plugin.
        /// This is called when the plugin is loaded by the PluginManager
        /// </summary>
        public abstract void Init();
    }
}
