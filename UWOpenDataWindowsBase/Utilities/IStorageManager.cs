using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UWOpenDataWindowsBase.Utilities
{
    public interface IStorageManager
    {
        bool DeleteKey(string key);
        bool AddOrUpdateValue(string key, Object value);
        T GetValueOrDefault<T>(string key, T defaultValue);
        void Save();
        bool ContainsKey(string key);
        void DeleteAllKeys();
    }
}
