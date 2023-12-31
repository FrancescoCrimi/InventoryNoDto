﻿// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.CustomerAggregate;
using Inventory.Domain.ProductAggregate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Inventory.Domain.OrderAggregate
{
    public class Order : Infrastructure.Common.ObservableObject<Order>, IEquatable<Order>
    {
        #region fields

        private DateTimeOffset orderDate;
        private DateTimeOffset? shippedDate;
        private DateTimeOffset? deliveredDate;
        private string trackingNumber;
        private string shipAddress;
        private string shipCity;
        private string shipRegion;
        private string shipPostalCode;
        private string shipPhone;
        private DateTimeOffset lastModifiedOn;
        private string searchTerms;
        private long customerId;
        private long? paymentTypeId;
        private long shipCountryId;
        private long? shipperId;
        private long statusId;
        private PaymentType paymentType;
        private Customer customer;
        private Shipper shipper;
        private Country shipCountry;
        private OrderStatus status;
        private ObservableCollection<OrderItem> orderItems;

        #endregion


        #region constructor

        public Order()
        {
            orderItems = new ObservableCollection<OrderItem>();
        }

        #endregion


        #region property

        public DateTimeOffset OrderDate
        {
            get => orderDate;
            set => SetProperty(ref orderDate, value);
        }
        public DateTimeOffset? ShippedDate
        {
            get => shippedDate;
            set => SetProperty(ref shippedDate, value);
        }
        public DateTimeOffset? DeliveredDate
        {
            get => deliveredDate;
            set => SetProperty(ref deliveredDate, value);
        }
        public string TrackingNumber
        {
            get => trackingNumber;
            set => SetProperty(ref trackingNumber, value);
        }
        public string ShipAddress
        {
            get => shipAddress;
            set => SetProperty(ref shipAddress, value);
        }
        public string ShipCity
        {
            get => shipCity;
            set => SetProperty(ref shipCity, value);
        }
        public string ShipRegion
        {
            get => shipRegion;
            set => SetProperty(ref shipRegion, value);
        }
        public string ShipPostalCode
        {
            get => shipPostalCode;
            set => SetProperty(ref shipPostalCode, value);
        }
        public string ShipPhone
        {
            get => shipPhone;
            set => SetProperty(ref shipPhone, value);
        }
        public DateTimeOffset LastModifiedOn
        {
            get => lastModifiedOn;
            set => SetProperty(ref lastModifiedOn, value);
        }
        public string SearchTerms
        {
            get => searchTerms;
            set => SetProperty(ref searchTerms, value);
        }

        #endregion


        #region relation

        public long CustomerId
        {
            get => customerId;
            set => SetProperty(ref customerId, value);
        }
        public long? PaymentTypeId
        {
            get => paymentTypeId;
            set => SetProperty(ref paymentTypeId, value);
        }
        public long ShipCountryId
        {
            get => shipCountryId;
            set => SetProperty(ref shipCountryId, value);
        }
        public long? ShipperId
        {
            get => shipperId;
            set => SetProperty(ref shipperId, value);
        }
        public long StatusId
        {
            get => statusId;
            set
            {
                if (SetProperty(ref statusId, value))
                    UpdateStatusDependencies();
            }
        }


        public virtual Customer Customer
        {
            get => customer;
            set => SetProperty(ref customer, value);
        }
        public virtual PaymentType PaymentType
        {
            get => paymentType;
            set => SetProperty(ref paymentType, value);
        }
        public virtual Shipper Shipper
        {
            get => shipper;
            set => SetProperty(ref shipper, value);
        }
        public virtual Country ShipCountry
        {
            get => shipCountry;
            set => SetProperty(ref shipCountry, value);
        }
        public virtual OrderStatus Status
        {
            get => status;
            set
            {
                if (SetProperty(ref status, value))
                    UpdateStatusDependencies();
            }
        }
        public virtual ObservableCollection<OrderItem> OrderItems
        {
            get => orderItems;
            set => orderItems = value;
        }

        #endregion


        #region not mapped

        [NotMapped]
        public bool CanEditPayment => StatusId > 0;
        [NotMapped]
        public bool CanEditShipping => StatusId > 1;
        [NotMapped]
        public bool CanEditDelivery => StatusId > 2;
        [NotMapped]
        public string StatusDesc => Status == null ? string.Empty : Status.Name;
        [NotMapped]
        public string PaymentTypeDesc => PaymentType == null ? string.Empty : PaymentType.Name;
        [NotMapped]
        public string ShipViaDesc => Shipper == null ? string.Empty : Shipper.Name;
        [NotMapped]
        public string ShipCountryName => ShipCountry == null ? string.Empty : ShipCountry.Name;

        #endregion


        #region public method

        public string BuildSearchTerms() => $"{Id} {CustomerId} {ShipCity} {ShipRegion}".ToLower();

        public void AddOrderItem(OrderItem orderItem)
        {
            var orderItemLineNumber = OrderItems.Max(x => x.OrderLine) + 1;
            if (!orderItem.IsDraft)
            {
                orderItem.OrderLine = orderItemLineNumber;
                OrderItems.Add(orderItem);
            }
        }

        public void RemoveOrderItemLine(int index)
        {
            var orderItem = OrderItems.FirstOrDefault(x => x.OrderLine == index);
            if (orderItem != null)
            {
                OrderItems.Remove(orderItem);
                int idx = 1;
                foreach (var item in OrderItems.OrderBy(o => o.OrderLine))
                {
                    item.OrderLine = idx;
                    idx++;
                }
            }
        }

        public OrderItem CreateNewOrderItem(Product product)
        {
            var item = new OrderItem(product);
            item.Order = this;
            return item;
        }

        public static Order CreateNewOrder(Customer customer)
        {
            return new Order
            {
                StatusId = 0,
                OrderDate = DateTime.UtcNow,
                CustomerId = customer.Id,
                ShipAddress = customer.AddressLine1,
                ShipCity = customer.City,
                ShipRegion = customer.Region,
                ShipCountryId = customer.CountryId,
                ShipPostalCode = customer.PostalCode,
                Customer = customer
            };
        }

        #endregion


        #region private method

        private void UpdateStatusDependencies()
        {
            switch (StatusId)
            {
                case 0:
                case 1:
                    ShippedDate = null;
                    DeliveredDate = null;
                    break;
                case 2:
                    ShippedDate = ShippedDate ?? OrderDate;
                    DeliveredDate = null;
                    break;
                case 3:
                    ShippedDate = ShippedDate ?? OrderDate;
                    DeliveredDate = DeliveredDate ?? ShippedDate ?? OrderDate;
                    break;
            }

            //OnPropertyChanged(nameof(Status));
            //OnPropertyChanged(nameof(StatusId));
            OnPropertyChanged(nameof(CanEditPayment));
            OnPropertyChanged(nameof(CanEditShipping));
            OnPropertyChanged(nameof(CanEditDelivery));
        }

        #endregion


        #region equals

        public override bool Equals(object obj)
            => Equals(obj as Order);

        public bool Equals(Order other)
        {
            if (other is null)
                return false;
            if (Id != 0 && other.Id != 0)
                return Id == other.Id;
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            if (Id == 0)
                return base.GetHashCode();
            else
                return HashCode.Combine(Id);
        }

        #endregion
    }
}
