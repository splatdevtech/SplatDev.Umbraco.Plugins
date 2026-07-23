namespace SplatDev.Umbraco.Plugins.CodeFirst.Models
{
    using Microsoft.Win32.SafeHandles;

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    /// <summary>
    /// Model helper to share objects into Partial Views
    /// </summary>
    public class ViewModelObject : IDisposable
    {
        bool disposed = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public ViewModelObject(string key, object value)
        {
            values.Add(key, value);
        }

        public ViewModelObject Add(string key, object value)
        {
            values.Add(key, value);
            return this;
        }

        public object this[string key]
        {
            get
            {
                if (values.ContainsKey(key))
                    return values[key];
                else return null;
            }
            set { values[key] = value; }
        }

        #region Dispose
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                values = null;
            }

            disposed = true;
        }
        #endregion

    }
}
