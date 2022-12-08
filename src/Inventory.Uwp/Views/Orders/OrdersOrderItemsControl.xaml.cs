﻿using Inventory.Uwp.ViewModels.OrderItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Controllo utente è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234236

namespace Inventory.Uwp.Views.Orders
{
    public sealed partial class OrdersOrderItemsControl : UserControl
    {
        public OrdersOrderItemsControl()
        {
            this.InitializeComponent();
        }

        #region ViewModel
        public OrderItemListViewModel ViewModel
        {
            get { return (OrderItemListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(OrderItemListViewModel), typeof(OrdersOrderItemsControl), new PropertyMetadata(null));
        #endregion
    }
}