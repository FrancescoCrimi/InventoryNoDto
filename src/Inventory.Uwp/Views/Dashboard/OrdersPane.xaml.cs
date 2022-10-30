﻿using Inventory.Uwp.Dto;
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

namespace Inventory.Uwp.Views.Dashboard
{
    public sealed partial class OrdersPane : UserControl
    {
        public OrdersPane()
        {
            this.InitializeComponent();
        }

        #region ItemsSource
        public IList<OrderDto> ItemsSource
        {
            get { return (IList<OrderDto>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList<OrderDto>), typeof(OrdersPane), new PropertyMetadata(null));
        #endregion
    }
}