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
        // Tests missing? Send us a pull request.

        [Fact]
        public void CreateWithNullRepositoryWillThrow()
        {
            // Act
            Action action = () => new AdjustInventoryService(
                repository: null,
                handler: new StubEventHandler<InventoryAdjusted>());

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void CreateWithNullHandlerWillThrow()
        {
            // Act
            Action action = () => new AdjustInventoryService(
                repository: new InMemoryInventoryRepository(),
                handler: null);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Theory]
        [InlineData(05)]
        [InlineData(10)]
        public void IncreaseInventory(int quantity)
        {
            // Arrange
            Guid productId = Guid.NewGuid();
            var command = new AdjustInventory { ProductId = productId, Decrease = false, Quantity = quantity };
            var expectedEvent = new { ProductId = productId, QuantityAdjustment = quantity };

            var repository = new InMemoryInventoryRepository();
            var handler = new SpyEventHandler<InventoryAdjusted>();

            var sut = new AdjustInventoryService(repository, handler);

            // Act
            sut.Execute(command);

            // Assert
            Assert.Equal(
                expected: expectedEvent,
                actual: new { handler.HandledEvent.ProductId, handler.HandledEvent.QuantityAdjustment });
            var actualInventory = repository.GetByIdOrNull(productId);
            var expected = new ProductInventory(productId, quantity);
            Assert.Equal(expected, actualInventory);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        public void DecreaseInventory(int quantity)
        {
            // Arrange
            Guid productId = Guid.NewGuid();
            var command = new AdjustInventory { ProductId = productId, Decrease = true, Quantity = quantity };
            var expectedEvent = new { ProductId = productId, QuantityAdjustment = -quantity };

            var repository = new InMemoryInventoryRepository();
            var handler = new SpyEventHandler<InventoryAdjusted>();

            var sut = new AdjustInventoryService(repository, handler);

            repository.Save(new ProductInventory(productId, quantity: 20));

            // Act
            sut.Execute(command);

            // Assert
            Assert.Equal(
                expected: expectedEvent,
                actual: new { handler.HandledEvent.ProductId, handler.HandledEvent.QuantityAdjustment });
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
                repository,
                new SpyEventHandler<InventoryAdjusted>());

            Assert.Throws<InvalidOperationException>(() => sut.Execute(command));
        }
    }
}