#region copyright
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

using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.Contracts.Views;
using Inventory.Uwp.ViewModels.OrderItems;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.OrderItems
{
    public sealed partial class OrderItemsView : UserControl, IView
    {
        public OrderItemsViewModel ViewModel { get; private set; }

        private IWindowManagerService _windowService;

        public OrderItemsView(IWindowManagerService windowManagerService, OrderItemsViewModel viewModel)
        {
            InitializeComponent();
            _windowService = windowManagerService;
            ViewModel = viewModel;
        }

        // TODO: remove this method and put it iinto ViewModel
        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            await _windowService.OpenWindow(typeof(OrderItemsView), ViewModel.OrderItemList.CreateArgs());
        }

        // TODO: remove this method and put it iinto ViewModel
        private async void OpenDetailsInNewView(object sender, RoutedEventArgs e)
        {
            //ViewModel.OrderItemDetails.CancelEdit();
            await _windowService.OpenWindow(typeof(OrderItemView), ViewModel.OrderItemDetails.CreateArgs());
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

        public async void OnNavigatedTo(object parameter)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(parameter as OrderItemListArgs);
            Bindings.Update();
        }

        public void OnNavigatedFrom()
        {
            Bindings.StopTracking();
            ViewModel.Unload();
            ViewModel.Unsubscribe();
            ViewModel = null;
        }
    }
}
