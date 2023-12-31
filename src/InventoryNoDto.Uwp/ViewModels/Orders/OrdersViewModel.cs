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
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.OrderAggregate;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.OrderItems;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        //private readonly OrderService _orderService;

        public OrdersViewModel(ILogger<OrdersViewModel> logger,
                               //OrderService orderService,
                               OrderListViewModel orderListViewModel,
                               OrderDetailsViewModel orderDetailsViewModel,
                               OrderItemListViewModel orderItemListViewModel)
            : base()
        {
            _logger = logger;
            //_orderService = orderService;
            OrderList = orderListViewModel;
            OrderDetails = orderDetailsViewModel;
            OrderItemList = orderItemListViewModel;
        }

        public OrderListViewModel OrderList { get; }

        public OrderDetailsViewModel OrderDetails { get; }

        public OrderItemListViewModel OrderItemList { get; }

        public async Task LoadAsync(OrderListArgs args)
        {
            await OrderList.LoadAsync(args);
            if (args != null)
            {
                IsMainView = args.IsMainView;
                OrderList.IsMainView = args.IsMainView;
                OrderDetails.IsMainView = args.IsMainView;
            }
        }

        public void Unload()
        {
            OrderDetails.CancelEdit();
            OrderList.Unload();
        }

        public void Subscribe()
        {
            //Messenger.Register<ViewModelsMessage<Order>>(this, OnMessage);
            OrderList.SelectedItemEvent += OnOrderListSelectedItem;

            OrderList.Subscribe();
            OrderDetails.Subscribe();
            OrderItemList.Subscribe();

            OrderDetails.CurrentOrderEvent += OnOrderDetailsCurrentOrderEvent;
        }

        public void Unsubscribe()
        {
            //Messenger.UnregisterAll(this);
            OrderList.SelectedItemEvent -= OnOrderListSelectedItem;

            OrderList.Unsubscribe();
            OrderDetails.Unsubscribe();
            OrderItemList.Unsubscribe();

            OrderDetails.CurrentOrderEvent -= OnOrderDetailsCurrentOrderEvent;
        }

        private async void OnOrderListSelectedItem(long id)
        {
            if (id != 0)
            {
                //TODO: rendere il metodo OnItemSelected cancellabile
                await OnItemSelected(id);
            }
        }

        //private async void OnMessage(object recipient, ViewModelsMessage<Order> message)
        //{
        //    if (message.Value == "ItemSelected")
        //    {
        //        if (message.Id != 0)
        //        {
        //            //TODO: rendere il metodo OnItemSelected cancellabile
        //            await OnItemSelected(message.Id);
        //        }
        //    }
        //}

        private async Task OnItemSelected(long id)
        {
            //if (OrderDetails.IsEditMode)
            //{
            //    StatusReady();
            //    OrderDetails.CancelEdit();
            //}
            OrderItemList.IsMultipleSelection = false;
            if (!OrderList.IsMultipleSelection)
            {
                if (id != 0)
                {
                    try
                    {
                        //var order = await _orderService.GetOrderAsync(id);
                        await OrderDetails.LoadAsync(new OrderDetailsArgs { /*Order = order,*/ OrderId = id });
                        //await OrderItemList.LoadAsync(new OrderItemListArgs { Order = order }, silent: true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(LogEvents.LoadOrder, ex, "Load OrderItems");
                    }
                }
            }
        }

        private async void OnOrderDetailsCurrentOrderEvent(Order order)
        {
            OrderItemList.IsMultipleSelection = false;
            if (!OrderList.IsMultipleSelection)
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs { Order = order }, silent: true);
            }
        }
    }
}
