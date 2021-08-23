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

        [Fact]
        public void IncreaseInventory()
        {
            // Arrange
            Guid productId = Guid.NewGuid();
            var command = new AdjustInventory { ProductId = productId, Decrease = false, Quantity = 10 };
            var expectedEvent = new { ProductId = productId, QuantityAdjustment = 10 };

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
            Assert.NotNull(actualInventory);
        }

        [Fact]
        public void DecreaseInventory()
        {
            // Arrange
            Guid productId = Guid.NewGuid();
            var command = new AdjustInventory { ProductId = productId, Decrease = true, Quantity = 5 };
            var expectedEvent = new { ProductId = productId, QuantityAdjustment = -5 };

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
            Assert.NotNull(actualInventory);
        }
    }
}