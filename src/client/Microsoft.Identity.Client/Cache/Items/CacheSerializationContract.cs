﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.Identity.Client.Cache.Items
{
    internal class CacheSerializationContract
    {
        private static readonly IEnumerable<string> s_knownPropertyNames = new[] {
                StorageJsonValues.CredentialTypeAccessToken,
                StorageJsonValues.CredentialTypeRefreshToken,
                StorageJsonValues.CredentialTypeIdToken,
                StorageJsonValues.AccountRootKey,
                StorageJsonValues.AppMetadata};

        public Dictionary<string, MsalAccessTokenCacheItem> AccessTokens { get; set; } =
            new Dictionary<string, MsalAccessTokenCacheItem>();

        public Dictionary<string, MsalRefreshTokenCacheItem> RefreshTokens { get; set; } =
            new Dictionary<string, MsalRefreshTokenCacheItem>();

        public Dictionary<string, MsalIdTokenCacheItem> IdTokens { get; set; } =
            new Dictionary<string, MsalIdTokenCacheItem>();

        public Dictionary<string, MsalAccountCacheItem> Accounts { get; set; } =
            new Dictionary<string, MsalAccountCacheItem>();

        public Dictionary<string, MsalAppMetadataCacheItem> AppMetadata { get; set; } =
            new Dictionary<string, MsalAppMetadataCacheItem>();

        public IDictionary<string, JsonNode> UnknownNodes { get; }

        public CacheSerializationContract(IDictionary<string, JsonNode> unknownNodes)
        {
            UnknownNodes = unknownNodes ?? new Dictionary<string, JsonNode>();
        }

        internal static CacheSerializationContract FromJsonString(string json)
        {
            var root = JsonNode.Parse(json, documentOptions: new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            }).AsObject();
            var unknownNodes = ExtractUnknownNodes(root);

            var contract = new CacheSerializationContract(unknownNodes);

            // Access Tokens
            if (root.ContainsKey(StorageJsonValues.CredentialTypeAccessToken))
            {
                foreach (var token in root[StorageJsonValues.CredentialTypeAccessToken].AsObject())
                {
                    if (token.Value is JsonObject j)
                    {
                        var item = MsalAccessTokenCacheItem.FromJObject(j);
                        contract.AccessTokens[item.GetKey().ToString()] = item;
                    }
                }
            }

            // Refresh Tokens
            if (root.ContainsKey(StorageJsonValues.CredentialTypeRefreshToken))
            {
                foreach (var token in root[StorageJsonValues.CredentialTypeRefreshToken].AsObject())
                {
                    if (token.Value is JsonObject j)
                    {
                        var item = MsalRefreshTokenCacheItem.FromJObject(j);
                        contract.RefreshTokens[item.GetKey().ToString()] = item;
                    }
                }
            }

            // Id Tokens
            if (root.ContainsKey(StorageJsonValues.CredentialTypeIdToken))
            {
                foreach (var token in root[StorageJsonValues.CredentialTypeIdToken].AsObject())
                {
                    if (token.Value is JsonObject j)
                    {
                        var item = MsalIdTokenCacheItem.FromJObject(j);
                        contract.IdTokens[item.GetKey().ToString()] = item;
                    }
                }
            }

            // Accounts
            if (root.ContainsKey(StorageJsonValues.AccountRootKey))
            {
                foreach (var token in root[StorageJsonValues.AccountRootKey].AsObject())
                {
                    if (token.Value is JsonObject j)
                    {
                        var item = MsalAccountCacheItem.FromJObject(j);
                        contract.Accounts[item.GetKey().ToString()] = item;
                    }
                }
            }

            // App Metadata
            if (root.ContainsKey(StorageJsonValues.AppMetadata))
            {
                foreach (var token in root[StorageJsonValues.AppMetadata].AsObject())
                {
                    if (token.Value is JsonObject j)
                    {
                        var item = MsalAppMetadataCacheItem.FromJObject(j);
                        contract.AppMetadata[item.GetKey().ToString()] = item;
                    }
                }
            }

            return contract;
        }

        private static IDictionary<string, JsonNode> ExtractUnknownNodes(JsonObject root)
        {
            return root
                .Where(kvp => !s_knownPropertyNames.Any(p => string.Equals(kvp.Key, p, StringComparison.OrdinalIgnoreCase)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal string ToJsonString()
        {
            JsonObject root = new JsonObject();

            // Access Tokens
            var accessTokensRoot = new JsonObject();
            foreach (var kvp in AccessTokens)
            {
                accessTokensRoot[kvp.Key] = kvp.Value.ToJObject();
            }

            root[StorageJsonValues.CredentialTypeAccessToken] = accessTokensRoot;

            // Refresh Tokens
            var refreshTokensRoot = new JsonObject();
            foreach (var kvp in RefreshTokens)
            {
                refreshTokensRoot[kvp.Key] = kvp.Value.ToJObject();
            }

            root[StorageJsonValues.CredentialTypeRefreshToken] = refreshTokensRoot;

            // Id Tokens
            var idTokensRoot = new JsonObject();
            foreach (var kvp in IdTokens)
            {
                idTokensRoot[kvp.Key] = kvp.Value.ToJObject();
            }

            root[StorageJsonValues.CredentialTypeIdToken] = idTokensRoot;

            // Accounts
            var accountsRoot = new JsonObject();
            foreach (var kvp in Accounts)
            {
                accountsRoot[kvp.Key] = kvp.Value.ToJObject();
            }

            root[StorageJsonValues.AccountRootKey] = accountsRoot;

            // App Metadata
            var appMetadataRoot = new JsonObject();
            foreach (var kvp in AppMetadata)
            {
                appMetadataRoot[kvp.Key] = kvp.Value.ToJObject();
            }

            root[StorageJsonValues.AppMetadata] = appMetadataRoot;

            // Anything else
            foreach (var kvp in UnknownNodes)
            {
                root[kvp.Key] = kvp.Value != null ? JsonNode.Parse(kvp.Value.ToJsonString()) : null;
            }

            return JsonSerializer.Serialize(
                root,
                new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
                });
        }
    }
}
