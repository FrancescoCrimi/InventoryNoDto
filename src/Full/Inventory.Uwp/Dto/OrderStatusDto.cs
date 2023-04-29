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
using System.Collections.Generic;

namespace Inventory.Uwp.Dto
{
    public class OrderStatusDto : ObservableDto, IEquatable<OrderStatusDto>
    {
        //public int Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as OrderStatusDto);
        }
        public bool Equals(OrderStatusDto other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
        public static bool operator ==(OrderStatusDto left, OrderStatusDto right)
        {
            return EqualityComparer<OrderStatusDto>.Default.Equals(left, right);
        }
        public static bool operator !=(OrderStatusDto left, OrderStatusDto right)
        {
            return !(left == right);
        }
    }
}
