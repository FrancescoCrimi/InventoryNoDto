// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Uwp.Contracts.Views;
using Inventory.Uwp.ViewModels.Products;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Products
{
    public sealed partial class ProductView : UserControl, IView
    {
        public ProductDetailsViewModel ViewModel { get; private set; }

        public ProductView(ProductDetailsViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public async void OnNavigatedTo(object parameter)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(parameter as ProductDetailsArgs);

            if (ViewModel.IsEditMode)
            {
                await Task.Delay(100);
                details.SetFocus();
            }
            Bindings.Update();
        }

        public void OnNavigatedFrom()
        {
            Bindings.StopTracking();
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }
    }
}
