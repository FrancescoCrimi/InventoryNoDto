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
using Inventory.Infrastructure.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class DashboardService
    {
        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public DashboardService(ILogger<DashboardService> logger,
                                IServiceProvider serviceProvider)
        {
            _logger = logger;
            var scope = serviceProvider.CreateScope().ServiceProvider;
            _orderRepository = scope.GetRequiredService<IOrderRepository>();
            _customerRepository = scope.GetRequiredService<ICustomerRepository>();
            _productRepository = scope.GetRequiredService<IProductRepository>();

        }

        public async Task<IList<Customer>> GetCustomersAsync(int index, int length, DataRequest<Customer> request)
        {
            return await _customerRepository.GetCustomersAsync(index, length, request);
        }

        public async Task<IList<Order>> GetOrdersAsync(int index, int length, DataRequest<Order> request)
        {
            return await _orderRepository.GetOrdersAsync(index, length, request);
        }

        public async Task<IList<Product>> GetProductsAsync(int index, int length, DataRequest<Product> request)
        {
            return await _productRepository.GetProductsAsync(index, length, request);
        }
    }
}
