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

using CiccioSoft.Inventory.Infrastructure.Logging;
using CiccioSoft.Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    #region AppLogDetailsArgs
    public class LogDetailsArgs
    {
        static public LogDetailsArgs CreateDefault() => new LogDetailsArgs();

        public long AppLogID { get; set; }
    }
    #endregion

    public class LogDetailsViewModel : GenericDetailsViewModelReadOnly<Log>
    {
        private readonly ILogger logger;
        private readonly LogService logService;

        public LogDetailsViewModel(ILogger<LogListViewModel> logger,
                                      LogService logService)
            : base()
        {
            this.logger = logger;
            this.logService = logService;
        }

        override public string Title => "Activity Logs";

        public override bool ItemIsNew => false;

        public LogDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(LogDetailsArgs args)
        {
            ViewModelArgs = args ?? LogDetailsArgs.CreateDefault();

            try
            {
                var item = await logService.GetLogAsync(ViewModelArgs.AppLogID);
                Item = item ?? new Log { Id = 0/*, IsEmpty = true*/ };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load");
            }
        }

        public void Unload()
        {
            ViewModelArgs.AppLogID = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<AppLogDetailsViewModel, AppLogModel>(this, OnDetailsMessage);
            Messenger.Register<ItemMessage<Log>>(this, OnDetailsMessage);
            //MessageService.Subscribe<AppLogListViewModel>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<Log>>>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnListIndexRange);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public LogDetailsArgs CreateArgs()
        {
            return new LogDetailsArgs
            {
                AppLogID = Item?.Id ?? 0
            };
        }

        protected override Task<bool> SaveItemAsync(Log model)
        {
            throw new NotImplementedException();
        }

        protected override async Task<bool> DeleteItemAsync(Log model)
        {
            try
            {
                StartStatusMessage("Deleting log...");
                await Task.Delay(100);
                await logService.DeleteLogAsync(model);
                EndStatusMessage("Log deleted");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting log: {ex.Message}");
                logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current log?", "Ok", "Cancel");
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnDetailsMessage(object recipient, ItemMessage<Log> message)
        {
            var current = Item;
            if (current != null)
            {
                if (message.Value != null && message.Value.Id == current?.Id)
                {
                    switch (message.Message)
                    {
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        //private async void OnDetailsMessage(AppLogDetailsViewModel sender, string message, AppLogModel changed)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        if (changed != null && changed.Id == current?.Id)
        //        {
        //            switch (message)
        //            {
        //                case "ItemDeleted":
        //                    await OnItemDeletedExternally();
        //                    break;
        //            }
        //        }
        //    }
        //}

        private async void OnListMessage(object recipient, ItemMessage<IList<Log>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemsDeleted":
                        if (message.Value is IList<Log> deletedModels)
                        {
                            if (deletedModels.Any(r => r.Id == current.Id))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                }
            }
        }

        private async void OnListIndexRange(object recipient, ItemMessage<IList<IndexRange>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemRangesDeleted":
                        var model = await logService.GetLogAsync(current.Id);
                        if (model == null)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            //await ContextService.RunAsync(() =>
            //{
            await Task.Run(() =>
            {
                //CancelEdit();
                //IsEnabled = false;
                StatusMessage("WARNING: This log has been deleted externally");
            });
            //});
        }
    }
}