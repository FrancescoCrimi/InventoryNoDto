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
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.OrderItems;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderViewModel : ViewModelBase
    {
        private readonly ILogger _logger;

        public OrderViewModel(ILogger<OrderViewModel> logger,
                              OrderDetailsViewModel orderDetailsViewModel,
                              OrderItemListViewModel orderItemListViewModel)
            : base()
        {
            _logger = logger;
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
                await OrderDetails.LoadAsync(new OrderDetailsArgs { OrderId = args.OrderId, CustomerId = args.CustomerId, Order = args.Order });
                //await OrderItemList.LoadAsync(new OrderItemListArgs { OrderId = args.OrderId, Order = args.Order });
            }
            else
            {
                await OrderDetails.LoadAsync(new OrderDetailsArgs { OrderId = args.OrderId, CustomerId = args.CustomerId, Order = args.Order });
                //await OrderItemList.LoadAsync(new OrderItemListArgs { OrderId = args.OrderId, Order = args.Order }, silent: true);
            }
        }

        public void Unload()
        {
            OrderDetails.CancelEdit();
            OrderDetails.Unload();
            OrderItemList.Unload();
        }

        public void Subscribe()
        {
            //Messenger.Register<ViewModelsMessage<Domain.OrderAggregate.Order>>(this, OnMessage);
            OrderDetails.Subscribe();
            OrderItemList.Subscribe();
            OrderDetails.CurrentOrderEvent += OnOrderDetailsCurrentOrderEvent;
        }

        public void Unsubscribe()
        {
            //Messenger.UnregisterAll(this);
            OrderDetails.Unsubscribe();
            OrderItemList.Unsubscribe();
            OrderDetails.CurrentOrderEvent -= OnOrderDetailsCurrentOrderEvent;
        }

        private async void OnOrderDetailsCurrentOrderEvent(Domain.OrderAggregate.Order order)
        {
            await OrderItemList.LoadAsync(new OrderItemListArgs { OrderId = order.Id, Order = order });
        }

        //private async void OnMessage(object recipient, ViewModelsMessage<Domain.OrderAggregate.Order> message)
        //{
        //    if (message.Value == "ItemChanged" && message.Id != 0)
        //    {
        //        await OrderItemList.LoadAsync(new OrderItemListArgs { OrderId = message.Id });
        //    }
        //}
    }
}
