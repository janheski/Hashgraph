﻿using Grpc.Core;
using Hashgraph.Implementation;
using Proto;
using System;
using System.Threading.Tasks;

namespace Hashgraph
{
    public partial class Client
    {
        /// <summary>
        /// Retrieves the balance in tinybars from the network for a given address.
        /// </summary>
        /// <param name="address">
        /// The hedera network address to retrieve the balance of.
        /// </param>
        /// <param name="configure">
        /// Optional callback method providing an opportunity to modify 
        /// the execution configuration for just this method call. 
        /// It is executed prior to submitting the request to the network.
        /// </param>
        /// <returns>
        /// The balance of the associated address.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If required arguments are missing.</exception>
        /// <exception cref="InvalidOperationException">If required context configuration is missing.</exception>
        /// <exception cref="PrecheckException">If the gateway node create rejected the request upon submission.</exception>
        public async Task<ulong> GetAccountBalanceAsync(Address address, Action<IContext>? configure = null)
        {
            address = RequireInputParameter.Address(address);
            var context = CreateChildContext(configure);
            var gateway = RequireInContext.Gateway(context);
            var payer = RequireInContext.Payer(context);
            var transfers = Transactions.CreateCryptoTransferList((payer, -context.FeeLimit), (gateway, context.FeeLimit));
            var transactionId = Transactions.GetOrCreateTransactionID(context);
            var transactionBody = Transactions.CreateCryptoTransferTransactionBody(context, transfers, transactionId, "Get Account Balance");
            var query = new Query
            {
                CryptogetAccountBalance = new CryptoGetAccountBalanceQuery
                {
                    Header = Transactions.SignQueryHeader(transactionBody, payer),
                    AccountID = Protobuf.ToAccountID(address)
                }
            };
            var response = await Transactions.ExecuteRequestWithRetryAsync(context, query, getRequestMethod, getResponseCode);
            ValidateResult.PreCheck(transactionId, getResponseCode(response));
            return response.CryptogetAccountBalance.Balance;

            static Func<Query, Task<Response>> getRequestMethod(Channel channel)
            {
                var client = new CryptoService.CryptoServiceClient(channel);
                return async (Query query) => (await client.cryptoGetBalanceAsync(query));
            }

            static ResponseCodeEnum getResponseCode(Response response)
            {
                return response.CryptogetAccountBalance?.Header?.NodeTransactionPrecheckCode ?? ResponseCodeEnum.Unknown;
            }
        }
    }
}
