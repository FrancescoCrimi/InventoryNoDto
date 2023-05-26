﻿#region copyright
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

using Inventory.Domain.Aggregates.CustomerAggregate;
using Inventory.Uwp.ViewModels.Customers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Customers
{
    public sealed partial class CustomerCardControl : UserControl
    {
        public CustomerCardControl()
        {
            InitializeComponent();
        }

        #region ViewModel
        public CustomerDetailsViewModel ViewModel
        {
            get => (CustomerDetailsViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel),
                                        typeof(CustomerDetailsViewModel),
                                        typeof(CustomerCardControl),
                                        new PropertyMetadata(null));
        #endregion

        #region Item
        public Customer Item
        {
            get => (Customer)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item),
                                        typeof(Customer),
                                        typeof(CustomerCardControl),
                                        new PropertyMetadata(null));
        #endregion
    }
}
