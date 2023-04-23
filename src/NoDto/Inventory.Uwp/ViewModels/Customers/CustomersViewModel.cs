﻿#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.Orders;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Customers
{
    public class CustomersViewModel : ViewModelBase
    {
        private readonly ILogger<CustomersViewModel> _logger;

        public CustomersViewModel(ILogger<CustomersViewModel> logger,
                                  CustomerListViewModel customerListViewModel,
                                  CustomerDetailsViewModel customerDetailsViewModel,
                                  OrderListViewModel orderListViewModel)
            : base()
        {
            _logger = logger;
            CustomerList = customerListViewModel;
            CustomerDetails = customerDetailsViewModel;
            CustomerOrders = orderListViewModel;
        }

        public CustomerListViewModel CustomerList
        {
            get; set;
        }
        public CustomerDetailsViewModel CustomerDetails
        {
            get; set;
        }
        public OrderListViewModel CustomerOrders
        {
            get; set;
        }

        public async Task LoadAsync(CustomerListArgs args)
        {
            await CustomerList.LoadAsync(args);
            //IsMainView = args.IsMainView;
            //OnPropertyChanged(nameof(IsMainView));
        }

        public void Unload()
        {
            CustomerDetails.CancelEdit();
            CustomerList.Unload();
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<CustomerListViewModel>(this, OnMessage);
            Messenger.Register<CustomerChangedMessage>(this, OnMessage);

            CustomerList.Subscribe();
            CustomerDetails.Subscribe();
            CustomerOrders.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.Unregister<CustomerChangedMessage>(this);
            CustomerList.Unsubscribe();
            CustomerDetails.Unsubscribe();
            CustomerOrders.Unsubscribe();
        }

        private async void OnMessage(object recipient, CustomerChangedMessage message)
        {
            if (message.Value == "ItemSelected")
            {
                if (message.Id != 0)
                {
                    //TODO: rendere il metodo OnItemSelected cancellabile
                    await OnItemSelected();
                }
            }
        }

        //private async void OnMessage(CustomerListViewModel viewModel, string message, object args)
        //{
        //    if (viewModel == CustomerList && message == "ItemSelected")
        //    {
        //        await ContextService.RunAsync(() =>
        //        {
        //            await OnItemSelected();
        //        });
        //    }
        //}

        // TODO: modificare metodo in (long selectedCustomerId)
        private async Task OnItemSelected()
        {
            if (CustomerDetails.IsEditMode)
            {
                StatusReady();
                CustomerDetails.CancelEdit();
            }
            CustomerOrders.IsMultipleSelection = false;
            var selected = CustomerList.SelectedItem;
            if (!CustomerList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected.Id);
                    await PopulateOrders(selected.Id);
                }
            }
            //CustomerDetails.Item = selected;
        }

        private async Task PopulateDetails(long selectedCustomerId)
        {
            try
            {
                await CustomerDetails.LoadAsync(new CustomerDetailsArgs { CustomerID = selectedCustomerId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load Details");
            }
        }

        private async Task PopulateOrders(long selectedCustomerId)
        {
            try
            {
                await CustomerOrders.LoadAsync(new OrderListArgs { CustomerID = selectedCustomerId }, silent: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load Orders");
            }
        }
    }
}