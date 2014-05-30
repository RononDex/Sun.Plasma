using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sun.Core.Wpf
{
    /// <summary>
    /// Interface for wpf controls that are cachable and define a function that gets called when they get loaded & displayed from cache
    /// </summary>
    public interface IWpfCachableControl
    {
        /// <summary>
        /// Function that gets called when the control gets reloaded / displayed from the cache
        /// </summary>
        void ReloadingFromCacheForDisply();
    }
}
