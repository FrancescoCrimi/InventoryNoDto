﻿// Copyright (c) Microsoft. All rights reserved.
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.OrderAggregate;
using Inventory.Domain.ProductAggregate;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemDetailsViewModel : /*GenericDetailsViewModel<OrderItem>*/ ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly OrderService _orderService;

        public OrderItemDetailsViewModel(ILogger<OrderItemDetailsViewModel> logger,
                                         NavigationService navigationService,
                                         WindowManagerService windowService,
                                         OrderService orderService)
        //: base(navigationService, windowService)
        {
            _logger = logger;
            //_orderItemRepository = orderItemRepository;
            _orderService = orderService;
        }

        #region property

        public override string Title => Item?.IsNew ?? true ? TitleNew : TitleEdit;

        public string TitleNew => $"New Order Item, Order #{OrderID}";

        public string TitleEdit => $"Order Line {Item?.OrderLine}, #{Item?.OrderId}" ?? string.Empty;

        public /*override*/ bool ItemIsNew => Item?.IsNew ?? true;

        public OrderItemDetailsArgs ViewModelArgs
        {
            get; private set;
        }

        public long OrderID
        {
            get; set;
        }

        public OrderItem Item { get; set; }
        public Order Order { get; set; }

        public ICommand ProductSelectedCommand => new RelayCommand<Product>(ProductSelected);
        private void ProductSelected(Product product)
        {
            Item = Order.CreateNewOrderItem(product);
        }


        public IEnumerable<TaxType> TaxTypes => _orderService.TaxTypes;

        #endregion


        #region method

        public async Task LoadAsync(OrderItemDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderItemDetailsArgs.CreateDefault();
            OrderID = ViewModelArgs.OrderID;
            Item = args.OrderItem;
            Order = args.Order;
            await Task.CompletedTask;

            //if (ViewModelArgs.IsNew)
            //{
            //    Item = new OrderItem { OrderId = OrderID };
            //    IsEditMode = true;
            //}
            //else
            //{
            //    try
            //    {
            //        var item = await _orderItemRepository.GetOrderItemAsync(OrderID, ViewModelArgs.OrderLine);
            //        Item = item ?? new OrderItem { OrderId = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(LogEvents.Load, ex, "Load OrderItem");
            //    }
            //}
        }

        public void Unload()
        {
            ViewModelArgs.OrderID = Item?.OrderId ?? 0;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<OrderItem>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }

        public OrderItemDetailsArgs CreateArgs()
        {
            return new OrderItemDetailsArgs
            {
                OrderID = Item?.OrderId ?? 0,
                OrderLine = Item?.OrderLine ?? 0
            };
        }

        #endregion


        #region protected and private method

 



        protected /*override*/ IEnumerable<IValidationConstraint<OrderItem>> GetValidationConstraints(OrderItem model)
        {
            yield return new RequiredConstraint<OrderItem>("Product", m => m.ProductId);
            yield return new NonZeroConstraint<OrderItem>("Quantity", m => m.Quantity);
            yield return new PositiveConstraint<OrderItem>("Quantity", m => m.Quantity);
            yield return new LessThanConstraint<OrderItem>("Quantity", m => m.Quantity, 100);
            yield return new PositiveConstraint<OrderItem>("Discount", m => m.Discount);
            yield return new NonGreaterThanConstraint<OrderItem>("Discount", m => m.Discount, (double)model.Subtotal, "'Subtotal'");
        }


        private async void OnMessage(object recipient, ViewModelsMessage<OrderItem> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Value)
                {
                    case "ItemChanged":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            OnPropertyChanged(nameof(Title));
                            StatusMessage("WARNING: This orderItem has been modified externally");
                        }
                        break;

                    case "ItemDeleted":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;

                    case "ItemRangesDeleted":
                        await OnItemDeletedExternally();
                        break;

                    case "ItemsDeleted":
                        if (message.SelectedItems.Any(r => r.OrderId == current.OrderId && r.OrderLine == current.OrderLine))
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await Task.Run(() =>
            {
                //CancelEdit();
                //IsEnabled = false;
                StatusMessage("WARNING: This orderItem has been deleted externally");
            });
        }

        #endregion
    }
}
