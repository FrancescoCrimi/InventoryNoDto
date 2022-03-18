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

using System;
using System.Diagnostics;

using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Models;
using CiccioSoft.Inventory.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace CiccioSoft.Inventory.ViewModels
{
    public class ViewModelBase : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient
    {
        private Stopwatch _stopwatch = new Stopwatch();

        public ViewModelBase()
        {
            //ContextService = Ioc.Default.GetService<IContextService>();
            //NavigationService = Ioc.Default.GetService<INavigationService>();
            //MessageService = Ioc.Default.GetService<IMessageService>();
            //DialogService = Ioc.Default.GetService<IDialogService>();
            //LogService = Ioc.Default.GetService<ILogService>();
            //IsMainView = false;
        }

        //public IContextService ContextService { get; }
        //public INavigationService NavigationService { get; }
        //public IMessageService MessageService { get; }
        //public IDialogService DialogService { get; }
        //public ILogService LogService { get; }

        //public bool IsMainView => ContextService.IsMainView;
        public bool IsMainView => true;

        virtual public string Title => String.Empty;

        public void StartStatusMessage(string message)
        {
            StatusMessage(message);
            _stopwatch.Reset();
            _stopwatch.Start();
        }
        public void EndStatusMessage(string message)
        {
            _stopwatch.Stop();
            StatusMessage($"{message} ({_stopwatch.Elapsed.TotalSeconds:#0.000} seconds)");            
        }

        public void StatusReady()
        {
            //MessageService.Send(this, "StatusMessage", "Ready");
            Messenger.Send(new StatusMessage("Ready", "StatusMessage"));
        }
        public void StatusMessage(string message)
        {
            //MessageService.Send(this, "StatusMessage", message);
            Messenger.Send(new StatusMessage(message, "StatusMessage"));
        }
        public void StatusError(string message)
        {
            //MessageService.Send(this, "StatusError", message);
            Messenger.Send(new StatusMessage(message, "StatusError"));
        }

        public void EnableThisView(string message = null)
        {
            message = message ?? "Ready";
            //MessageService.Send(this, "EnableThisView", message);
            Messenger.Send(new StatusMessage(message, "EnableThisView"));
        }
        public void DisableThisView(string message)
        {
            //MessageService.Send(this, "DisableThisView", message);
            Messenger.Send(new StatusMessage(message, "DisableThisView"));
        }

        public void EnableOtherViews(string message = null)
        {
            message = message ?? "Ready";
            //MessageService.Send(this, "EnableOtherViews", message);
            Messenger.Send(new StatusMessage(message, "EnableOtherViews"));
        }
        public void DisableOtherViews(string message)
        {
            //MessageService.Send(this, "DisableOtherViews", message);
            Messenger.Send(new StatusMessage(message, "DisableOtherViews"));
        }

        public void EnableAllViews(string message = null)
        {
            message = message ?? "Ready";
            //MessageService.Send(this, "EnableAllViews", message);
            Messenger.Send(new StatusMessage(message, "EnableAllViews"));
        }
        public void DisableAllViews(string message)
        {
            //MessageService.Send(this, "DisableAllViews", message);
            Messenger.Send(new StatusMessage(message, "DisableAllViews"));
        }
    }
}
