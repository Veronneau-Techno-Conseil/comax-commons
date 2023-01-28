using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.FlowControl
{
    [TestFixture]
    public class ThreadIsolationTest
    {

        AsyncLocal<Contract> _contract = new AsyncLocal<Contract>();

        [Test]
        public async Task TestFlowStraight()
        {
            _contract.Value = new Contract()
            {
                Value = "TestA"
            };
            // No change, straightforward
            var testRes = await Task.Run(async () =>
            {
                var res = await Task.Run(() =>
                {
                    return _contract.Value.Value;
                });
                return res;
            });

            testRes.Should().BeEquivalentTo(_contract.Value.Value);
        }

        [Test]
        public async Task TestFlowModify()
        {
            _contract.Value = new Contract()
            {
                Value = "TestA"
            };
            // Changed, should still flow
            var testRes = await Task.Run(async () =>
            {
                var res = await Task.Run(() =>
                {
                    _contract.Value.Value = "TestB";
                    return _contract.Value.Value;
                });
                return res;
            });
            testRes.Should().BeEquivalentTo(_contract.Value.Value);
        }

        [Test]
        public async Task TestCancelledFlow()
        {
            _contract.Value = new Contract()
            {
                Value = "TestA"
            };
            
            var testRes = await Task.Run(() =>
            {
                var afc = ExecutionContext.SuppressFlow();
                var res = Task.Run(() =>
                {
                    if (_contract.Value == null)
                        return null;
                    return _contract.Value.Value;
                }).ConfigureAwait(true).GetAwaiter().GetResult();
                afc.Undo();
                return res;
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
                var afc = ExecutionContext.SuppressFlow();
                var res = Task.Run(() =>
                {
                    return myClosure;
                }).ConfigureAwait(true).GetAwaiter().GetResult();
                afc.Undo();
                return res;
            });
            testRes.Should().BeEquivalentTo("MyClosure");
        }
    }

    public class Contract
    {
        public string Value { get; set; }
    }
}
