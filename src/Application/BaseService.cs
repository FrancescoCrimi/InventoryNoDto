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
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Inventory.Application
{
    public abstract class BaseService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private static List<Country> _countries;
        private static List<OrderStatus> _orderStatuses;
        private static List<PaymentType> _paymentTypes;
        private static List<Shipper> _shippers;
        private static List<TaxType> _taxTypes;
        private static List<Category> _categories;

        public BaseService(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<Category> Categories
        {
            get
            {
                if (_categories == null)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                            _categories = repo.GetCategoriesAsync().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _categories = new List<Category>();
                        _logger.LogError(LogEvents.LoadCategories, ex, "Load Categories");
                    }
                }
                return _categories;
            }
        }

        public IEnumerable<Country> Countries
        {
            get
            {
                if (_countries == null)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repo = scope.ServiceProvider.GetService<ICustomerRepository>();
                            _countries = repo.GetCountriesAsync().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _countries = new List<Country>();
                        _logger.LogError(LogEvents.LoadCountryCodes, ex, "Load CountryCodes");
                    }
                }
                return _countries;
            }
        }

        public IEnumerable<OrderStatus> OrderStatuses
        {
            get
            {
                if (_orderStatuses == null)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                            _orderStatuses = repo.GetOrderStatusAsync().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _orderStatuses = new List<OrderStatus>();
                        _logger.LogError(LogEvents.LoadOrderStatus, ex, "Load OrderStatus");
                    }
                }
                return _orderStatuses;
            }
        }

        public IEnumerable<PaymentType> PaymentTypes
        {
            get
            {
                if (_paymentTypes == null)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                            _paymentTypes = repo.GetPaymentTypesAsync().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _paymentTypes = new List<PaymentType>();
                        _logger.LogError(LogEvents.LoadPaymentTypes, ex, "Load PaymentTypes");
                    }
                }
                return _paymentTypes;
            }
        }

        public IEnumerable<Shipper> Shippers
        {
            get
            {
                if (_shippers == null)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                            _shippers = repo.GetShippersAsync().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _shippers = new List<Shipper>();
                        _logger.LogError(LogEvents.LoadShippers, ex, "Load Shippers");
                    }
                }
                return _shippers;
            }
        }

        public IEnumerable<TaxType> TaxTypes
        {
            get
            {
                if (_taxTypes == null)
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                            _taxTypes = repo.GetTaxTypesAsync().Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _taxTypes = new List<TaxType>();
                        _logger.LogError(LogEvents.LoadTaxTypes, ex, "Load TaxTypes");
                    }
                }
                return _taxTypes;
            }
        }
    }
}
