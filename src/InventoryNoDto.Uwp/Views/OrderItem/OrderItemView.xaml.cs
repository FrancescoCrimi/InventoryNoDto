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

using Inventory.Uwp.Contracts.Views;
using Inventory.Uwp.ViewModels.OrderItems;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.OrderItems
{
    public sealed partial class OrderItemView : UserControl, IView
    {
        public OrderItemDetailsViewModel ViewModel { get; private set; }

        public OrderItemView(OrderItemDetailsViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }


        public void OnNavigatedFrom()
        {
            Bindings.StopTracking();
            ViewModel.Unload();
            ViewModel.Unsubscribe();
            ViewModel = null;
        }

        public async void OnNavigatedTo(object parameter)
        {
            //ViewModel.Initialize(XamlRoot);
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(parameter as OrderItemDetailsArgs);

            //if (ViewModel.IsEditMode)
            //{
            await Task.Delay(100);
            details.SetFocus();
            //}
            Bindings.Update();
        }
    }
}
