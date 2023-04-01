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

using Inventory.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace Inventory.Persistence.DbContexts
{
    public class SQLiteAppDbContext : AppDbContext
    {
        public SQLiteAppDbContext(DbContextOptions<SQLiteAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>().HasKey(e => new { e.OrderID, e.OrderLine });

            //modelBuilder.Entity<Order>()
            //    .Property(e => e.OrderDate)
            //    .HasConversion(new DateTimeOffsetToBinaryConverter());

            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
            //                                                                || p.PropertyType == typeof(DateTimeOffset?));
            //    foreach (var property in properties)
            //    {
            //        modelBuilder
            //            .Entity(entityType.Name)
            //            .Property(property.Name)
            //            .HasConversion(new DateTimeOffsetToBinaryConverter());
            //    }

            //    var properties2 = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal)
            //                                                                || p.PropertyType == typeof(decimal?));
            //    foreach (var property in properties2)
            //    {
            //        modelBuilder
            //            .Entity(entityType.Name)
            //            .Property(property.Name)
            //            .HasConversion<double>();
            //    }
            //}
        }
    }
}