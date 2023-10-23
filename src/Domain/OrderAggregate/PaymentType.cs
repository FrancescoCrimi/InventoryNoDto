// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.OrderAggregate
{
    [Table("PaymentTypes")]
    public class PaymentType : Entity, IEquatable<PaymentType>
    {
        private string name;

        [Required]
        [MaxLength(50)]
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        #region Equals

        public override bool Equals(object obj)
            => Equals(obj as PaymentType);

        public bool Equals(PaymentType other)
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
