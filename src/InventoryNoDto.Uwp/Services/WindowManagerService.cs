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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Inventory.Uwp.Services
{
    public class WindowManagerService : IWindowManagerService, IDisposable
    {
        private static readonly Dictionary<UIContext, AppWindow> _appWindows
            = new Dictionary<UIContext, AppWindow>();

        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IServiceScope _scope;
        private AppWindow _appWindow;
        private ContentControl _contentControl;
        private bool _disposedValue;

        public WindowManagerService(ILogger<WindowManagerService> logger,
                                    IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _logger.LogInformation("Constructor: {HashCode}", GetHashCode().ToString());
        }

        public async Task OpenWindow(Type pageType, object parameter = null, string windowTitle = "")
        {
            var scope = _serviceScopeFactory.CreateScope();
            var wms = (WindowManagerService)scope.ServiceProvider.GetRequiredService<IWindowManagerService>();
            await wms.OpenWindow(scope, pageType, parameter, windowTitle);
        }

        private async Task OpenWindow(IServiceScope scope,
                                     Type viewType,
                                     object parameter = null,
                                     string windowTitle = "")
        {
            _scope = scope;

            _appWindow = await AppWindow.TryCreateAsync();
            _appWindow.Title = windowTitle;

            _contentControl = new ContentControl
            {
                VerticalContentAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };

            var navigationService = (NavigationService)_scope.ServiceProvider.GetService<INavigationService>();
            navigationService.Initialize(_contentControl);

            ElementCompositionPreview.SetAppWindowContent(_appWindow, _contentControl);

            _appWindows.Add(_contentControl.UIContext, _appWindow);

            navigationService.Navigate(viewType, parameter);

            _appWindow.Closed += AppWindow_Closed;

           await _appWindow.TryShowAsync();
        }

        private void AppWindow_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
            _appWindow.Closed -= AppWindow_Closed;
            _appWindows.Remove(_contentControl.UIContext);
            //navigationService = null;

            _contentControl.Content = null;
            _contentControl = null;
            _appWindow = null;

            _scope.Dispose();
            _scope = null;
        }

        public async Task CloseWindowAsync()
        {
            if (_appWindow != null)
            {
                await _appWindow.CloseAsync();
            }
            else
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync().AsTask();
            }
        }

        public async Task CloseAllWindowsAsync()
        {
            foreach (var pair in _appWindows.ToList())
            {
                await pair.Value.CloseAsync();
                //await Task.CompletedTask;
            }
            System.GC.Collect();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti)
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
                // TODO: impostare campi di grandi dimensioni su Null
                _disposedValue = true;
            }
            _logger.LogInformation("Dispose: {HashCode} - {disposing}", GetHashCode().ToString(), disposing.ToString());
        }

        // // TODO: eseguire l'override del finalizzatore solo se 'Dispose(bool disposing)' contiene codice per liberare risorse non gestite
        // ~WindowManagerService()
        // {
        //     // Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
