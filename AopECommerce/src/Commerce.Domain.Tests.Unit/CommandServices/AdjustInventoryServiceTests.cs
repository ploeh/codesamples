using System;
using Ploeh.Samples.Commerce.Domain.Commands;
using Ploeh.Samples.Commerce.Domain.CommandServices;
using Ploeh.Samples.Commerce.Domain.Events;
using Ploeh.Samples.Commerce.Domain.Tests.Unit.Fakes;
using Xunit;

namespace Ploeh.Samples.Commerce.Domain.Tests.Unit.CommandServices
{
    public class AdjustInventoryServiceTests
    {
        [Fact]
        public void CreateWithNullRepositoryWillThrow()
        {
            Action action = () => new AdjustInventoryService(
                repository: null);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Theory]
        [InlineData(05)]
        [InlineData(10)]
        public void IncreaseInventory(int quantity)
        {
            Guid productId = Guid.NewGuid();
            var command = new AdjustInventory
            {
                ProductId = productId,
                Decrease = false,
                Quantity = quantity
            };
            var repository = new InMemoryInventoryRepository();
            var sut = new AdjustInventoryService(repository);

            sut.Execute(command);

            var actualInventory = repository.GetByIdOrNull(productId);
            var expected = new ProductInventory(productId, quantity);
            Assert.Equal(expected, actualInventory);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        public void DecreaseInventory(int quantity)
        {
            Guid productId = Guid.NewGuid();
            var command = new AdjustInventory
            {
                ProductId = productId,
                Decrease = true,
                Quantity = quantity
            };
            var repository = new InMemoryInventoryRepository();
            var sut = new AdjustInventoryService(repository);
            repository.Save(new ProductInventory(productId, quantity: 20));

            sut.Execute(command);

            var actualInventory = repository.GetByIdOrNull(productId);
            var expected = new ProductInventory(productId, 20 - quantity);
            Assert.Equal(expected, actualInventory);
        }

        [Fact]
        public void DecreaseTooMuch()
        {
            var productId = Guid.NewGuid();
            var command = new AdjustInventory
            {
                ProductId = productId,
                Decrease = true,
                Quantity = 2
            };
            var repository = new InMemoryInventoryRepository();
            repository.Save(new ProductInventory(productId, 1));
            var sut = new AdjustInventoryService(
                repository);

            Assert.Throws<InvalidOperationException>(() => sut.Execute(command));
        }
    }
}