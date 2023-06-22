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
    public class CustomerService : BaseService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ILogger<CustomerService> logger,
                               IServiceProvider serviceProvider,
                               IUnitOfWork unitOfWork,
                               ICustomerRepository customerRepository)
            : base(logger, serviceProvider)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
        }

        public async Task<IList<Customer>> GetCustomersAsync(int index, int length, DataRequest<Customer> request)
        {
            return await _customerRepository.GetCustomersAsync(index, length, request);
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            return await _customerRepository.GetCustomersCountAsync(request);
        }

        public async Task<Customer> GetCustomerAsync(long customerId)
        {
            return await _customerRepository.GetCustomerAsync(customerId);
        }

        public async Task UpdateCustomerAsync(Customer model)
        {
            await _customerRepository.UpdateCustomerAsync(model);
        }

        public async Task DeleteCustomersAsync(Customer model)
        {
            await _customerRepository.DeleteCustomersAsync(model);
        }

        public async Task DeleteCustomersAsync(IEnumerable<Customer> models)
        {
            foreach (var model in models)
            {
                await _customerRepository.DeleteCustomersAsync(model);
            }
        }

        public async Task DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request)
        {
            var items = await _customerRepository.GetCustomerKeysAsync(index, length, request);
            await _customerRepository.DeleteCustomersAsync(items.ToArray());
        }
    }
}
