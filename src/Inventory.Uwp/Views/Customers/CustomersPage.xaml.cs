﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.ViewModels.Customers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views.Customers
{
    public sealed partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            ViewModel = Ioc.Default.GetService<CustomersViewModel>();
            InitializeComponent();
        }

        public CustomersViewModel ViewModel { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as CustomerListArgs);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }



        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void OnDoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }



        private  void OpenInNewView(object sender, RoutedEventArgs e)
        {
            var args = ViewModel.CustomerList.CreateArgs();
            //args.IsMainView = false;
            //await windowService.OpenInNewWindow<CustomersViewModel>(args);
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

    }
}
