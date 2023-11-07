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
using Inventory.Uwp.ViewModels.Orders;
using Inventory.Uwp.Views.OrderItems;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Orders
{
    public sealed partial class OrdersView : UserControl, IView
    {
        private IWindowManagerService _windowService;

        public OrdersView(IWindowManagerService windowManagerService, OrdersViewModel viewModel)
        {
            InitializeComponent();
            _windowService = windowManagerService;
            ViewModel = viewModel;
        }

        public OrdersViewModel ViewModel { get; private set; }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            var args = ViewModel.OrderList.CreateArgs();
            args.IsMainView = false;
            await _windowService.OpenWindowAsync(typeof(OrdersView), args);
        }

        // TODO: remove this method and put it iinto ViewModel
        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            //ViewModel.OrderDetails.CancelEdit();
            if (pivot.SelectedIndex == 0)
            {
                await _windowService.OpenWindowAsync(typeof(OrderView), ViewModel.OrderDetails.CreateArgs());
            }
            else
            {
                await _windowService.OpenWindowAsync(typeof(OrderItemsView), ViewModel.OrderItemList.CreateArgs());
            }
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

        public async void OnNavigatedTo(object parameter)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(parameter as OrderListArgs);
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
