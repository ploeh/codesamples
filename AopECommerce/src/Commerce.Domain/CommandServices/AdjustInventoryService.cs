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

            productInventory = productInventory.Handle(command);

            this.repository.Save(productInventory);
        }
    }
}