﻿#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Persistence.Repository
{
    internal class LookupTableRepository : ILookupTableRepository
    {
        private AppDbContext _dataSource = null;

        public LookupTableRepository(AppDbContext dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IList<Category>> GetCategoriesAsync()
        {
            return await _dataSource.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<IList<Country>> GetCountryCodesAsync()
        {
            return await _dataSource.Countries.AsNoTracking().ToListAsync();
        }

        public async Task<IList<OrderStatus>> GetOrderStatusAsync()
        {
            return await _dataSource.OrderStatuses.AsNoTracking().ToListAsync();
        }

        public async Task<IList<PaymentType>> GetPaymentTypesAsync()
        {
            return await _dataSource.PaymentTypes.AsNoTracking().ToListAsync();
        }

        public async Task<IList<Shipper>> GetShippersAsync()
        {
            return await _dataSource.Shippers.AsNoTracking().ToListAsync();
        }

        public async Task<IList<TaxType>> GetTaxTypesAsync()
        {
            return await _dataSource.TaxTypes.AsNoTracking().ToListAsync();
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
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                }
            }
        }
        #endregion
    }
}