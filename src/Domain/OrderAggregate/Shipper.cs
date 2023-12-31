﻿// Copyright (c) Microsoft. All rights reserved.
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
    [Table("Shippers")]
    public class Shipper : Entity, IEquatable<Shipper>
    {
        private string name;
        private string phone;

        [Required]
        [MaxLength(50)]
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        [MaxLength(20)]
        public string Phone
        {
            get => phone;
            set => SetProperty(ref phone, value);
        }

        #region Equals

        public override bool Equals(object obj)
            => Equals(obj as Shipper);

        public bool Equals(Shipper other)
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
