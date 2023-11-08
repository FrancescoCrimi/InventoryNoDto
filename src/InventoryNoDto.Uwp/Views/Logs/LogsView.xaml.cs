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

using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.Contracts.Views;
using Inventory.Uwp.ViewModels.Logs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Logs
{
    public sealed partial class LogsView : UserControl, IView
    {
        public LogsViewModel ViewModel { get; private set; }

        private IWindowManagerService _windowService;

        public LogsView(IWindowManagerService windowManagerService, LogsViewModel viewModel)
        {
            InitializeComponent();
            _windowService = windowManagerService;
            ViewModel = viewModel;
        }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            var args = ViewModel.LogList.CreateArgs();
            args.IsMainView = false;
            await _windowService.OpenWindow(typeof(LogsView), args);
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

        public async void OnNavigatedTo(object parameter)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(parameter as LogListArgs);
            Bindings.Update();
        }

        public void OnNavigatedFrom()
        {
            Bindings.StopTracking();
            ViewModel.Unload();
            ViewModel.Unsubscribe();
            ViewModel = null;
            _windowService = null;
        }
    }
}
