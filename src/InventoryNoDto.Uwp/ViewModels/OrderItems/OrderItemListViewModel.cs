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
using Inventory.Domain.OrderAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.Views.OrderItems;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemListViewModel
        //: GenericListViewModel<OrderItem>
        : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly INavigationService _navigationService;
        private readonly IWindowManagerService _windowService;
        private Order Order;
        private string _query;
        private ObservableCollection<OrderItem> _items;
        private int _itemsCount;
        private OrderItem _selectedItem;
        private bool _isMultipleSelection;
        private RelayCommand _newCommand;
        private RelayCommand _refreshCommand;
        private RelayCommand _startSelectionCommand;
        private RelayCommand _cancelSelectionCommand;
        private RelayCommand<IList<object>> _selectItemsCommand;
        private RelayCommand<IList<object>> _deselectItemsCommand;
        private RelayCommand<IndexRange[]> _selectRangesCommand;
        private RelayCommand _deleteSelectionCommand;
        private RelayCommand _openInNewViewCommand;

        public OrderItemListViewModel(ILogger<OrderItemListViewModel> logger,
                                      INavigationService navigationService,
                                      IWindowManagerService windowService)
            : base()
        {
            _logger = logger;
            //_orderItemRepository = orderItemRepository;
            _navigationService = navigationService;
            _windowService = windowService;
        }

        public OrderItemListArgs ViewModelArgs
        {
            get; private set;
        }

        public async Task LoadAsync(OrderItemListArgs args, bool silent = false)
        {
            ViewModelArgs = args ?? new OrderItemListArgs();
            Query = ViewModelArgs.Query;

            Order = ViewModelArgs.Order;

            await RefreshAsync();
        }

        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<OrderItem>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }


        public OrderItemListArgs CreateArgs()
        {
            return new OrderItemListArgs
            {
                Query = Query,
                //OrderBy = ViewModelArgs.OrderBy,
                //OrderByDesc = ViewModelArgs.OrderByDesc,
                OrderId = ViewModelArgs.OrderId
            };
        }

        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        public ObservableCollection<OrderItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public int ItemsCount
        {
            get => _itemsCount;
            set => SetProperty(ref _itemsCount, value);
        }

        public OrderItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (!IsMultipleSelection)
                    {
                        // fix _selectedItem == null
                        if (_selectedItem != null)
                        {
                            //MessageService.Send(this, "ItemSelected", _selectedItem);
                            Messenger.Send(new ViewModelsMessage<OrderItem>("ItemSelected", _selectedItem.Id));
                        }
                    }
                }
            }
        }

        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => SetProperty(ref _isMultipleSelection, value);
        }

        public List<OrderItem> SelectedItems { get; protected set; }

        public IndexRange[] SelectedIndexRanges { get; protected set; }


        #region icommand property

        public ICommand OpenInNewViewCommand => _openInNewViewCommand
            ?? (_openInNewViewCommand = new RelayCommand(async () =>
            {
                if (SelectedItem != null)
                {
                    await _windowService.OpenWindow(typeof(OrderItemView), new OrderItemDetailsArgs { OrderId = SelectedItem.OrderId, OrderLine = SelectedItem.OrderLine, OrderItem = SelectedItem, Order = Order });
                }
            }));

        public ICommand NewCommand => _newCommand
            ?? (_newCommand = new RelayCommand(async () =>
            {
                if (IsMainView)
                {
                    await _windowService.OpenWindow(typeof(OrderItemView), new OrderItemDetailsArgs { OrderId = ViewModelArgs.OrderId, Order = Order });
                }
                else
                {
                    _navigationService.Navigate(typeof(OrderItemView), new OrderItemDetailsArgs { OrderId = ViewModelArgs.OrderId, Order = Order });
                }
                StatusReady();
            }));

        public ICommand RefreshCommand => _refreshCommand
            ?? (_refreshCommand = new RelayCommand(async () =>
            {
                StartStatusMessage("Loading order items...");
                if (await RefreshAsync())
                {
                    EndStatusMessage("Order items loaded");
                }
            }));

        public ICommand StartSelectionCommand => _startSelectionCommand
            ?? (_startSelectionCommand = new RelayCommand(() =>
            {
                StatusMessage("Start selection");
                SelectedItem = null;
                SelectedItems = new List<OrderItem>();
                SelectedIndexRanges = null;
                IsMultipleSelection = true;
            }));

        public ICommand CancelSelectionCommand => _cancelSelectionCommand
            ?? (_cancelSelectionCommand = new RelayCommand(() =>
        {
            StatusReady();
            SelectedItems = null;
            SelectedIndexRanges = null;
            IsMultipleSelection = false;
            SelectedItem = Items?.FirstOrDefault();
        }));

        public ICommand SelectItemsCommand => _selectItemsCommand
            ?? (_selectItemsCommand = new RelayCommand<IList<object>>((items) =>
            {
                StatusReady();
                if (IsMultipleSelection)
                {
                    SelectedItems.AddRange(items.Cast<OrderItem>());
                    StatusMessage($"{SelectedItems.Count} items selected");
                }
            }));

        public ICommand DeselectItemsCommand => _deselectItemsCommand
            ?? (_deselectItemsCommand = new RelayCommand<IList<object>>((items) =>
            {
                if (items?.Count > 0)
                {
                    StatusReady();
                }
                if (IsMultipleSelection)
                {
                    foreach (OrderItem item in items)
                    {
                        SelectedItems.Remove(item);
                    }
                    StatusMessage($"{SelectedItems.Count} items selected");
                }
            }));

        public ICommand SelectRangesCommand => _selectRangesCommand
            ?? (_selectRangesCommand = new RelayCommand<IndexRange[]>((indexRanges) =>

            {
                SelectedIndexRanges = indexRanges;
                var count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
                StatusMessage($"{count} items selected");
            }));

        public ICommand DeleteSelectionCommand => _deleteSelectionCommand
            ?? (_deleteSelectionCommand = new RelayCommand(async () =>
            {
                StatusReady();
                if (await _windowService.OpenDialog("Confirm Delete", "Are you sure you want to delete selected order items?", "Ok", "Cancel"))
                {
                    var count = 0;
                    try
                    {
                        //if (SelectedIndexRanges != null)
                        //{
                        //    count = SelectedIndexRanges.Sum(r => r.Length);
                        //    StartStatusMessage($"Deleting {count} order items...");
                        //    await DeleteRangesAsync(SelectedIndexRanges);
                        //    //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        //    Messenger.Send(new ViewModelsMessage<OrderItem>("ItemRangesDeleted", SelectedIndexRanges));
                        //}
                        //else
                        if (SelectedItems != null)
                        {
                            count = SelectedItems.Count();
                            StartStatusMessage($"Deleting {count} order items...");
                            await DeleteItemsAsync(SelectedItems);
                            //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                            Messenger.Send(new ViewModelsMessage<OrderItem>("ItemsDeleted", SelectedItems));
                        }
                    }
                    catch (Exception ex)
                    {
                        StatusError($"Error deleting {count} order items: {ex.Message}");
                        _logger.LogError(LogEvents.Delete, ex, $"Error deleting {count} order items");
                        count = 0;
                    }
                    await RefreshAsync();
                    SelectedIndexRanges = null;
                    SelectedItems = null;
                    if (count > 0)
                    {
                        EndStatusMessage($"{count} order items deleted");
                    }
                }
            }));

        #endregion


        private async Task<bool> RefreshAsync()
        {
            var isOk = true;
            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            if (Order != null)
            {
                Items = Order.OrderItems;
                ItemsCount = Items.Count;
            }

            OnPropertyChanged(nameof(Title));
            await Task.CompletedTask;
            return isOk;
        }

        private async Task DeleteItemsAsync(IEnumerable<OrderItem> models)
        {
            foreach (var model in models)
            {
                //await _orderItemRepository.DeleteOrderItemsAsync(model);
                Order.RemoveOrderItemLine(model.OrderLine);
                await Task.CompletedTask;
            }
        }

        //private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        //{
        //    DataRequest<OrderItem> request = BuildDataRequest();
        //    foreach (var range in ranges)
        //    {
        //        //await _orderItemService.DeleteOrderItemRangeAsync(range.Index, range.Length, request);
        //        var items = await _orderItemRepository.GetOrderItemKeysAsync(range.Index, range.Length, request);
        //        await _orderItemRepository.DeleteOrderItemsAsync(items.ToArray());
        //    }
        //}

        private DataRequest<OrderItem> BuildDataRequest()
        {
            var request = new DataRequest<OrderItem>()
            {
                Query = Query,
                //OrderBy = ViewModelArgs.OrderBy,
                //OrderByDesc = ViewModelArgs.OrderByDesc
            };
            if (ViewModelArgs.OrderId > 0)
            {
                request.Where = (r) => r.OrderId == ViewModelArgs.OrderId;
            }
            return request;
        }

        private async void OnMessage(object recipient, ViewModelsMessage<OrderItem> message)
        {
            switch (message.Value)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }
    }
}
