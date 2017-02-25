using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace hopins
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void backButton()
        {
            var currentView = SystemNavigationManager.GetForCurrentView();

            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += backButton_Tapped;
        }


        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(addProduct), null);
            backButton();
        }

        private void btProduct_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(myProduct), null);
            backButton();
        }

        private void btOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(orderManagement), null);
            backButton();
            
        }

        private void backButton_Tapped(object sender, BackRequestedEventArgs e)
        {

            if (Frame.CanGoBack) Frame.GoBack();

        }

    }
}
