using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using UWOpenDataLib.Services;
using UWOpenDataWindowsBase.Utilities;

namespace UWOpenDataWindowsBase.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        private static readonly UWOpenDataApiManager UwOpenDataApiManager = new UWOpenDataApiManager();

        protected UWOpenDataApiManager UwOpenDataApi
        {
            get { return UwOpenDataApiManager; }
        }


        /// <summary>
        /// The <see cref="IsDataLoading" /> property's name.
        /// </summary>
        public const string IsDataLoadingPropertyName = "IsDataLoading";

        private bool _isDataLoading = false;

        /// <summary>
        /// Sets and gets the IsDataLoading property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsDataLoading
        {
            get
            {
                return _isDataLoading;
            }

            set
            {
                if (_isDataLoading == value)
                {
                    return;
                }

                RaisePropertyChanging(IsDataLoadingPropertyName);
                _isDataLoading = value;
                RaisePropertyChanged(IsDataLoadingPropertyName);
            }
        }


        public virtual Task GetData(CancellationToken cancellationToken)
        {
            return Task.Delay(0, cancellationToken);
        }

        protected async Task<T> GetData<T>(String dataStoryKey, Func<CancellationToken, Task<Response<T>>> downloadFunc, CancellationToken cancellationToken)
        {
            if (!DataManager.Instance.ShouldDownloadData(dataStoryKey))
            {
                return default(T);
            }

            var response = await DataManager.Instance.GetData(dataStoryKey, downloadFunc, cancellationToken);

            if (response != null && !response.HasError())
            {
                return response.Data;
            }

            return default(T);
        }

    }
}
