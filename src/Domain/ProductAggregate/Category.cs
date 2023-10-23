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

namespace Inventory.Domain.ProductAggregate
{
    public class Category : Entity, IEquatable<Category>
    {
        private string name;
        private string description;
        private byte[] picture;
        private byte[] thumbnail;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public byte[] Picture
        {
            get => picture;
            set => SetProperty(ref picture, value);
        }
        public byte[] Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }

        #region Equals

        public override bool Equals(object obj)
            => Equals(obj as Category);

        public bool Equals(Category other)
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
