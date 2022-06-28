﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.Identity.Client.Cache.Keys;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.OAuth2;
using Microsoft.Identity.Client.Utils;

namespace Microsoft.Identity.Client.Cache.Items
{
    internal class MsalIdTokenCacheItem : MsalCredentialCacheItemBase
    {
        internal MsalIdTokenCacheItem()
        {
            CredentialType = StorageJsonValues.CredentialTypeIdToken;
            idTokenLazy = new Lazy<IdToken>(() => IdToken.Parse(Secret));
        }

        internal MsalIdTokenCacheItem(
            string preferredCacheEnv,
            string clientId,
            MsalTokenResponse response,
            string tenantId,
            string homeAccountId)
            : this(
                preferredCacheEnv,
                clientId,
                response.IdToken,
                response.ClientInfo,
                homeAccountId,
                tenantId)
        {
        }

        internal MsalIdTokenCacheItem(
            string preferredCacheEnv,
            string clientId,
            string secret,
            string rawClientInfo,
            string homeAccountId,
            string tenantId)
            : this()
        {
            Environment = preferredCacheEnv;
            TenantId = tenantId;
            ClientId = clientId;
            Secret = secret;
            RawClientInfo = rawClientInfo;
            HomeAccountId = homeAccountId;
        }

    
        internal string TenantId { get; set; }

        private readonly Lazy<IdToken> idTokenLazy;

        internal IdToken IdToken => idTokenLazy.Value;

        internal MsalIdTokenCacheKey GetKey()
        {
            return new MsalIdTokenCacheKey(Environment, TenantId, HomeAccountId, ClientId);
        }

        internal static MsalIdTokenCacheItem FromJsonString(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return FromJObject(JsonNode.Parse(json).AsObject());
        }

        internal static MsalIdTokenCacheItem FromJObject(JsonObject j)
        {
            var item = new MsalIdTokenCacheItem
            {
                TenantId = JsonUtils.ExtractExistingOrEmptyString(j, StorageJsonKeys.Realm),
            };

            item.PopulateFieldsFromJObject(j);

            return item;
        }

        internal override JsonObject ToJObject()
        {
            var json = base.ToJObject();
            SetItemIfValueNotNull(json, StorageJsonKeys.Realm, TenantId);
            return json;
        }

        internal string ToJsonString()
        {
            return ToJObject()
                .ToString();
        }

        internal string GetUsername()
        {
            return IdToken?.PreferredUsername ?? IdToken?.Upn;
        }
    }
}
