﻿using Hashgraph.Test.Fixtures;
using System.Threading.Tasks;
using Xunit;

namespace Hashgraph.Test.Crypto
{
    [Collection(nameof(NetworkCredentialsFixture))]
    public class TransferTests
    {
        private readonly NetworkCredentialsFixture _networkCredentials;
        public TransferTests(NetworkCredentialsFixture networkCredentials)
        {
            _networkCredentials = networkCredentials;
        }
        [Fact(DisplayName = "Transfer Tests: Can Send to Gateway Node")]
        public async Task CanTransferCryptoToGatewayNode()
        {
            long fee = 0;
            long transferAmount = 10;
            var client = _networkCredentials.CreateClientWithDefaultConfiguration();
            client.Configure(ctx => fee = ctx.Fee);
            var fromAccount = _networkCredentials.CreateDefaultAccount();
            var toAddress = _networkCredentials.CreateDefaultGateway();            
            var balanceBefore = await client.GetAccountBalanceAsync(fromAccount);
            var receipt = await client.TransferAsync(fromAccount, toAddress, transferAmount);
            var balanceAfter = await client.GetAccountBalanceAsync(fromAccount);
            Assert.Equal((ulong)transferAmount + (ulong)fee + (ulong)fee, balanceBefore - balanceAfter);
        }
    }
}
