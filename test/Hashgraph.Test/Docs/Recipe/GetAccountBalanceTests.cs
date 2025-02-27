﻿using Hashgraph.Test.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Hashgraph.Tests.Docs.Recipe
{
    [Collection(nameof(NetworkCredentials))]
    public class GetAccountBalanceTests
    {
        // Code Example:  Docs / Recipe / Get Account Balance
        static async Task Main(string[] args)
        {                                                 // For Example:
            var gatewayUrl = args[0];                     //   2.testnet.hedera.com:50211
            var gatewayAccountNo = long.Parse(args[1]);   //   5 (gateway node 0.0.5)
            var payerAccountNo = long.Parse(args[2]);     //   20 (account 0.0.20)
            var payerPrivateKey = Hex.ToBytes(args[3]);   //   302e0201... (48 byte Ed25519 private in hex)
            var queryAccountNo = long.Parse(args[4]);     //   2300 (account 0.0.2300)
            try
            {
                await using var client = new Client(ctx =>
                {
                    ctx.Gateway = new Gateway(gatewayUrl, 0, 0, gatewayAccountNo);
                    ctx.Payer = new Account(0, 0, payerAccountNo, payerPrivateKey);
                });
                var account = new Address(0, 0, queryAccountNo);
                var balance = await client.GetAccountBalanceAsync(account);
                Console.WriteLine($"Account Balance for {account.AccountNum} is {balance} tinybars.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
            }
        }

        private readonly NetworkCredentials _network;
        public GetAccountBalanceTests(NetworkCredentials network, ITestOutputHelper output)
        {
            _network = network;
            _network.Output = output;
        }

        [Fact(DisplayName = "Docs Recipe Example: Get Account Balance")]
        public async Task RunTest()
        {
            using (new ConsoleRedirector(_network.Output))
            {
                var arg0 = _network.Gateway.Url;
                var arg1 = _network.Gateway.AccountNum.ToString();
                var arg2 = _network.Payer.AccountNum.ToString();
                var arg3 = Hex.FromBytes(_network.PrivateKey);
                var arg4 = _network.Payer.AccountNum.ToString();
                await Main(new string[] { arg0, arg1, arg2, arg3, arg4 });
            }
        }
    }
}
