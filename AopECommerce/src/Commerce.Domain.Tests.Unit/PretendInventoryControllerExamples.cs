using Ploeh.Samples.Commerce.Domain.Commands;
using Ploeh.Samples.Commerce.Domain.Tests.Unit.Fakes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ploeh.Samples.Commerce.Domain.Tests.Unit
{
    public class PretendInventoryControllerExamples
    {
        private class PretendInventoryController
        {
            private readonly IInventoryRepository repository;

            public PretendInventoryController(IInventoryRepository repository)
            {
                this.repository = repository;
            }

            public void Adjust(AdjustInventory command)
            {
                var inventoryAdjuster =
                    new DelegatingCommandHandler<ProductInventory>(repository.Save)
                        .ContraMap((ProductInventory inv) =>
                            (inv ?? new ProductInventory(command.ProductId)).Handle(command))
                        .ContraMap((AdjustInventory cmd) =>
                            repository.GetByIdOrNull(cmd.ProductId));
                inventoryAdjuster.Execute(command);
            }
        }

        [Theory]
        [InlineData(0, false, 0, 0)]
        [InlineData(0,  true, 0, 0)]
        [InlineData(0, false, 1, 1)]
        [InlineData(0, false, 2, 2)]
        [InlineData(1, false, 1, 2)]
        [InlineData(2, false, 3, 5)]
        [InlineData(5,  true, 2, 3)]
        [InlineData(5,  true, 5, 0)]
        public void SuccessfullyAdjust(
            int initial,
            bool decrease,
            int adjustment,
            int expected)
        {
            var id = Guid.NewGuid();
            var repository = new InMemoryInventoryRepository();
            repository.Save(new ProductInventory(id, initial));
            var sut = new PretendInventoryController(repository);

            var command = new AdjustInventory
            {
                ProductId = id,
                Decrease = decrease,
                Quantity = adjustment
            };
            sut.Adjust(command);

            var actual = repository.GetByIdOrNull(id);
            Assert.Equal(new ProductInventory(id, expected), actual);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        public void AttemptToDecreaseToNegativeInventory(
            int initial,
            int adjustment)
        {
            var id = Guid.NewGuid();
            var repository = new InMemoryInventoryRepository();
            repository.Save(new ProductInventory(id, initial));
            var sut = new PretendInventoryController(repository);

            var command = new AdjustInventory
            {
                ProductId = id,
                Decrease = true,
                Quantity = adjustment
            };
            Action action = () => sut.Adjust(command);

            Assert.Throws<ArgumentOutOfRangeException>(action);
        }
    }
}
