﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Reddit.Net.Auth
{
    public class RedditCodeAuth : RedditAuth
    {
        public string Code { get; set; }

        public string RefreshToken { get; set; }

        public RedditCodeAuth(string clientId, string clientSecret, string redirectUri, string code) : base(clientId, clientSecret, redirectUri)
        {
            Code = code;
        }

        public override async Task GetOAuthTokenAsync()
        {
            TokenResponse resp;

            if (!string.IsNullOrEmpty(RefreshToken))
                resp = await TokenClient.RequestRefreshTokenAsync(RefreshToken).ConfigureAwait(false);
            else
                resp = await TokenClient.RequestAuthorizationCodeAsync(Code, RedirectUri).ConfigureAwait(false);

            if (resp.IsError)
                throw new Exception(resp.IsHttpError ? resp.HttpErrorReason : resp.Error);

            if (resp.ExpiresIn > 0)
                ExpireTime = DateTime.UtcNow.AddSeconds(resp.ExpiresIn);

            AccessToken = resp.AccessToken;
            RefreshToken = resp.RefreshToken;
        }
    }
}
