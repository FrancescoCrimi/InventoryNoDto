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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.OrderAggregate;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.OrderItems;
using InventoryNoDto.Uwp.ViewModels.Orders;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly OrderService _orderService;

        public OrderViewModel(ILogger<OrderViewModel> logger,
                              OrderService orderService,
                              OrderDetailsViewModel orderDetailsViewModel,
                              OrderItemListViewModel orderItemListViewModel)
            : base()
        {
            _logger = logger;
            _orderService = orderService;
            OrderDetails = orderDetailsViewModel;
            OrderItemList = orderItemListViewModel;
        }

        public OrderDetailsViewModel OrderDetails { get; }

        public OrderItemListViewModel OrderItemList { get; }

        public async Task LoadAsync(OrderArgs args)
        {

            long orderId = args?.OrderId ?? 0;
            if (orderId > 0)
            {
                var order = await _orderService.GetOrderAsync(orderId);
                await OrderDetails.LoadAsync(new OrderDetailsArgs { OrderId = orderId, Order = order });
                await OrderItemList.LoadAsync(new OrderItemListArgs { OrderId = orderId, Order = order });
            }
            else
            {
                await OrderDetails.LoadAsync(new OrderDetailsArgs { OrderId = 0});
                await OrderItemList.LoadAsync(new OrderItemListArgs { OrderId = 0}, silent: true);
            }

        }

        public void Unload()
        {
            //OrderDetails.CancelEdit();
            OrderDetails.Unload();
            OrderItemList.Unload();
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<Order>>(this, OnMessage);
            OrderDetails.Subscribe();
            OrderItemList.Subscribe();
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
            OrderDetails.Unsubscribe();
            OrderItemList.Unsubscribe();
        }

        private async void OnMessage(object recipient, ViewModelsMessage<Order> message)
        {
            if (message.Value == "ItemChanged" && message.Id != 0)
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs { OrderId = message.Id });
            }
        }
    }
}
