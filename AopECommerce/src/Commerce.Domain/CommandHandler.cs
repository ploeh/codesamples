using System;
using System.Collections.Generic;
using System.Text;

namespace Ploeh.Samples.Commerce.Domain
{
    public static class CommandHandler
    {
        public static ICommandHandler<T1> ContraMap<T, T1>(
            this ICommandHandler<T> source,
            Func<T1, T> selector)
        {
            Action<T1> action = x => source.Execute(selector(x));
            return new DelegatingCommandHandler<T1>(action);
        }
    }

    public class DelegatingCommandHandler<T> : ICommandHandler<T>
    {
        private readonly Action<T> action;

        public DelegatingCommandHandler(Action<T> action)
        {
            this.action = action;
        }

        public void Execute(T command)
        {
            action(command);
        }
    }
}
