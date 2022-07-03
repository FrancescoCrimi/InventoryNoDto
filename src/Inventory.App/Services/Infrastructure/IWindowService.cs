﻿using System;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services.Infrastructure
{
    public interface IWindowService
    {
        Task CloseViewAsync();
        bool? OpenInDialog(Type viewModelType, object parameter = null);
        Task<int> OpenInNewWindow(Type viewModelType, object parameter = null);
        Task<int> OpenInNewWindow<TViewModel>(object parameter = null);
    }
}