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

using Inventory.Uwp.ViewModels.OrderItems;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.OrderItem
{
    public sealed partial class OrderItemDetailsControl : UserControl
    {
        public OrderItemDetailsControl()
        {
            InitializeComponent();
        }

        #region ViewModel
        public OrderItemDetailsViewModel ViewModel
        {
            get { return (OrderItemDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
                                        typeof(OrderItemDetailsViewModel),
                                        typeof(OrderItemDetailsControl),
                                        new PropertyMetadata(null));
        #endregion

        public void SetFocus()
        {
            details.SetFocus();
        }
    }
}
