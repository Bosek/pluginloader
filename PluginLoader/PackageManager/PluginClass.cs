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
        
        public String PluginDir { get; internal set; }

        public PluginClass(ref PluginGlobal Global)
        {
            this.Global = Global;
        }

        public abstract void Init();
    }
}
