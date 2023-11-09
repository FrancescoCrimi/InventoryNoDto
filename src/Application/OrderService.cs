// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.CustomerAggregate;
using Inventory.Domain.OrderAggregate;
using Inventory.Domain.ProductAggregate;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class OrderService : BaseService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderService(ILogger<OrderService> logger,
                            IServiceProvider serviceProvider,
                            IUnitOfWork unitOfWork,
                            IOrderRepository orderRepository,
                            ICustomerRepository customerRepository)
            : base(logger, serviceProvider)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _logger.LogWarning(LogEvents.Load, "OrderService started: {0}", GetHashCode().ToString());
        }

        public async Task<IList<Order>> GetOrdersAsync(int index, int length, DataRequest<Order> request)
        {
            IList<Order> list = await _orderRepository.GetOrdersAsync(index, length, request);
            return list;
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            return await _orderRepository.GetOrdersCountAsync(request);
        }

        public async Task<Order> GetOrderAsync(long orderId)
        {
            return await _orderRepository.GetOrderAsync(orderId);
        }

        public async Task UpdateOrderAsync(Order model)
        {
            await _orderRepository.UpdateOrderAsync(model);
        }

        public async Task DeleteOrderAsync(Order model)
        {
            await _orderRepository.DeleteOrdersAsync(model);
        }

        public async Task DeleteOrdersRangeAsync(int index, int length, DataRequest<Order> request)
        {
            var items = await _orderRepository.GetOrderKeysAsync(index, length, request);
            await _orderRepository.DeleteOrdersAsync(items.ToArray());
        }

        public async Task<Customer> GetCustomerAsync(long customerId)
        {
            return await _customerRepository.GetCustomerAsync(customerId);
        }
    }
}
