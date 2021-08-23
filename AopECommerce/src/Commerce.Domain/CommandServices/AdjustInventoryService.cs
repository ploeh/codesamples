using System;
using Ploeh.Samples.Commerce.Domain.Commands;
using Ploeh.Samples.Commerce.Domain.Events;

namespace Ploeh.Samples.Commerce.Domain.CommandServices
{
    public class AdjustInventoryService : ICommandHandler<AdjustInventory>
    {
        private readonly IInventoryRepository repository;

        public AdjustInventoryService(IInventoryRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            this.repository = repository;
        }

        public void Execute(AdjustInventory command)
        {
            var productInventory = this.repository.GetByIdOrNull(command.ProductId)
                ?? new ProductInventory(command.ProductId);

            int quantityAdjustment = command.Quantity * (command.Decrease ? -1 : 1);
            productInventory = productInventory.AdjustQuantity(quantityAdjustment);

            if (productInventory.Quantity < 0)
                throw new InvalidOperationException("Can't decrease below 0.");

            this.repository.Save(productInventory);
        }
    }
}