﻿using NSec.Cryptography;
using System;

namespace Hashgraph.Implementation
{
    /// <summary>
    /// Internal helper class providing validation checks for 
    /// various methods, throwing invalid operation and argument 
    /// exceptions when required information is missing from the 
    /// context or parameter arguments.
    /// </summary>
    internal static class RequireInputParameter
    {
        internal static Address Address(Address address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address), "Account Address is is missing. Please check that it is not null.");
            }
            return address;
        }
        internal static Account AccountToDelete(Account accountToDelete)
        {
            if (accountToDelete is null)
            {
                throw new ArgumentNullException(nameof(accountToDelete), "Account to Delete is missing. Please check that it is not null.");
            }
            return accountToDelete;
        }

        internal static Address File(Address file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file), "File is missing. Please check that it is not null.");
            }
            return file;
        }
        internal static Address FileToDelete(Address fileToDelete)
        {
            if (fileToDelete is null)
            {
                throw new ArgumentNullException(nameof(fileToDelete), "File to Delete is missing. Please check that it is not null.");
            }
            return fileToDelete;
        }
        internal static Address TransferToAddress(Address transferToAddress)
        {
            if (transferToAddress is null)
            {
                throw new ArgumentNullException(nameof(transferToAddress), "Transfer address is is missing. Please check that it is not null.");
            }
            return transferToAddress;
        }
        internal static Account FromAccount(Account fromAccount)
        {
            if (fromAccount is null)
            {
                throw new ArgumentNullException(nameof(fromAccount), "Account to transfer from is is missing. Please check that it is not null.");
            }
            return fromAccount;
        }
        internal static Address ToAddress(Address toAddress)
        {
            if (toAddress is null)
            {
                throw new ArgumentNullException(nameof(toAddress), "Account to transfer to is is missing. Please check that it is not null.");
            }
            return toAddress;
        }
        internal static long Amount(long amount)
        {
            if (amount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "The amount to transfer must be non-negative.");
            }
            return amount;
        }
        internal static ReadOnlyMemory<byte> Hash(ReadOnlyMemory<byte> hash)
        {
            if (hash.IsEmpty)
            {
                throw new ArgumentNullException(nameof(hash), "The claim hash is missing. Please check that it is not null.");
            }
            if (hash.Length != 48)
            {
                throw new ArgumentOutOfRangeException(nameof(hash), "The claim hash is expected to be 48 bytes in length.");
            }
            return hash;
        }
        internal static UpdateAccountParams UpdateParameters(UpdateAccountParams updateParameters)
        {
            if (updateParameters is null)
            {
                throw new ArgumentNullException(nameof(updateParameters), "Account Update Parameters argument is missing. Please check that it is not null.");
            }
            if (updateParameters.Account is null)
            {
                throw new ArgumentNullException(nameof(updateParameters.Account), "Account is is missing. Please check that it is not null.");
            }
            if (updateParameters.Endorsement is null &&
                updateParameters.SendThresholdCreateRecord is null &&
                updateParameters.ReceiveThresholdCreateRecord is null &&
                updateParameters.Expiration is null &&
                updateParameters.AutoRenewPeriod is null)
            {
                throw new ArgumentException(nameof(updateParameters), "The Account Updates contains no update properties, it is blank.");
            }
            return updateParameters;
        }
        internal static long AcountNumber(long accountNum)
        {
            if (accountNum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountNum), "Account Number cannot be negative.");
            }
            return accountNum;
        }
        internal static long ShardNumber(long shardNum)
        {
            if (shardNum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shardNum), "Shard Number cannot be negative.");
            }
            return shardNum;
        }
        internal static long RealmNumber(long realmNum)
        {
            if (realmNum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(realmNum), "Realm Number cannot be negative.");
            }
            return realmNum;
        }
        internal static string Url(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentOutOfRangeException(nameof(url), "URL is required.");
            }
            return url;
        }
        internal static PublicKey[] PublicKeys(ReadOnlyMemory<byte>[] publicKeys)
        {
            if (publicKeys.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(publicKeys), "At least one public key is required.");
            }
            var result = new PublicKey[publicKeys.Length];
            for (int i = 0; i < publicKeys.Length; i++)
            {
                try
                {
                    result[i] = Keys.ImportPublicEd25519KeyFromBytes(publicKeys[i]);
                }
                catch (Exception ex)
                {
                    throw new ArgumentOutOfRangeException(nameof(publicKeys), ex.Message);
                }
            }
            return result;
        }
        internal static Key[] PrivateKeys(ReadOnlyMemory<byte>[] privateKeys)
        {
            if (privateKeys.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(privateKeys), "At least one private key is required.");
            }
            var result = new Key[privateKeys.Length];
            for (int i = 0; i < privateKeys.Length; i++)
            {
                try
                {
                    result[i] = Keys.ImportPrivateEd25519KeyFromBytes(privateKeys[i]);
                }
                catch (Exception ex)
                {
                    throw new ArgumentOutOfRangeException(nameof(privateKeys), ex.Message);
                }
            }
            return result;
        }
        internal static Endorsement[] Endorsements(Endorsement[] endorsements)
        {
            if (endorsements is null)
            {
                throw new ArgumentNullException(nameof(endorsements), "The list of endorsements may not be null.");
            }
            else if (endorsements.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(endorsements), "At least one endorsement in a list is required.");
            }
            for (int i = 0; i < endorsements.Length; i++)
            {
                if (endorsements[i] is null)
                {
                    throw new ArgumentNullException(nameof(endorsements), "No endorsement within the list may be null.");
                }
            }
            return endorsements;
        }
        internal static uint RequiredCount(uint requiredCount, int maxCount)
        {
            if (requiredCount > maxCount)
            {
                throw new ArgumentOutOfRangeException(nameof(requiredCount), "The required number of keys for a valid signature cannot exceed the number of public keys provided.");
            }
            return requiredCount;
        }
        internal static CreateAccountParams CreateParameters(CreateAccountParams createParameters)
        {
            if (createParameters is null)
            {
                throw new ArgumentNullException(nameof(createParameters), "The create parameters are is missing. Please check that the argument is not null.");
            }
            if (createParameters.PublicKey.IsEmpty)
            {
                throw new ArgumentOutOfRangeException(nameof(createParameters.PublicKey), "The public key is required.");
            }
            try
            {
                Keys.ImportPublicEd25519KeyFromBytes(createParameters.PublicKey);
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(nameof(createParameters.PublicKey), ex.Message);
            }
            return createParameters;
        }
        internal static CreateFileParams CreateParameters(CreateFileParams createParameters)
        {
            if (createParameters is null)
            {
                throw new ArgumentNullException(nameof(createParameters), "The create parameters are is missing. Please check that the argument is not null.");
            }
            if (createParameters.Endorsements is null)
            {
                throw new ArgumentOutOfRangeException(nameof(createParameters.Endorsements), "Endorsements are required.");
            }
            return createParameters;
        }
        internal static CreateContractParams CreateParameters(CreateContractParams createParameters)
        {
            if (createParameters is null)
            {
                throw new ArgumentNullException(nameof(createParameters), "The create parameters are is missing. Please check that the argument is not null.");
            }
            if(createParameters.File is null)
            {
                throw new ArgumentNullException(nameof(createParameters.File), "The File Address containing the contract is missing, it cannot be null.");
            }
            return createParameters;
        }
        internal static UpdateFileParams UpdateParameters(UpdateFileParams updateParameters)
        {
            if (updateParameters is null)
            {
                throw new ArgumentNullException(nameof(updateParameters), "File Update Parameters argument is missing. Please check that it is not null.");
            }
            if (updateParameters.File is null)
            {
                throw new ArgumentNullException(nameof(updateParameters.File), "File identifier is is missing. Please check that it is not null.");
            }
            if (updateParameters.Endorsements is null &&
                updateParameters.Contents is null)
            {
                throw new ArgumentException(nameof(updateParameters), "The File Update parameters contain no update properties, it is blank.");
            }
            return updateParameters;
        }
        internal static AppendFileParams AppendParameters(AppendFileParams appendParameters)
        {
            if (appendParameters is null)
            {
                throw new ArgumentNullException(nameof(appendParameters), "File Update Parameters argument is missing. Please check that it is not null.");
            }
            if (appendParameters.File is null)
            {
                throw new ArgumentNullException(nameof(appendParameters.File), "File identifier is is missing. Please check that it is not null.");
            }
            return appendParameters;
        }
        internal static Claim AddParameters(Claim addParameters)
        {
            if (addParameters is null)
            {
                throw new ArgumentNullException(nameof(addParameters), "Add Claim Parameters argument is missing. Please check that it is not null.");
            }
            if (addParameters.Address is null)
            {
                throw new ArgumentNullException(nameof(addParameters.Address), "The address to attach the claim to is is missing. Please check that it is not null.");
            }
            if (addParameters.Hash.IsEmpty)
            {
                throw new ArgumentNullException(nameof(addParameters.Hash), "The claim hash is missing. Please check that it is not null.");
            }
            if (addParameters.Hash.Length != 48)
            {
                throw new ArgumentOutOfRangeException(nameof(addParameters.Hash), "The claim hash is expected to be 48 bytes in length.");
            }
            if (addParameters.Endorsements is null)
            {
                throw new ArgumentNullException(nameof(addParameters.Endorsements), "The endorsements property is missing. Please check that it is not null.");
            }
            if (addParameters.Endorsements.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(addParameters.Endorsements), "The endorsements array is empty. Please must include at least one endorsement.");
            }
            if(addParameters.ClaimDuration.Ticks == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(addParameters.ClaimDuration), "Claim Duration must have some length.");
            }
            return addParameters;
        }
    }
}
