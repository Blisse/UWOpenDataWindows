using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UWOpenDataLib.JsonModels.Common;
using UWOpenDataLib.JsonModels.Events;
using UWOpenDataLib.JsonModels.FoodServices;
using UWOpenDataLib.Services;

using DataKey = System.String;
using TimeStamp = System.DateTime;

namespace UWOpenDataWindowsBase.Utilities
{
    public class DataManager
    {
        #region Constructors

        public static DataManager Instance { get; set; }

        private readonly IStorageManager _storageManager;
        private readonly Dictionary<DataKey, TimeStamp> _dataKeyToTimeStampDictionary = new Dictionary<string, DateTime>();

        public DataManager(IStorageManager storageManager)
        {
            _storageManager = storageManager;
        }

        #endregion

        #region Public Methods

        public async Task<Response<T>> GetData<T>(String storageKey, Func<CancellationToken, Task<Response<T>>> downloadFunc, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfStorageManagerNotInstantiated();

            Debug.WriteLine("StorageKey: " + storageKey);
            Debug.WriteLine(ShouldDownloadData(storageKey) ? "Downloading new data" : "Getting cached data");

            if (ShouldDownloadData(storageKey))
            {
                var data = await downloadFunc(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (!data.HasError())
                {
                    AddOrUpdateDataTimeStamp(storageKey);
                    _storageManager.AddOrUpdateValue(storageKey, data);
                }
                else
                {
                    Debug.WriteLine("An error has occurred with the downloaded data");
                    RemoveDataTimeStamp(storageKey);
                    _storageManager.DeleteKey(storageKey);
                    return null;
                }
            }

            return _storageManager.GetValueOrDefault<Response<T>>(storageKey, null);
        }

        public Boolean ShouldDownloadData(DataKey key)
        {
            // no existing data
            if (!_dataKeyToTimeStampDictionary.ContainsKey(key))
            {
                return true;
            }
            else
            {
                // data has expired 
                if (DateTime.UtcNow >= _dataKeyToTimeStampDictionary[key])
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void ThrowIfStorageManagerNotInstantiated()
        {
            if (_storageManager == null)
            {
                throw new Exception("Class DataManager has not been instantiated.");
            }
        }

        private void RemoveDataTimeStamp(DataKey key)
        {
            if (_dataKeyToTimeStampDictionary.ContainsKey(key))
            {
                _dataKeyToTimeStampDictionary.Remove(key);
            }
        }

        private void AddOrUpdateDataTimeStamp(DataKey key)
        {
            if (_dataKeyToTimeStampDictionary.ContainsKey(key))
            {
                _dataKeyToTimeStampDictionary[key] = DateTime.UtcNow.AddMinutes(Constants.DataCacheTimeInMinutes);
            }
            else
            {
                _dataKeyToTimeStampDictionary.Add(key,DateTime.UtcNow.AddMinutes(Constants.DataCacheTimeInMinutes));
            }
        }

        #endregion
    }
}
