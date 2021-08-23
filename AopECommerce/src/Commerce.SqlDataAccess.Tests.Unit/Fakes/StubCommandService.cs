using Ploeh.Samples.Commerce.Domain;

namespace Ploeh.Samples.Commerce.SqlDataAccess.Tests.Unit.Fakes
{
    public class StubCommandService<TCommand> : ICommandHandler<TCommand>
    {
        public void Execute(TCommand command)
        {
        }
    }
}