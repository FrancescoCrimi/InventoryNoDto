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
using Inventory.Uwp.Contracts.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Services
{
    public class NavigationService : INavigationService, IDisposable
    {
        public event EventHandler Navigated;

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<UserControl> _forwardStack;
        private readonly Stack<UserControl> _backStack;
        private ContentControl _contentControl;
        private object _lastParameterUsed;
        private bool _disposedValue;

        public NavigationService(ILogger<NavigationService> logger,
                                 IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _forwardStack = new Stack<UserControl>();
            _backStack = new Stack<UserControl>();
            _logger.LogInformation("Constructor: {HashCode}", GetHashCode().ToString());
        }

        public void Initialize(ContentControl frame)
        {
            if (frame is null)
            {
                throw new ArgumentNullException(nameof(frame));
            }
            if (_contentControl == null)
            {
                _contentControl = frame;
            }
            else
            {
                throw new Exception("NavigationService already initialized");
            }
        }

        public bool CanGoBack => _backStack.Count != 0;

        public bool CanGoForward => _forwardStack.Count != 0;

        public void GoBack()
        {
            if (_backStack.Count != 0)
            {
                _forwardStack.Push((UserControl)ContentControl.Content);
                ContentControl.Content = _backStack.Peek();
                _backStack.Pop();
                Navigated?.Invoke(this, new EventArgs());
            }
        }

        public void GoForward()
        {
            if (_forwardStack.Count != 0)
            {
                _backStack.Push((UserControl)ContentControl.Content);
                ContentControl.Content = _forwardStack.Peek();
                _forwardStack.Pop();
                Navigated?.Invoke(this, new EventArgs());
            }
        }

        public bool UpdateView(Type pageType,
                               object parameter = null)
        {
            //var pageType = _pageService.GetPageType(pageKey);
            if (pageType == null || !pageType.IsSubclassOf(typeof(UserControl)))
            {
                throw new ArgumentException($"Invalid pageType '{pageType}', please provide a valid pageType.", nameof(pageType));
            }

            // Don't open the same page multiple times
            if (ContentControl.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed)))
            {
                _lastParameterUsed = parameter;
                var page = _serviceProvider.GetRequiredService(pageType);

                // termina pagina corrente
                if (ContentControl.Content != null)
                {
                    if (ContentControl.Content is IView pagefrom)
                    {
                        pagefrom.OnNavigatedFrom();
                    }
                }

                TerminateBackStack();
                TerminateForwardStack();

                // visualizza nuova pagina
                ContentControl.Content = page;

                // inizializza nuova pagina
                if (page is IView navigationAware)
                {
                    navigationAware.OnNavigatedTo(parameter);
                }

                Navigated?.Invoke(this, new EventArgs());
                return true;
            }
            return false;
        }

        public bool Navigate(Type pageType,
                             object parameter = null)
        {
            //var pageType = _pageService.GetPageType(pageKey);
            if (pageType == null || !pageType.IsSubclassOf(typeof(UserControl)))
            {
                throw new ArgumentException($"Invalid pageType '{pageType}', please provide a valid pageType.", nameof(pageType));
            }

            // Don't open the same page multiple times
            if (ContentControl.Content?.GetType() != pageType || parameter != null && !parameter.Equals(_lastParameterUsed))
            {
                _lastParameterUsed = parameter;
                var page = _serviceProvider.GetRequiredService(pageType);

                // copia pagina corrente nel backstack
                if (ContentControl.Content != null)
                {
                    _backStack.Push((UserControl)ContentControl.Content);
                }

                TerminateForwardStack();

                // visualizza nuova pagina
                ContentControl.Content = page;

                // inizializza nuova pagina
                if (page is IView navigationAware)
                {
                    navigationAware.OnNavigatedTo(parameter);
                }

                Navigated?.Invoke(this, new EventArgs());
                return true;
            }
            return false;
        }


        private ContentControl ContentControl
        {
            get
            {
                if (_contentControl == null)
                {
                    throw new Exception("NavigationService must be Initialize before use it");
                }
                return _contentControl;
            }
        }

        private void TerminateBackStack()
        {
            foreach (var item in _backStack)
            {
                if (item is IView itemfrom)
                {
                    itemfrom.OnNavigatedFrom();
                }
            }
            _backStack.Clear();
        }

        private void TerminateForwardStack()
        {
            foreach (var item in _forwardStack)
            {
                if (item is IView itemfrom)
                {
                    itemfrom.OnNavigatedFrom();
                }
            }
            _forwardStack.Clear();
        }


        #region dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti)
                    TerminateBackStack();
                    TerminateForwardStack();
                    if (_contentControl != null)
                    {
                        if (_contentControl.Content is IView navigationAware)
                        {
                            navigationAware.OnNavigatedFrom();
                        }
                        _contentControl.Content = null;
                        _contentControl = null;
                    }
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
                // TODO: impostare campi di grandi dimensioni su Null
                _disposedValue = true;
            }
            _logger.LogInformation("Dispose: {HashCode} - {disposing}", GetHashCode().ToString(), disposing.ToString());
        }

        // // TODO: eseguire l'override del finalizzatore solo se 'Dispose(bool disposing)' contiene codice per liberare risorse non gestite
        // ~NavigationService()
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

        #endregion
    }
}
