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

using CiccioSoft.Inventory.Application.Models;
using CiccioSoft.Inventory.Application.Services;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class LogCollection : VirtualCollection<AppLogModel>
    {
        private DataRequest<Log> _dataRequest = null;
        private readonly ILogService logService;
        private readonly ILogger<LogCollection> logger = Ioc.Default.GetService<ILogger<LogCollection>>();

        public LogCollection(ILogService logService)
            : base()
        {
            this.logService = logService;
        }

        private AppLogModel _defaultItem = AppLogModel.CreateEmpty();

        protected override AppLogModel DefaultItem => _defaultItem;

        public async Task LoadAsync(DataRequest<Log> dataRequest)
        {
            try
            {
                _dataRequest = dataRequest;
                Count = await logService.GetLogsCountAsync(_dataRequest);
                Ranges[0] = await FetchDataAsync(0, RangeSize);
            }
            catch (Exception ex)
            {
                Count = 0;
                throw ex;
            }
        }

        protected override async Task<IList<AppLogModel>> FetchDataAsync(int rangeIndex, int rangeSize)
        {
            try
            {
                return await logService.GetLogsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fetch");
            }
            return null;
        }
    }
}
