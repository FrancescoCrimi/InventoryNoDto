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

using Inventory.Uwp.Contracts.Views;
using Inventory.Uwp.ViewModels.Orders;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Orders
{
    public sealed partial class OrderView : UserControl, IView
    {
        public OrderViewModel ViewModel { get; private set; }

        public OrderView(OrderViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public int GetRowSpan(bool isItemNew)
        {
            return isItemNew ? 2 : 1;
        }

        public async void OnNavigatedTo(object parameter)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(parameter as OrderArgs);

            // trick forse per il binding 
            if (ViewModel.OrderDetails.IsEditMode)
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
            ViewModel = null;
        }
    }
}
