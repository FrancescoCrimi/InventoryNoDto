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

using Inventory.Uwp.Views.Settings;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Data;

namespace Inventory.Uwp.Helpers
{
    public class NavigationViewItemInvokedEventArgsToType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is NavigationViewItemInvokedEventArgs args)
            {
                var selectedItem = args.InvokedItemContainer as NavigationViewItem;

                if (args.IsSettingsInvoked)
                    if (selectedItem?.GetValue(NavTypeHelper.NavigateToProperty) == null)
                        NavTypeHelper.SetNavigateTo(selectedItem, typeof(SettingsView));

                var pageType = selectedItem?.GetValue(NavTypeHelper.NavigateToProperty) as Type;
                return pageType;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
