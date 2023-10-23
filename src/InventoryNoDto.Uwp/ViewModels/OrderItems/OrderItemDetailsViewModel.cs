// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.OrderAggregate;
using Inventory.Domain.ProductAggregate;
using Inventory.Uwp.Common;
using Inventory.Uwp.Contracts.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemDetailsViewModel : /*GenericDetailsViewModel<OrderItem>*/ ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly INavigationService _navigationService;
        private readonly IWindowManagerService _windowService;
        private readonly OrderService _orderService;
        private RelayCommand<Product> _productSelectedCommand;
        private bool _isEditMode;
        private OrderItem _item;

        public OrderItemDetailsViewModel(ILogger<OrderItemDetailsViewModel> logger,
                                         INavigationService navigationService,
                                         IWindowManagerService windowService,
                                         OrderService orderService)
        //: base(navigationService, windowService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _windowService = windowService;
            //_orderItemRepository = orderItemRepository;
            _orderService = orderService;
        }

        #region property

        public override string Title => Item?.IsNew ?? true ? TitleNew : TitleEdit;

        public string TitleNew => $"New Order Item, Order #{OrderID}";

        public string TitleEdit => $"Order Line {Item?.OrderLine}, #{Item?.OrderId}" ?? string.Empty;

        public /*override*/ bool ItemIsNew => Item?.IsNew ?? true;

        public OrderItemDetailsArgs ViewModelArgs { get; private set; }

        public long OrderID { get; set; }

        public OrderItem Item
        {
            get => _item;
            set
            {
                if (SetProperty(ref _item, value))
                {
                    //EditableItem = _item;
                    //IsEnabled = (!_item?.IsEmpty) ?? false;
                    //OnPropertyChanged(nameof(IsDataAvailable));
                    //OnPropertyChanged(nameof(IsDataUnavailable));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public Order Order { get; set; }

        public IEnumerable<TaxType> TaxTypes => _orderService.TaxTypes;

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public bool CanGoBack => !IsMainView && _navigationService.CanGoBack;

        #endregion


        #region command

        public ICommand SaveCommand => new RelayCommand(() =>
        {
        });

        public ICommand CancelCommand => new RelayCommand(() =>
        {
            StatusReady();
            CancelEdit();
            Messenger.Send(new ViewModelsMessage<OrderItem>("CancelEdit", Item.Id));
        });

        public ICommand DeleteCommand => new RelayCommand(() =>
        {
        });

        public ICommand EditCommand => new RelayCommand(() =>
        {
            StatusReady();
            BeginEdit();
            //MessageService.Send(this, "BeginEdit", Item);
            Messenger.Send(new ViewModelsMessage<OrderItem>("BeginEdit", Item.Id));
        });

        public ICommand BackCommand => new RelayCommand(() =>
        {
        });


        public ICommand ProductSelectedCommand => _productSelectedCommand
            ?? (_productSelectedCommand = new RelayCommand<Product>((product) =>
            {
                Item = Order.CreateNewOrderItem(product);
            }));

        #endregion


        #region method

        public async Task LoadAsync(OrderItemDetailsArgs args)
        {
            ViewModelArgs = args;
            OrderID = ViewModelArgs.OrderId;
            Item = args.OrderItem;
            Order = args.Order;
            await Task.CompletedTask;

            if (ViewModelArgs.IsNew)
            {
                //Item = new OrderItem { OrderId = OrderID };
                IsEditMode = true;
            }
            //else
            //{
            //    try
            //    {
            //        var item = await _orderItemRepository.GetOrderItemAsync(OrderID, ViewModelArgs.OrderLine);
            //        Item = item ?? new OrderItem { OrderId = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(LogEvents.Load, ex, "Load OrderItem");
            //    }
            //}
        }

        public void Unload()
        {
            ViewModelArgs.OrderId = Item?.OrderId ?? 0;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<OrderItem>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }

        public OrderItemDetailsArgs CreateArgs()
        {
            return new OrderItemDetailsArgs
            {
                OrderId = Item?.OrderId ?? 0,
                OrderLine = Item?.OrderLine ?? 0
            };
        }

        public virtual void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                //// Create a copy for edit
                //var editableItem = new TModel();
                //editableItem.Merge(Item);
                //EditableItem = editableItem;
            }
        }

        public virtual void CancelEdit()
        {
            if (ItemIsNew)
            {
                // We were creating a new item: cancel means exit
                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await _windowService.CloseWindowAsync();
                    });
                }
                return;
            }

            //// We were editing an existing item: just cancel edition
            //if (IsEditMode)
            //{
            //    var item = GetItemAsync(Item.Id).Result;
            //    EditableItem = item;
            //}
            IsEditMode = false;
        }

        #endregion


        #region protected and private method

        protected /*override*/ IEnumerable<IValidationConstraint<OrderItem>> GetValidationConstraints(OrderItem model)
        {
            yield return new RequiredConstraint<OrderItem>("Product", m => m.ProductId);
            yield return new NonZeroConstraint<OrderItem>("Quantity", m => m.Quantity);
            yield return new PositiveConstraint<OrderItem>("Quantity", m => m.Quantity);
            yield return new LessThanConstraint<OrderItem>("Quantity", m => m.Quantity, 100);
            yield return new PositiveConstraint<OrderItem>("Discount", m => m.Discount);
            yield return new NonGreaterThanConstraint<OrderItem>("Discount", m => m.Discount, (double)model.Subtotal, "'Subtotal'");
        }

        private async void OnMessage(object recipient, ViewModelsMessage<OrderItem> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Value)
                {
                    case "ItemChanged":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            OnPropertyChanged(nameof(Title));
                            StatusMessage("WARNING: This orderItem has been modified externally");
                        }
                        break;

                    case "ItemDeleted":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;

                    case "ItemRangesDeleted":
                        await OnItemDeletedExternally();
                        break;

                    case "ItemsDeleted":
                        if (message.SelectedItems.Any(r => r.OrderId == current.OrderId && r.OrderLine == current.OrderLine))
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await Task.Run(() =>
            {
                //CancelEdit();
                //IsEnabled = false;
                StatusMessage("WARNING: This orderItem has been deleted externally");
            });
        }

        #endregion
    }
}
