using System;
using Ploeh.Samples.Commerce.Domain.Commands;
using Ploeh.Samples.Commerce.Domain.Events;

namespace Ploeh.Samples.Commerce.Domain.CommandServices
{
    public class AdjustInventoryService : ICommandService<AdjustInventory>
    {
        private readonly IInventoryRepository repository;
        private readonly IEventHandler<InventoryAdjusted> handler;

        public AdjustInventoryService(
            IInventoryRepository repository, IEventHandler<InventoryAdjusted> handler)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            this.repository = repository;
            this.handler = handler;
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

            this.handler.Handle(new InventoryAdjusted(command.ProductId, quantityAdjustment));
        }
    }
}