namespace Ploeh.Samples.Commerce.Domain
{
    // ---- Code Listing 10.12 ----
    public interface ICommandHandler<TCommand>
    {
        void Execute(TCommand command);
    }
}