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
using Inventory.Uwp.Helpers;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Inventory.Uwp.Views
{
    public sealed partial class ShellView : Page
    {
        public ShellView(INavigationService navigationService, ShellViewModel viewModel)
        {
            InitializeComponent();
            //ShellFrame.Navigated += OnFrameNavigated;
            ((NavigationService)navigationService).Initialize(ShellContentControl);
            ViewModel = viewModel;
            Debug.Print("ShellPage Costructor: " + GetHashCode().ToString() + "\n");
        }

        public ShellViewModel ViewModel { get; private set; }

        //private void OnPageLoaded(object sender, RoutedEventArgs e)
        //{
        //     = ServiceLocator.Get(UIContext).GetService<ShellViewModel>();
        //    Bindings.Update();
        //}

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Bindings.StopTracking();
            ViewModel = null;
        }


        #region NavigationView MenuItem Selected

        private void OnFrameNavigated(object sender,
                                      NavigationEventArgs e)
        {
            var selectedItem = GetSelectedItem(e.SourcePageType);
            if (selectedItem != null)
            {
                navigationView.SelectedItem = selectedItem;
            }
        }

        public WinUI.NavigationViewItem GetSelectedItem(Type pageType)
        {
            return GetSelectedItem(navigationView.MenuItems, pageType)
                ?? GetSelectedItem(navigationView.FooterMenuItems, pageType)
                ?? GetSelectedItem(new[] { navigationView.SettingsItem }, pageType);
        }

        private WinUI.NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavTypeHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        #endregion
    }
}
