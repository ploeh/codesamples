using System;

namespace Ploeh.Samples.Commerce.Domain
{
    public class ProductInventory
    {
        public ProductInventory(Guid id) : this(id, 0)
        {
        }

        public ProductInventory(Guid id, int quantity)
        {
            Id = id;
            Quantity = quantity;
        }

        public Guid Id { get; }
        public int Quantity { get; }

        public ProductInventory WithQuantity(int newQuantity)
        {
            return new ProductInventory(Id, newQuantity);
        }

        public ProductInventory AdjustQuantity(int adjustment)
        {
            return WithQuantity(Quantity + adjustment);
        }

        public override bool Equals(object obj)
        {
            return obj is ProductInventory inventory &&
                   Id.Equals(inventory.Id) &&
                   Quantity == inventory.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Quantity);
        }
    }
}