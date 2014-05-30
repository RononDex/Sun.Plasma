﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Sun.Core.Wpf
{
    /// <summary>
    /// This class makes it possible to cache wpf controls.
    /// </summary>
    public class WpfControlCache
    {
        public WpfControlCache()
        {
            this.Cache = new Dictionary<string, UserControl>();
        }

        /// <summary>
        /// Interal storeage of wpf controls cache
        /// </summary>
        private Dictionary<string, UserControl> Cache { get; set; }

        /// <summary>
        /// Accesser for the cached controls
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public UserControl this[string controlName]
        {
            get 
            { 
                if (string.IsNullOrEmpty(controlName)) return null;
                else if (Cache.ContainsKey(controlName)) return Cache[controlName];
                else return null;
            }
            set
            {
                if (Cache.ContainsKey(controlName))
                    Cache[controlName] = value;
                else
                    Cache.Add(controlName, value);
            }
        }

        /// <summary>
        /// Removes the given contorl from the internal cache
        /// </summary>
        /// <param name="controlName"></param>
        public void RemoveUserControlFromCache(string controlName)
        {
            if (Cache.ContainsKey(controlName))            
                Cache.Remove(controlName);            
        }
    }
}
