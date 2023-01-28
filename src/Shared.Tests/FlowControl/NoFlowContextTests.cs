using Castle.Core.Logging;
using CommunAxiom.Commons.Shared.FlowControl;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    public class NoFlowContextTests
    {
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        SegregatedContext<Contract> _noFlowContext;
        AsyncLocal<Contract> _contract = new AsyncLocal<Contract>();
        
        public NoFlowContextTests() 
        {
        }

        [SetUp]
        public void SetupTest()
        {
            _noFlowContext = new SegregatedContext<Contract>(SetupState, _cancellationTokenSource.Token, Setup.ServiceProvider.GetService<ILogger<SegregatedContext<Contract>>>());
        }

        [TearDown]
        public void Teardown()
        {
            _noFlowContext.Dispose();
        }

        Task<Contract> SetupState()
        {
            return Task.FromResult(new Contract
            {
                Value = "StateValue"
            });
        }

        [Test]
        public async Task TestCancelledFlow()
        {
            _contract.Value = new Contract()
            {
                Value = "TestA"
            };
            
            var testRes = await _noFlowContext.Run<string?>((state) =>
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

            var myClosure = "MyClosure";
            var testRes = await _noFlowContext.Run<string?>((state) =>
            {
                return Task.FromResult<string?>(myClosure);
            });
            
            testRes.Should().BeEquivalentTo(myClosure);
        }

        [Test]
        public async Task TestFlowSeperation()
        {
            var c = new AsyncLocal<Contract>();
            c.Value = new Contract
            {
                Value = "This is local"
            };
            var nfc = new SegregatedContext<Contract>(() =>
            {
                c.Value = new Contract()
                {
                    Value = "This is not local"
                };
                return Task.FromResult(c.Value);
            }, _cancellationTokenSource.Token, Setup.ServiceProvider.GetService<ILogger<SegregatedContext<Contract>>>());

            var res = await nfc.Run(state =>
            {
                state.Value += "!";
                return Task.FromResult(state.Value);
            });

            res.Should().BeEquivalentTo("This is not local!");
            c.Value.Value.Should().BeEquivalentTo("This is local");

            res = await nfc.Run(state =>
            {
                return Task.FromResult(c.Value.Value);
            });

            res.Should().BeEquivalentTo("This is not local!");

            nfc.Dispose();
        }
    }

}
