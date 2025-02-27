﻿using Hashgraph.Test.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Hashgraph.Test.Crypto
{
    [Collection(nameof(NetworkCredentials))]
    public class GetAccountBalanceTests
    {
        private readonly NetworkCredentials _network;
        public GetAccountBalanceTests(NetworkCredentials network, ITestOutputHelper output)
        {
            _network = network;
            _network.Output = output;
        }
        [Fact(DisplayName = "Get Account Balance: Can Get Balance for Account")]
        public async Task CanGetTinybarBalanceForAccountAsync()
        {
            await using var client = _network.NewClient();
            var account = _network.Payer;
            var balance = await client.GetAccountBalanceAsync(account);
            Assert.True(balance > 0, "Account Balance should be greater than zero.");
        }
        [Fact(DisplayName = "Get Account Balance: Can Get Balance for Server Node")]
        public async Task CanGetTinybarBalanceForGatewayAsync()
        {
            await using var client = _network.NewClient();
            var account = _network.Gateway;
            var balance = await client.GetAccountBalanceAsync(account);
            Assert.True(balance > 0, "Gateway Account Balance should be greater than zero.");
        }
        [Fact(DisplayName = "Get Account Balance: Missing Gateway Information Throws Exception")]
        public async Task MissingNodeInformationThrowsException()
        {
            var account = _network.Payer;
            await using var client = new Client(ctx => { ctx.Payer = account; });
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var balance = await client.GetAccountBalanceAsync(account);
            });
            Assert.StartsWith("Network Gateway Node has not been configured.", ex.Message);
        }
        [Fact(DisplayName = "Get Account Balance: Missing Payer Account Throws Exception")]
        public async Task MissingPayerAccountThrowsException()
        {
            var account = _network.Payer;
            await using var client = new Client(ctx => { ctx.Gateway = _network.Gateway; });
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var balance = await client.GetAccountBalanceAsync(account);
            });
            Assert.StartsWith("The Payer account has not been configured.", ex.Message);
        }
        [Fact(DisplayName = "Get Account Balance: Missing Account for Balance Check Throws Exception")]
        public async Task MissingBalanceAccountThrowsException()
        {
            await using var client = _network.NewClient();
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var balance = await client.GetAccountBalanceAsync(null);
            });
            Assert.StartsWith("Account Address is missing.", ex.Message);
        }
        [Fact(DisplayName = "Get Account Balance: Invalid Account Address Throws Exception")]
        public async Task InvalidAccountAddressThrowsException()
        {
            await using var client = _network.NewClient();
            var account = new Address(0, 0, 0);
            var ex = await Assert.ThrowsAsync<PrecheckException>(async () =>
            {
                var balance = await client.GetAccountBalanceAsync(account);
            });
            Assert.Equal(ResponseCode.InvalidAccountId, ex.Status);
            Assert.StartsWith("Transaction Failed Pre-Check: InvalidAccount", ex.Message);
        }
        [Fact(DisplayName = "Get Account Balance: Invalid Node Account Throws Exception")]
        public async Task InvalidGatewayAddressThrowsException()
        {
            await using var client = _network.NewClient();
            client.Configure(cfg =>
            {
                cfg.Gateway = new Gateway($"{_network.NetworkAddress}:{_network.NetworkPort}", 0, 0, 0);
            });
            var account = _network.Payer;
            var ex = await Assert.ThrowsAsync<PrecheckException>(async () =>
            {
                var balance = await client.GetAccountBalanceAsync(account);
            });
            Assert.Equal(ResponseCode.InvalidNodeAccount, ex.Status);
            Assert.StartsWith("Transaction Failed Pre-Check: InvalidNodeAccount", ex.Message);
        }
        [Fact(DisplayName = "Get Account Balance: Insufficient Fees Throw Exception")]
        public async Task InsuficientFeesThrowException()
        {
            await using var client = _network.NewClient();
            client.Configure(cfg =>
            {
                cfg.FeeLimit = 0;
            });
            var account = _network.Payer;
            var ex = await Assert.ThrowsAsync<PrecheckException>(async () =>
            {
                var balance = await client.GetAccountBalanceAsync(account);
            });
            Assert.Equal(ResponseCode.InsufficientTxFee, ex.Status);
            Assert.StartsWith("Transaction Failed Pre-Check: InsufficientTxFee", ex.Message);
        }
    }
}
