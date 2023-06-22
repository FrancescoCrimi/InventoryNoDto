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

using Inventory.Domain.OrderAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Persistence.Repository
{
    internal class OrderRepository : IOrderRepository
    {
        public OrderRepository(UnitOfWork unitOfWork)
        {
            DbContext = unitOfWork.DbContext;
        }

        private AppDbContext DbContext { get; set; }

        public async Task<IList<Order>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            var items = GetOrders(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Order>> GetOrderKeysAsync(int skip, int take, DataRequest<Order> request)
        {
            var items = GetOrders(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Order
                {
                    Id = r.Id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            IQueryable<Order> items = DbContext.Orders;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<Order> GetOrderAsync(long id)
        {
            return await DbContext.Orders.Where(r => r.Id == id)
                //.Include(r => r.Customer)
                //.Include(o => o.Status)
                //.Include(o => o.ShipCountry)
                //.AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            if (order.Id > 0)
            {
                DbContext.Entry(order).State = EntityState.Modified;
            }
            else
            {
                order.Id = UIDGenerator.Next(4);
                order.OrderDate = DateTime.UtcNow;
                DbContext.Entry(order).State = EntityState.Added;
            }
            order.LastModifiedOn = DateTime.UtcNow;
            order.SearchTerms = order.BuildSearchTerms();
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteOrdersAsync(params Order[] orders)
        {
            DbContext.Orders.RemoveRange(orders);
            return await DbContext.SaveChangesAsync();
        }


        public async Task<List<OrderStatus>> GetOrderStatusAsync()
        {
            return await DbContext.OrderStatuses.AsNoTracking().ToListAsync();
        }

        public async Task<List<PaymentType>> GetPaymentTypesAsync()
        {
            return await DbContext.PaymentTypes.AsNoTracking().ToListAsync();
        }

        public async Task<List<Shipper>> GetShippersAsync()
        {
            return await DbContext.Shippers.AsNoTracking().ToListAsync();
        }


        private IQueryable<Order> GetOrders(DataRequest<Order> request)
        {
            IQueryable<Order> items = DbContext.Orders;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order By
            if (request.OrderBy != null)
            {
                items = items.OrderBy(request.OrderBy);
            }
            if (request.OrderByDesc != null)
            {
                items = items.OrderByDescending(request.OrderByDesc);
            }

            return items;
        }


        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }
        #endregion
    }
}
