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

using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Services;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Customers;
using Inventory.Uwp.ViewModels.Dashboard;
using Inventory.Uwp.ViewModels.Logs;
using Inventory.Uwp.ViewModels.OrderItems;
using Inventory.Uwp.ViewModels.Orders;
using Inventory.Uwp.ViewModels.Products;
using Inventory.Uwp.ViewModels.Settings;
using Inventory.Uwp.ViewModels;
using Inventory.Uwp.Views;
using Inventory.Uwp.Views.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.Views.Customers;
using Inventory.Uwp.Views.Logs;
using Inventory.Uwp.Views.Orders;
using Inventory.Uwp.Views.OrderItems;
using Inventory.Uwp.Views.Products;
using Inventory.Uwp.Views.Settings;

namespace Inventory.Uwp
{
    public sealed partial class App : Windows.UI.Xaml.Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            InitializeComponent();
            _serviceProvider = GetServiceProvider();
            Ioc.Default.ConfigureServices(_serviceProvider);
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!e.PrelaunchActivated)
            {
                await ActivateAsync(e);
            }
        }

        protected async override void OnActivated(IActivatedEventArgs e)
        {
            await ActivateAsync(e);
        }

        private async Task ActivateAsync(object args)
        {
            if (Window.Current.Content == null)
            {
                Window.Current.Content = _serviceProvider.GetService<ShellView>();
            }

            await InitializeAsync();

            object arguments = null;
            if (args is LaunchActivatedEventArgs launchArgs)
            {
                arguments = launchArgs.Arguments;
            }

            _serviceProvider.GetService<INavigationService>().Navigate(typeof(DashboardView), arguments);

            Window.Current.Activate();
            await StartupAsync();
        }

        private async Task InitializeAsync()
        {
            ThemeSelectorService.Initialize();
            //await WindowManagerService.Current.InitializeAsync();
            var appSettings = _serviceProvider.GetService<AppSettings>();
            await appSettings.EnsureLogDatabaseAsync();
            await appSettings.EnsureLocalDatabaseAsync();
            _serviceProvider
                .GetService<ILogger<App>>()
                .LogInformation(LogEvents.Startup, "Application Started");
        }

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //var logger = Ioc.Default.GetService<ILogger<App>>();
            //logger.LogInformation(LogEvents.Suspending, $"Application ended.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            //var logger = Ioc.Default.GetService<ILogger<App>>();
            //logger.LogError(LogEvents.UnhandledException, e.Exception, "Unhandled Exception");
        }

        private IServiceProvider GetServiceProvider()
        {
            return new ServiceCollection()

                // Services
                .AddScoped<INavigationService, NavigationService>()
                .AddScoped<IWindowManagerService, WindowManagerService>()
                .AddSingleton<AppSettings>()
                .AddSingleton<IAppSettings>(x => x.GetRequiredService<AppSettings>())

                // Core Services
                //.AddInventoryInfrastructure()
                //.AddInventoryPersistence()
                .AddInventoryApplication()
                .AddSingleton<FilePickerService>()

                // ViewModels
                .AddTransient<ShellViewModel>()
                .AddTransient<DashboardViewModel>()
                .AddTransient<SettingsViewModel>()
                .AddTransient<ValidateConnectionViewModel>()
                .AddTransient<CreateDatabaseViewModel>()
                .AddTransient<CustomerDetailsViewModel>()
                .AddTransient<CustomerListViewModel>()
                .AddTransient<CustomersViewModel>()
                .AddTransient<ProductListViewModel>()
                .AddTransient<ProductDetailsViewModel>()
                .AddTransient<ProductsViewModel>()
                .AddTransient<OrderDetailsViewModel>()
                .AddTransient<OrderViewModel>()
                .AddTransient<OrderListViewModel>()
                .AddTransient<OrdersViewModel>()
                .AddTransient<OrderItemDetailsViewModel>()
                .AddTransient<OrderItemListViewModel>()
                .AddTransient<OrderItemsViewModel>()
                .AddTransient<LogsViewModel>()
                .AddTransient<LogListViewModel>()
                .AddTransient<LogDetailsViewModel>()
                .AddTransient<CustomerCollection>()
                .AddTransient<LogCollection>()
                .AddTransient<OrderCollection>()
                .AddTransient<ProductCollection>()

                //.AddTransient<Views.Test.OrderDetailsViewModel>()
                //.AddTransient<Views.Test.OrderViewModel>()

                // View
                .AddTransient<ShellView>()
                .AddTransient<CustomerView>()
                .AddTransient<CustomersView>()
                .AddTransient<DashboardView>()
                .AddTransient<LogsView>()
                .AddTransient<OrderView>()
                .AddTransient<OrderItemView>()
                .AddTransient<OrderItemsView>()
                .AddTransient<OrdersView>()
                .AddTransient<ProductView>()
                .AddTransient<ProductsView>()
                .AddTransient<SettingsView>()

                .BuildServiceProvider();
        }
    }
}
