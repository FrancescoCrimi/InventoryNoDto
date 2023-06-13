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

using Inventory.Domain.Aggregates.ProductAggregate;

namespace Inventory.Domain.Aggregates.OrderAggregate
{
    public class OrderItem : Infrastructure.Common.ObservableObject<OrderItem>
    {
        #region fields

        private int orderLine;
        private int quantity;
        private decimal unitPrice;
        private decimal discount;
        private long orderId;
        private long productId;
        private long taxTypeId;
        private TaxType taxType;
        private Product product;
        private Order order;
        private bool isDraft = true;

        #endregion


        #region constructor

        internal protected OrderItem(Product product)
            : this(product, 1)
        {
        }

        internal protected OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        #endregion


        #region public property

        public int OrderLine
        {
            get => orderLine;
            internal protected set => SetProperty(ref orderLine, value);
        }
        public int Quantity
        {
            get => quantity;
            set
            {
                if (SetProperty(ref quantity, value))
                {
                    OnPropertyChanged(nameof(Subtotal));
                    OnPropertyChanged(nameof(Total));
                };
            }
        }
        public decimal UnitPrice
        {
            get => unitPrice;
            set => SetProperty(ref unitPrice, value);
        }
        public decimal Discount
        {
            get => discount;
            set => SetProperty(ref discount, value);
        }

        public long OrderId
        {
            get => orderId;
            set
            {
                if (SetProperty(ref orderId, value))
                    CheckIsDraft();
            }
        }
        public long ProductId
        {
            get => productId;
            set
            {
                if (SetProperty(ref productId, value))
                    CheckIsDraft();
            }
        }
        public long TaxTypeId
        {
            get => taxTypeId;
            set
            {
                if (SetProperty(ref taxTypeId, value))
                    CheckIsDraft();
            }
        }

        public bool IsDraft => isDraft;

        #endregion


        #region relation

        public virtual Order Order
        {
            get => order;
            set
            {
                if (SetProperty(ref order, value))
                    OrderId = order.Id;
            }
        }
        public virtual Product Product
        {
            get => product;
            set
            {
                if (SetProperty(ref product, value))
                {
                    ProductId = product.Id;
                    UnitPrice = product.ListPrice;
                    TaxType = product.TaxType;
                    OnPropertyChanged(nameof(Subtotal));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }
        public virtual TaxType TaxType
        {
            get => taxType;
            set
            {
                if (SetProperty(ref taxType, value))
                {
                    TaxTypeId = taxType.Id;
                    OnPropertyChanged(nameof(Subtotal));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        #endregion


        public decimal Subtotal => Quantity * UnitPrice;
        public decimal Total
        {
            get
            {
                var total = Subtotal - Discount;
                if (TaxType != null)
                {
                    total *= (1 + TaxType.Rate / 100m);
                }
                return total;
            }
        }


        private void CheckIsDraft()
        {
            isDraft = orderId == 0 || productId == 0 || taxTypeId == 0;
        }
    }
}
