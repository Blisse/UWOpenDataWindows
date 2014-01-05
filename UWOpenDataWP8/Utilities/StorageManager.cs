using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UWOpenDataWindowsBase.Utilities;

namespace UWOpenDataWP8.Utilities
{
    public class StorageManager : IStorageManager
    {
        private readonly SemaphoreSlim _saveSem = new SemaphoreSlim(1);
        private readonly IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;

        public bool DeleteKey(string key)
        {
            _saveSem.Wait();
            try
            {
                return _settings.Remove(key);
            }
            finally
            {
                _saveSem.Release();
            }
        }

        public bool AddOrUpdateValue(string key, Object value)
        {
            _saveSem.Wait();
            try
            {
                if (_settings.Contains(key))
                {
                    _settings[key] = value;

                }
                else
                {
                    _settings.Add(key, value);
                }
                return true;
            }
            finally
            {
                _saveSem.Release();
            }
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            if (_settings.Contains(key))
            {
                value = (T)_settings[key];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public void Save()
        {
            _saveSem.Wait();
            try
            {
                _settings.Save();
            }
            finally
            {
                _saveSem.Release();
            }
        }


        public bool ContainsKey(string key)
        {
            return _settings.Contains(key);
        }

        public void DeleteAllKeys()
        {
            _saveSem.Wait();
            try
            {
                _settings.Clear();
            }
            finally
            {
                _saveSem.Release();
            }
        }
    }
}
