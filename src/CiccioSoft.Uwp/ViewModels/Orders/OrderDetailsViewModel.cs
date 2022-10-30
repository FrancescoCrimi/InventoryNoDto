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

using CiccioSoft.Inventory.Uwp.Models;
using CiccioSoft.Inventory.Uwp.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    #region OrderDetailsArgs
    public class OrderDetailsArgs
    {
        public static OrderDetailsArgs CreateDefault() => new OrderDetailsArgs { CustomerID = 0 };

        public long CustomerID { get; set; }
        public long OrderID { get; set; }

        public bool IsNew => OrderID <= 0;
    }
    #endregion

    public class OrderDetailsViewModel : GenericDetailsViewModel<OrderModel>
    {
        private readonly ILogger<OrderDetailsViewModel> logger;
        private readonly OrderServiceUwp orderService;

        public OrderDetailsViewModel(ILogger<OrderDetailsViewModel> logger,
                                     OrderServiceUwp orderService)
            : base()
        {
            this.logger = logger;
            this.orderService = orderService;
        }

        public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => Item?.Customer == null ? "New Order" : $"New Order, {Item?.Customer?.FullName}";
        public string TitleEdit => Item == null ? "Order" : $"Order #{Item?.OrderID}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditCustomer => Item?.CustomerID <= 0;

        public ICommand CustomerSelectedCommand => new RelayCommand<CustomerModel>(CustomerSelected);
        private void CustomerSelected(CustomerModel customer)
        {
            EditableItem.CustomerID = customer.CustomerID;
            EditableItem.ShipAddress = customer.AddressLine1;
            EditableItem.ShipCity = customer.City;
            EditableItem.ShipRegion = customer.Region;
            EditableItem.ShipCountryCode = customer.CountryCode;
            EditableItem.ShipPostalCode = customer.PostalCode;
            EditableItem.Customer = customer;

            EditableItem.NotifyChanges();
        }

        public OrderDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = await orderService.CreateNewOrderAsync(ViewModelArgs.CustomerID);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await orderService.GetOrderAsync(ViewModelArgs.OrderID);
                    Item = item ?? new OrderModel { OrderID = ViewModelArgs.OrderID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Load");
                }
            }
            OnPropertyChanged(nameof(ItemIsNew));
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.CustomerID ?? 0;
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<OrderDetailsViewModel, OrderModel>(this, OnDetailsMessage);
            Messenger.Register<ItemMessage<OrderModel>>(this, OnOrderMessage);

            //MessageService.Subscribe<OrderListViewModel>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<OrderModel>>>(this, OnOrderListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public OrderDetailsArgs CreateArgs()
        {
            return new OrderDetailsArgs
            {
                CustomerID = Item?.CustomerID ?? 0,
                OrderID = Item?.OrderID ?? 0
            };
        }

        protected async override Task<bool> SaveItemAsync(OrderModel model)
        {
            try
            {
                StartStatusMessage("Saving order...");
                await Task.Delay(100);
                await orderService.UpdateOrderAsync(model);
                EndStatusMessage("Order saved");
                logger.LogInformation($"Order #{model.OrderID} was saved successfully.");
                OnPropertyChanged(nameof(CanEditCustomer));
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order: {ex.Message}");
                logger.LogError(ex, "Save");
                return false;
            }
        }

        protected async override Task<bool> DeleteItemAsync(OrderModel model)
        {
            try
            {
                StartStatusMessage("Deleting order...");
                await Task.Delay(100);
                await orderService.DeleteOrderAsync(model);
                EndStatusMessage("Order deleted");
                logger.LogWarning($"Order #{model.OrderID} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order: {ex.Message}");
                logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected async override Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current order?", "Ok", "Cancel");
        }

        protected override IEnumerable<IValidationConstraint<OrderModel>> GetValidationConstraints(OrderModel model)
        {
            yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Customer", m => m.CustomerID);
            if (model.Status > 0)
            {
                yield return new RequiredConstraint<OrderModel>("Payment Type", m => m.PaymentType);
                yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Payment Type", m => m.PaymentType);
                if (model.Status > 1)
                {
                    yield return new RequiredConstraint<OrderModel>("Shipper", m => m.ShipVia);
                    yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Shipper", m => m.ShipVia);
                }
            }
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnOrderMessage(object recipient, ItemMessage<OrderModel> message)
        {
        //    throw new NotImplementedException();
        //}
        //private async void OnDetailsMessage(OrderDetailsViewModel sender, string message, OrderModel changed)
        //{
            var current = Item;
            if (current != null)
            {
                if (message.Value != null && message.Value.OrderID == current?.OrderID)
                {
                    switch (message.Message)
                    {
                        case "ItemChanged":
                            //await ContextService.RunAsync(async () =>
                            //{
                                try
                                {
                                    var item = await orderService.GetOrderAsync(current.OrderID);
                                    item = item ?? new OrderModel { OrderID = current.OrderID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    OnPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This order has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "Handle Changes");
                                }
                            //});
                            break;
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async void OnIndexRangeListMessage(object recipient, ItemMessage<IList<IndexRange>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await orderService.GetOrderAsync(current.OrderID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Handle Ranges Deleted");
                        }
                        break;
                }
            }
        }
        private async void OnOrderListMessage(object recipient, ItemMessage<IList<OrderModel>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemsDeleted":
                        if (message.Value is IList<OrderModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.OrderID == current.OrderID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                }
            }
        }
        //private async void OnListMessage(OrderListViewModel sender, string message, object args)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        switch (message)
        //        {
        //            case "ItemsDeleted":
        //                if (args is IList<OrderModel> deletedModels)
        //                {
        //                    if (deletedModels.Any(r => r.OrderID == current.OrderID))
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                break;
        //            case "ItemRangesDeleted":
        //                try
        //                {
        //                    var model = await orderService.GetOrderAsync(current.OrderID);
        //                    if (model == null)
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    logger.LogError(ex, "Handle Ranges Deleted");
        //                }
        //                break;
        //        }
        //    }
        //}

        private async Task OnItemDeletedExternally()
        {
            //await ContextService.RunAsync(() =>
            //{
            await Task.Run(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This order has been deleted externally");
            });
            //});
        }
    }
}