﻿using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Proto;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Hashgraph.Test.Fixtures
{
    public class NetworkCredentials
    {
        private readonly IConfiguration _configuration;
        public string NetworkAddress { get { return _configuration["network:address"]; } }
        public int NetworkPort { get { return getAsInt("network:port"); } }
        public long ServerRealm { get { return getAsInt("server:realm"); } }
        public long ServerShard { get { return getAsInt("server:shard"); } }
        public long ServerNumber { get { return getAsInt("server:number"); } }
        public long AccountRealm { get { return getAsInt("account:realm"); } }
        public long AccountShard { get { return getAsInt("account:shard"); } }
        public long AccountNumber { get { return getAsInt("account:number"); } }
        public ReadOnlyMemory<byte> PrivateKey { get { return Hex.ToBytes(_configuration["account:privateKey"]); } }
        public ReadOnlyMemory<byte> PublicKey { get { return Hex.ToBytes(_configuration["account:publicKey"]); } }
        public Account Payer { get { return new Account(AccountRealm, AccountShard, AccountNumber, PrivateKey); } }
        public Gateway Gateway { get { return new Gateway($"{NetworkAddress}:{NetworkPort}", ServerRealm, ServerShard, ServerNumber); } }
        public ITestOutputHelper Output { get; set; }
        public NetworkCredentials()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .AddUserSecrets<NetworkCredentials>(true)
                .Build();
        }
        public Client NewClient()
        {
            return new Client(ctx =>
            {
                ctx.Gateway = Gateway;
                ctx.Payer = Payer;
                ctx.RetryCount = 50; // Use a high number, sometimes the test network glitches.
                ctx.OnSendingRequest = OutputSendingRequest;
                ctx.OnResponseReceived = OutputReceivResponse;
                ctx.AdjustForLocalClockDrift = true; // Build server has clock drift issues
            });
        }
        public Account PayerWithKeys(params ReadOnlyMemory<byte>[] privateKeys)
        {
            return new Account(AccountRealm, AccountShard, AccountNumber, privateKeys.Append(PrivateKey).ToArray());
        }
        private int getAsInt(string key)
        {
            var valueAsString = _configuration[key];
            if (int.TryParse(valueAsString, out int value))
            {
                return value;
            }
            throw new InvalidProgramException($"Unable to convert configuration '{key}' of '{valueAsString}' into an integer value.");
        }
        private void OutputSendingRequest(IMessage message)
        {
            if (Output != null)
            {
                if (message is Proto.Transaction transaction && transaction.BodyBytes != null)
                {
                    var transactionBody = Proto.TransactionBody.Parser.ParseFrom(transaction.BodyBytes);
                    Output.WriteLine($"{DateTime.UtcNow}  TX BODY  {JsonFormatter.Default.Format(transactionBody)}");
                    Output.WriteLine($"{DateTime.UtcNow}  └─ SIG → {JsonFormatter.Default.Format(message)}");
                }
                else if (message is Proto.Query query && TryGetQueryTransaction(query, out Proto.Transaction payment) && payment.BodyBytes != null)
                {
                    var transactionBody = Proto.TransactionBody.Parser.ParseFrom(payment.BodyBytes);
                    Output.WriteLine($"{DateTime.UtcNow}  QX PYMT  {JsonFormatter.Default.Format(transactionBody)}");
                    Output.WriteLine($"{DateTime.UtcNow}  └─ QRY → {JsonFormatter.Default.Format(message)}");
                }
                else
                {
                    Output.WriteLine($"{DateTime.UtcNow}  TX     → {JsonFormatter.Default.Format(message)}");
                }
            }
        }
        private void OutputReceivResponse(int tryNo, IMessage message)
        {
            Output?.WriteLine($"{DateTime.UtcNow}  RX:({tryNo:00})  {JsonFormatter.Default.Format(message)}");
        }
        private bool TryGetQueryTransaction(Query query, out Transaction payment)
        {
            payment = null;
            switch (query.QueryCase)
            {
                case Query.QueryOneofCase.GetByKey:
                    payment = query.GetByKey?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.GetBySolidityID:
                    payment = query.GetBySolidityID?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.ContractCallLocal:
                    payment = query.ContractCallLocal?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.ContractGetInfo:
                    payment = query.ContractGetInfo?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.ContractGetBytecode:
                    payment = query.ContractGetBytecode?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.ContractGetRecords:
                    payment = query.ContractGetRecords?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.CryptogetAccountBalance:
                    payment = query.CryptogetAccountBalance?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.CryptoGetAccountRecords:
                    payment = query.CryptoGetAccountRecords?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.CryptoGetInfo:
                    payment = query.CryptoGetInfo?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.CryptoGetClaim:
                    payment = query.CryptoGetClaim?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.CryptoGetProxyStakers:
                    payment = query.CryptoGetProxyStakers?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.FileGetContents:
                    payment = query.FileGetContents?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.FileGetInfo:
                    payment = query.FileGetInfo?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.TransactionGetReceipt:
                    payment = query.TransactionGetReceipt?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.TransactionGetRecord:
                    payment = query.TransactionGetRecord?.Header?.Payment;
                    break;
                case Query.QueryOneofCase.TransactionGetFastRecord:
                    payment = query.TransactionGetFastRecord?.Header?.Payment;
                    break;
            }
            return payment != null;
        }
        [CollectionDefinition(nameof(NetworkCredentials))]
        public class FixtureCollection : ICollectionFixture<NetworkCredentials>
        {
        }
    }
}