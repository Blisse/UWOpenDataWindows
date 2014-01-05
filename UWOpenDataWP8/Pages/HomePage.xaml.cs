using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UWOpenDataWindowsBase.ViewModels;

namespace UWOpenDataWP8.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        private CancellationTokenSource CancellationTokenSource { get; set; }

        public HomePage()
        {
            InitializeComponent();
            CancellationTokenSource = new CancellationTokenSource();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                await App.ViewModelLocator.HomePage.GetHomePageData(CancellationTokenSource.Token);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                Debug.WriteLine("Data download cancelled: " + operationCanceledException);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            CancellationTokenSource.Cancel();
        }
    }
}