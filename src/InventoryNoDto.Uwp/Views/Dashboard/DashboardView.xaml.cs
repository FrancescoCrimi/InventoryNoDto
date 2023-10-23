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
using Inventory.Uwp.ViewModels.Dashboard;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Dashboard
{
    public sealed partial class DashboardView : UserControl, IView
    {
        public DashboardViewModel ViewModel { get; private set; }

        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public void OnNavigatedTo(object parameter)
        {
            //ViewModel.Initialize(parameter);
        }

        public void OnNavigatedFrom()
        {
            Bindings.StopTracking();
            ViewModel = null;
        }
    }
}
