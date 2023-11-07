// Copyright (c) Microsoft. All rights reserved.
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
using Inventory.Uwp.ViewModels.Products;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Products
{
    public sealed partial class ProductsView : UserControl, IView
    {
        public ProductsViewModel ViewModel { get; private set; }

        private IWindowManagerService _windowService;

        public ProductsView(IWindowManagerService windowManagerService, ProductsViewModel viewModel)
        {
            InitializeComponent();
            _windowService = windowManagerService;
            ViewModel = viewModel;
        }

        // TODO: remove this method and put it iinto ViewModel
        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            var args = ViewModel.ProductList.CreateArgs();
            args.IsMainView = false;
            await _windowService.OpenWindowAsync(typeof(ProductsView), args);
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }

        public async void OnNavigatedTo(object parameter)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(parameter as ProductListArgs);
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
