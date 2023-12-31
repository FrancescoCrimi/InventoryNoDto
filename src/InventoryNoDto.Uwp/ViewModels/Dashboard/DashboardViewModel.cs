﻿// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Inventory.Application;
using Inventory.Domain.CustomerAggregate;
using Inventory.Domain.OrderAggregate;
using Inventory.Domain.ProductAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Customers;
using Inventory.Uwp.ViewModels.Orders;
using Inventory.Uwp.ViewModels.Products;
using Inventory.Uwp.Views.Customers;
using Inventory.Uwp.Views.Orders;
using Inventory.Uwp.Views.Products;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.ViewModels.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly DashboardService _dashboardService;
        private readonly INavigationService _navigationService;
        private AsyncRelayCommand _loadedCommand;
        private RelayCommand _unLoadedCommand;
        private RelayCommand<ItemClickEventArgs> _itemClickCommand;
        private IList<Customer> _customers;
        private IList<Product> _products;
        private IList<Order> _orders;

        public DashboardViewModel(ILogger<DashboardViewModel> logger,
                                  DashboardService dashboardService,
                                  INavigationService navigationService
            )
            : base()
        {
            _logger = logger;
            _dashboardService = dashboardService;
            _navigationService = navigationService;
        }

        public ICommand LoadedCommand => _loadedCommand
            ?? (_loadedCommand = new AsyncRelayCommand(LoadAsync));

        public ICommand UnLoadedCommand => _unLoadedCommand
            ?? (_unLoadedCommand = new RelayCommand(Unload));

        public ICommand ItemClickCommand => _itemClickCommand
            ?? (_itemClickCommand = new RelayCommand<ItemClickEventArgs>(ItemClick));

        public IList<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public IList<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public IList<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        private async Task LoadAsync()
        {
            StartStatusMessage("Loading dashboard...");
            await LoadCustomersAsync();
            await LoadOrdersAsync();
            await LoadProductsAsync();
            EndStatusMessage("Dashboard loaded");
        }

        private void Unload()
        {
            Customers = null;
            Products = null;
            Orders = null;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var request = new DataRequest<Customer>
                {
                    OrderByDesc = r => r.CreatedOn
                };
                Customers = await _dashboardService.GetCustomersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadCustomers, ex, "Load Customers");
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                var request = new DataRequest<Order>
                {
                    OrderByDesc = r => r.OrderDate
                };
                Orders = await _dashboardService.GetOrdersAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadOrders, ex, "Load Orders");
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var request = new DataRequest<Product>
                {
                    OrderByDesc = r => r.CreatedOn
                };
                Products = await _dashboardService.GetProductsAsync(0, 5, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadProducts, ex, "Load Products");
            }
        }

        private void ItemClick(ItemClickEventArgs e)
        {
            if (e.ClickedItem is Control control)
            {
                switch (control.Tag as string)
                {
                    case "Customers":
                        _navigationService.Navigate(typeof(CustomersView),new CustomerListArgs { OrderByDesc = r => r.CreatedOn });
                        break;
                    case "Orders":
                        _navigationService.Navigate(typeof(OrdersView), new OrderListArgs { OrderByDesc = r => r.OrderDate });
                        break;
                    case "Products":
                        _navigationService.Navigate(typeof(ProductsView), new ProductListArgs { OrderByDesc = r => r.ListPrice });
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
