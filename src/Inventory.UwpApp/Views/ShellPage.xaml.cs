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

using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.UwpApp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace Inventory.UwpApp.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            InitializeComponent();
            ShellViewModel viewModel = Ioc.Default.GetService<ShellViewModel>();
            DataContext = viewModel;
            viewModel.Initialize(shellFrame, KeyboardAccelerators);
        }
    }
}
