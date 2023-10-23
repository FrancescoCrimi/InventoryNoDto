// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.CustomerAggregate;
using Inventory.Domain.OrderAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Common;
using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.Orders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.WindowManagement;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderDetailsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly OrderService _orderService;
        private readonly INavigationService _navigationService;
        private readonly IWindowManagerService _windowService;
        private Order _item;
        private bool _isEnabled;
        private bool _isEditMode;
        private RelayCommand _backCommand;
        private RelayCommand _editCommand;
        private AsyncRelayCommand _deleteCommand;
        private AsyncRelayCommand _saveCommand;
        private RelayCommand _cancelCommand;
        private RelayCommand<Customer> _customerSelectedCommand;

        public OrderDetailsViewModel(ILogger<OrderDetailsViewModel> logger,
                                     OrderService orderService,
                                     INavigationService navigationService,
                                     IWindowManagerService windowService)
        {
            _logger = logger;
            _orderService = orderService;
            _navigationService = navigationService;
            _windowService = windowService;
        }

        public event Action<Order> CurrentOrderEvent;

        #region public method

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                //Item = await _orderService.CreateNewOrderAsync(ViewModelArgs.CustomerID);
                if (ViewModelArgs.CustomerId == 0)
                {
                    Item = new Order();
                }
                else
                {
                    var customer = await _orderService.GetCustomerAsync(ViewModelArgs.CustomerId);
                    Item = Order.CreateNewOrder(customer);
                }
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _orderService.GetOrderAsync(ViewModelArgs.OrderId);
                    Item = item ?? new Order { Id = ViewModelArgs.OrderId, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.Load, ex, "Load Order");
                }
            }
            OnPropertyChanged(nameof(ItemIsNew));
        }

        public void Unload()
        {
            ViewModelArgs.CustomerId = Item?.CustomerId ?? 0;
            ViewModelArgs.OrderId = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<Order>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }

        public OrderArgs CreateArgs()
        {
            return new OrderArgs
            {
                CustomerId = Item?.CustomerId ?? 0,
                OrderId = Item?.Id ?? 0
            };
        }

        public virtual void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                //// Create a copy for edit
                //var editableItem = new TModel();
                ////editableItem.Merge(Item);
                //Item = editableItem;
            }
        }

        public virtual void CancelEdit()
        {
            if (ItemIsNew)
            {
                // We were creating a new item: cancel means exit
                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await _windowService.CloseWindowAsync();
                    });
                }
                return;
            }

            // We were editing an existing item: just cancel edition
            if (IsEditMode)
            {
                Item = Item;
            }
            IsEditMode = false;
        }

        #endregion


        #region command

        public ICommand CustomerSelectedCommand => _customerSelectedCommand
            ?? (_customerSelectedCommand = new RelayCommand<Customer>((customer) =>
            {
                //EditableItem.CustomerId = customer.Id;
                //EditableItem.ShipAddress = customer.AddressLine1;
                //EditableItem.ShipCity = customer.City;
                //EditableItem.ShipRegion = customer.Region;
                //EditableItem.ShipCountryId = customer.CountryId;
                //EditableItem.ShipPostalCode = customer.PostalCode;
                //EditableItem.Customer = customer;

                Item = Order.CreateNewOrder(customer);
                //Item.NotifyChanges();
            }));

        public ICommand BackCommand => _backCommand
            ?? (_backCommand = new RelayCommand(() =>
            {
                StatusReady();
                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
            }));

        public ICommand EditCommand => _editCommand
            ?? (_editCommand = new RelayCommand(() =>
            {
                StatusReady();
                BeginEdit();
                //MessageService.Send(this, "BeginEdit", Item);
                Messenger.Send(new ViewModelsMessage<Order>("BeginEdit", Item.Id));
            }));

        public ICommand DeleteCommand => _deleteCommand
            ?? (_deleteCommand = new AsyncRelayCommand(async () =>
            {
                StatusReady();
                if (await ConfirmDeleteAsync())
                {
                    await DeleteAsync();
                }
            }));

        public ICommand SaveCommand => _saveCommand
            ?? (_saveCommand = new AsyncRelayCommand(async () =>
            {
                StatusReady();
                var result = Validate(Item);
                if (result.IsOk)
                {
                    await SaveAsync();
                }
                else
                {
                    await ShowDialogAsync(result.Message, $"{result.Description} Please, correct the error and try again.");
                }
            }));

        public ICommand CancelCommand => _cancelCommand
            ?? (_cancelCommand = new RelayCommand(() =>
            {
                StatusReady();
                CancelEdit();
                Messenger.Send(new ViewModelsMessage<Order>("CancelEdit", Item.Id));
            }));

        #endregion


        #region public property

        public override string Title => Item?.IsNew ?? true ? TitleNew : TitleEdit;

        public string TitleNew => Item?.Customer == null ? "New Order" : $"New Order, {Item?.Customer?.FullName}";

        public string TitleEdit => Item == null ? "Order" : $"Order #{Item?.Id}";

        public bool ItemIsNew
        {
            get
            {
                // TODO: controllare il funzionamento di CancelEdit
                //return Item?.IsNew ?? true;
                return Item?.IsNew ?? false;
            }
        }

        public bool CanEditCustomer => Item?.CustomerId <= 0;

        public OrderDetailsArgs ViewModelArgs { get; private set; }

        public Order Item
        {
            get => _item;
            set
            {
                //if (SetProperty(ref _item, value))
                //{
                OnPropertyChanging(nameof(Item));
                _item = value;
                CurrentOrderEvent?.Invoke(Item);
                IsEnabled = !_item?.IsEmpty ?? false;
                OnPropertyChanged(nameof(Item));
                OnPropertyChanged(nameof(IsDataAvailable));
                OnPropertyChanged(nameof(IsDataUnavailable));
                OnPropertyChanged(nameof(Title));
                //}
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsDataAvailable => _item != null;

        public bool IsDataUnavailable => !IsDataAvailable;

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public bool CanGoBack => !IsMainView && _navigationService.CanGoBack;

        public IEnumerable<Country> Countries => _orderService.Countries;

        public IEnumerable<OrderStatus> OrderStatuses => _orderService.OrderStatuses;

        public IEnumerable<PaymentType> PaymentTypes => _orderService.PaymentTypes;

        public IEnumerable<Shipper> Shippers => _orderService.Shippers;

        #endregion


        #region private method

        private async Task<bool> SaveItemAsync(Order model)
        {
            try
            {
                StartStatusMessage("Saving order...");
                await Task.Delay(100);
                await _orderService.UpdateOrderAsync(model);
                EndStatusMessage("Order saved");
                _logger.LogInformation(LogEvents.Save, $"Order #{model.Id} was saved successfully.");
                OnPropertyChanged(nameof(CanEditCustomer));
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order: {ex.Message}");
                _logger.LogError(LogEvents.Save, ex, "Error saving Order");
                return false;
            }
        }

        private async Task<bool> DeleteItemAsync(Order model)
        {
            try
            {
                StartStatusMessage("Deleting order...");
                await Task.Delay(100);
                await _orderService.DeleteOrderAsync(model);
                EndStatusMessage("Order deleted");
                _logger.LogWarning(LogEvents.Delete, $"Order #{model.Id} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order: {ex.Message}");
                _logger.LogError(LogEvents.Delete, ex, "Error deleting Order");
                return false;
            }
        }

        private async Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current order?", "Ok", "Cancel");
        }

        private IEnumerable<IValidationConstraint<Order>> GetValidationConstraints(Order model)
        {
            yield return new RequiredGreaterThanZeroConstraint<Order>("Customer", m => m.CustomerId);
            if (model.StatusId > 0)
            {
                yield return new RequiredConstraint<Order>("Payment Type", m => m.PaymentTypeId);
                yield return new RequiredGreaterThanZeroConstraint<Order>("Payment Type", m => m.PaymentTypeId);
                if (model.StatusId > 1)
                {
                    yield return new RequiredConstraint<Order>("Shipper", m => m.ShipperId);
                    yield return new RequiredGreaterThanZeroConstraint<Order>("Shipper", m => m.ShipperId);
                }
            }
        }

        private async void OnMessage(object recipient, ViewModelsMessage<Order> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Value)
                {
                    case "ItemChanged":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            try
                            {
                                //var item = await _orderRepository.GetOrderAsync(current.Id);
                                //item = item ?? new Order { Id = current.Id, IsEmpty = true };
                                //current.Merge(item);
                                //current.NotifyChanges();
                                //Item = await GetItemAsync(current.Id);
                                OnPropertyChanged(nameof(Title));
                                //if (IsEditMode)
                                //{
                                //    StatusMessage("WARNING: This order has been modified externally");
                                //}
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(LogEvents.HandleChanges, ex, "Handle Order Changes");
                            }
                        }
                        break;

                    case "ItemDeleted":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;

                    case "ItemsDeleted":
                        if (message.SelectedItems != null)
                        {
                            if (message.SelectedItems.Any(r => r.Id == current.Id))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;

                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await _orderService.GetOrderAsync(current.Id);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(LogEvents.HandleRangesDeleted, ex, "Handle Order Ranges Deleted");
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await Task.Run(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This order has been deleted externally");
            });
        }

        //protected async override Task<Order> GetItemAsync(long id)
        //{
        //    return await _orderService.GetOrderAsync(id);
        //}

        private async Task DeleteAsync()
        {
            var model = Item;
            if (model != null)
            {
                IsEnabled = false;
                if (await DeleteItemAsync(model))
                {
                    //MessageService.Send(this, "ItemDeleted", model);
                    Messenger.Send(new ViewModelsMessage<Order>("ItemDeleted", model.Id));
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        public virtual Result Validate(Order model)
        {
            foreach (var constraint in GetValidationConstraints(model))
            {
                if (!constraint.Validate(model))
                {
                    return Result.Error("Validation Error", constraint.Message);
                }
            }
            return Result.Ok();
        }

        private async Task SaveAsync()
        {
            IsEnabled = false;
            var isNew = ItemIsNew;
            if (await SaveItemAsync(Item))
            {
                OnPropertyChanged(nameof(Title));
                //EditableItem = Item;

                if (isNew)
                {
                    //MessageService.Send(this, "NewItemSaved", Item);
                    Messenger.Send(new ViewModelsMessage<Order>("NewItemSaved", Item.Id));
                }
                else
                {
                    //MessageService.Send(this, "ItemChanged", Item);
                    Messenger.Send(new ViewModelsMessage<Order>("ItemChanged", Item.Id));
                }
                IsEditMode = false;

                OnPropertyChanged(nameof(ItemIsNew));
            }
            IsEnabled = true;
        }

        #endregion
    }
}
