using CommunAxiom.Commons.Shared.FlowControl;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.FlowControl
{
    [TestFixture]
    public class NoFlowTaskTests
    {

        AsyncLocal<Contract> _contract = new AsyncLocal<Contract>();
        
        [Test]
        public async Task TestCancelledFlow()
        {
            _contract.Value = new Contract()
            {
                Value = "TestA"
            };
            var nft = new NoFlowTask<Contract>(() => Task.FromResult(new Contract { Value = _contract.Value?.Value }));

            var testRes = await nft.Run<string?>((state) =>
            {
                if (_contract.Value == null)
                    return Task.FromResult<string?>(null);
                return Task.FromResult(_contract.Value?.Value);
            });
               
            testRes.Should().NotBeEquivalentTo(_contract.Value.Value);
        }

        [Test]
        public async Task TestClosureOnCancelledFlow()
        {
            _contract.Value = new Contract()
            {
                Value = "TestA"
            };

            var testRes = await Task.Run(() =>
            {
                var myClosure = "MyClosure";
                var nft = new NoFlowTask<Contract>(() => Task.FromResult(new Contract { Value = _contract.Value?.Value }));

                var res = nft.Run<string?>((state) =>
                {
                    return Task.FromResult<string?>(myClosure);
                });
                
                return res;
            });
            testRes.Should().BeEquivalentTo("MyClosure");
        }
    }

}
