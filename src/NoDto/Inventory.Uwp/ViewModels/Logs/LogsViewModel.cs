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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Logs
{
    public class LogsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly LogService _logService;

        public LogsViewModel(ILogger<LogsViewModel> logger,
                             LogService logService,
                             LogListViewModel appLogListViewModel,
                             LogDetailsViewModel appLogDetailsViewModel)
            : base()
        {
            _logger = logger;
            _logService = logService;
            AppLogList = appLogListViewModel;
            AppLogDetails = appLogDetailsViewModel;
        }

        public LogListViewModel AppLogList
        {
            get;
        }
        public LogDetailsViewModel AppLogDetails
        {
            get;
        }

        public async Task LoadAsync(LogListArgs args)
        {
            await _logService.MarkAllAsReadAsync();
            await AppLogList.LoadAsync(args);
        }

        public void Unload()
        {
            AppLogList.Unload();
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<AppLogListViewModel>(this, OnMessage);
            Messenger.Register<LogMessage>(this, OnMessage);
            AppLogList.Subscribe();
            AppLogDetails.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
            AppLogList.Unsubscribe();
            AppLogDetails.Unsubscribe();
        }

        private async void OnMessage(object recipient, LogMessage message)
        {
            if (/*recipient == AppLogList &&*/ message.Value == "ItemSelected")
            {
                await OnItemSelected();
            }
        }

        private async Task OnItemSelected()
        {
            //if (AppLogDetails.IsEditMode)
            //{
            StatusReady();
            //}
            var selected = AppLogList.SelectedItem;
            if (!AppLogList.IsMultipleSelection)
            {
                if (selected != null /*&& !selected.IsEmpty*/)
                {
                    await PopulateDetails(selected);
                }
            }
            AppLogDetails.Item = selected;
        }

        private async Task<Log> PopulateDetails(Log selected)
        {
            try
            {
                var model = await _logService.GetLogAsync(selected.Id);
                //selected.Merge(model);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadDetails, ex, "Load Log Details");
                return null;
            }
        }
    }
}
