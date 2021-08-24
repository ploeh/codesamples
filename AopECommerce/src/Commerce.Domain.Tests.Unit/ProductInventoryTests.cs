using Ploeh.Samples.Commerce.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ploeh.Samples.Commerce.Domain.Tests.Unit
{
    public class ProductInventoryTests
    {
        [Theory]
        [InlineData(0, false, 0, 0)]
        [InlineData(0,  true, 0, 0)]
        [InlineData(0, false, 1, 1)]
        [InlineData(0, false, 2, 2)]
        [InlineData(1, false, 1, 2)]
        [InlineData(2, false, 3, 5)]
        [InlineData(5,  true, 2, 3)]
        [InlineData(5,  true, 5, 0)]
        public void Handle(
            int initial,
            bool decrease,
            int adjustment,
            int expected)
        {
            var sut = new ProductInventory(Guid.NewGuid(), initial);

            var command = new AdjustInventory
            {
                ProductId = sut.Id,
                Decrease = decrease,
                Quantity = adjustment
            };
            var actual = sut.Handle(command);

            Assert.Equal(sut.WithQuantity(expected), actual);
        }

        [Theory]
        [InlineData( -1)]
        [InlineData( -2)]
        [InlineData(-19)]
        public void SetNegativeQuantity(int negative)
        {
            var id = Guid.NewGuid();
            Action action = () => new ProductInventory(id, negative);
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }
    }
}
