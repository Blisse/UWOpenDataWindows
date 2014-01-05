using System;
using System.Collections.Generic;
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
            //cancellationToken.ThrowIfCancellationRequested();
            //ThrowIfStorageManagerNotInstantiated();

            //if (ShouldDownloadData(storageKey))
            //{
            //    var data = await downloadFunc(cancellationToken);
            //    cancellationToken.ThrowIfCancellationRequested();

            //    if (!data.HasError())
            //    {
            //        AddOrUpdateDataTimeStamp(storageKey);
            //        _storageManager.AddOrUpdateValue(storageKey, data);
            //    }
            //    else
            //    {
            //        RemoveDataTimeStamp(storageKey);
            //        _storageManager.DeleteKey(storageKey);
            //    }
            //}

            //return _storageManager.GetValueOrDefault<Response<T>>(storageKey, null);
        }

        public Boolean ShouldDownloadData(DataKey key)
        {
            // no existing data
            if (!_dataKeyToTimeStampDictionary.ContainsKey(key))
            {
                return true;
            }

            // data has expired 
            if (DateTime.UtcNow >= _dataKeyToTimeStampDictionary[key])
            {
                return true;
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
                _dataKeyToTimeStampDictionary[Constants.EventsHolidaysDataKey] =
                    DateTime.UtcNow.AddMinutes(Constants.DataCacheTimeInMinutes);
            }
            else
            {
                _dataKeyToTimeStampDictionary.Add(Constants.EventsHolidaysDataKey,
                    DateTime.UtcNow.AddMinutes(Constants.DataCacheTimeInMinutes));
            }
        }

        #endregion
    }
}
