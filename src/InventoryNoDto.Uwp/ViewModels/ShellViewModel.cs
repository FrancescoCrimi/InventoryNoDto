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

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;

namespace Inventory.Uwp.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly ILogger<ShellViewModel> _logger;
        private readonly INavigationService _navigationService;
        private readonly LogService _logService;
        private readonly CoreDispatcher _dispatcher;
        private bool _isEnabled = true;
        private bool _isError = false;
        private string _message = "Ready";
        private int logCount = 10;
        private bool _isBackEnabled;
        private AsyncRelayCommand _loadedCommand;
        private RelayCommand _unloadedCommand;
        private AsyncRelayCommand<Type> _itemInvokedCommand;
        private RelayCommand _backRequestedCommand;

        public ShellViewModel(ILogger<ShellViewModel> logger,
                              INavigationService navigationService,
                              LogService logService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _logService = logService;
            _dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            _navigationService.Navigated += NavigationService_Navigated;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public int LogCount
        {
            get => logCount;
            set => SetProperty(ref logCount, value);
        }

        public bool IsBackEnabled
        {
            get => _isBackEnabled;
            set => SetProperty(ref _isBackEnabled, value);
        }

        public IAsyncRelayCommand LoadedCommand
            => _loadedCommand ?? (_loadedCommand = new AsyncRelayCommand(OnLoaded));

        public ICommand UnloadedCommand
            => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(OnUnloaded));

        public IAsyncRelayCommand ItemInvokedCommand
            => _itemInvokedCommand ?? (_itemInvokedCommand = new AsyncRelayCommand<Type>(OnItemInvoked));

        public ICommand BackRequestedCommand
            => _backRequestedCommand ?? (_backRequestedCommand = new RelayCommand(()
                => _navigationService.GoBack()));


        private async Task OnLoaded()
        {
            await UpdateAppLogBadge();
            LogService.AddLogEvent += Logging_AddLogEvent;
            Messenger.Register<StatusMessage>(this, OnStatusMessage);
        }

        private void OnUnloaded()
        {
            //MessageService.Unsubscribe(this);
            LogService.AddLogEvent -= Logging_AddLogEvent;
            Messenger.UnregisterAll(this);
        }

        private async Task OnItemInvoked(Type pageType)
        {
            if (pageType != null)
            {
                _navigationService.Navigate(pageType, null);
            }
            await UpdateAppLogBadge();
        }

        private void NavigationService_Navigated(object sender, EventArgs e)
        {
            IsBackEnabled = _navigationService.CanGoBack;
        }

   

        private void SetStatus(string message)
        {
            message = message ?? "";
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            Message = message;
        }

        private void OnStatusMessage(object recipient, StatusMessage message)
        {
            //    throw new NotImplementedException();
            //}

            //private async void OnMessage(ViewModelBase viewModel, string message, string status)
            //{
            switch (message.Value)
            {
                case "StatusMessage":
                case "StatusError":
                    IsError = message.Value == "StatusError";
                    SetStatus(message.Args);
                    break;

                case "EnableThisView":
                case "DisableThisView":
                    IsEnabled = message.Value == "EnableThisView";
                    SetStatus(message.Args);
                    break;

                case "EnableOtherViews":
                case "DisableOtherViews":
                    IsEnabled = message.Value == "EnableOtherViews";
                    SetStatus(message.Args);
                    break;

                case "EnableAllViews":
                case "DisableAllViews":
                    IsEnabled = message.Value == "EnableAllViews";
                    SetStatus(message.Args);
                    break;
            }
        }

        private async Task UpdateAppLogBadge()
        {
            //var result = await logService.GetLogsCountAsync(new DataRequest<Log> { Where = r => !r.IsRead });
            ////LogNewCount = await logService.GetLogsCountAsync(new DataRequest<Log> { Where = r => !r.IsRead });
            //////AppLogsItem.Badge = count > 0 ? count.ToString() : null;

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var result = await _logService.GetLogsCountAsync(new DataRequest<Log> { Where = r => !r.IsRead });
                LogCount = result;
            });
        }

        private async void Logging_AddLogEvent(object sender, EventArgs e)
        {
            await Task.CompletedTask;
            //await UpdateAppLogBadge();
        }
    }
}
