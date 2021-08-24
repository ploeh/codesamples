using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Ploeh.Samples.Commerce.Domain.Tests.Unit
{
    public class CommandHandlerTests
    {
        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("baz")]
        [InlineData("qux")]
        [InlineData("quux")]
        [InlineData("quuz")]
        [InlineData("corge")]
        [InlineData("grault")]
        [InlineData("garply")]
        public void IdentityLaw(string input)
        {
            var observations = new List<string>();
            ICommandHandler<string> sut =
                new DelegatingCommandHandler<string>(observations.Add);

            T id<T>(T x) => x;
            ICommandHandler<string> projection = sut.ContraMap<string, string>(id);

            // Run both handlers
            sut.Execute(input);
            projection.Execute(input);
            Assert.Equal(2, observations.Count);
            Assert.Single(observations.Distinct());
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("baz")]
        [InlineData("qux")]
        [InlineData("quux")]
        [InlineData("quuz")]
        [InlineData("corge")]
        [InlineData("grault")]
        [InlineData("garply")]
        public void CompositionLaw(string input)
        {
            var observations = new List<TimeSpan>();
            ICommandHandler<TimeSpan> sut =
                new DelegatingCommandHandler<TimeSpan>(observations.Add);

            Func<string, int> f = s => s.Length;
            Func<int, TimeSpan> g = i => TimeSpan.FromDays(i);
            ICommandHandler<string> projection1 = sut.ContraMap((string s) => g(f(s)));
            ICommandHandler<string> projection2 = sut.ContraMap(g).ContraMap(f);

            // Run both handlers
            projection1.Execute(input);
            projection2.Execute(input);
            Assert.Equal(2, observations.Count);
            Assert.Single(observations.Distinct());
        }
    }
}
