﻿using System;
using System.Threading.Tasks;
using Xunit;

namespace Hashgraph.Test.Fixtures
{
    public class TestAccountInstance : IAsyncDisposable
    {
        public ReadOnlyMemory<byte> PublicKey;
        public ReadOnlyMemory<byte> PrivateKey;
        public CreateAccountParams CreateAccountParams;
        public AccountRecord AccountRecord;
        public NetworkCredentialsFixture NetworkCredentials;
        public Client Client;

        public static async Task<TestAccountInstance> CreateAsync(NetworkCredentialsFixture networkCredentials)
        {
            var fx = new TestAccountInstance();
            fx.NetworkCredentials = networkCredentials;
            fx.NetworkCredentials.TestOutput?.WriteLine("STARTING SETUP: Test Account Instance");
            (fx.PublicKey, fx.PrivateKey) = Generator.KeyPair();
            fx.CreateAccountParams = new CreateAccountParams
            {
                PublicKey = fx.PublicKey,
                InitialBalance = (ulong)Generator.Integer(10, 20)
            };
            fx.Client = networkCredentials.CreateClientWithDefaultConfiguration();
            fx.AccountRecord = await fx.Client.CreateAccountWithRecordAsync(fx.CreateAccountParams, ctx =>
             {
                 ctx.Memo = "Test Account Instance: Creating Test Account on Network";
             });
            Assert.Equal(ResponseCode.Success, fx.AccountRecord.Status);
            networkCredentials.TestOutput?.WriteLine("SETUP COMPLETED: Test Account Instance");
            return fx;
        }

        public async ValueTask DisposeAsync()
        {
            NetworkCredentials.TestOutput?.WriteLine("STARTING TEARDOWN: Test Account Instance");
            try
            {
                await Client.DeleteAccountAsync(new Account(AccountRecord.Address, PrivateKey), NetworkCredentials.CreateDefaultAccount(), ctx =>
                  {
                      ctx.Memo = "Test Account Instance Teardown: Attempting to delete Account from Network (if exists)";
                  });
            }
            catch
            {
                //noop
            }
            await Client.DisposeAsync();
            NetworkCredentials.TestOutput?.WriteLine("TEARDOWN COMPLETED: Test Account Instance");
        }
    }
}