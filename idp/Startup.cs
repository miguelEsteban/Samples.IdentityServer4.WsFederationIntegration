﻿using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.WsFederation.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace idp
{
    public class Startup
    {
        private static readonly Client RelyingParty = new Client
        {
            ClientId = "rp1",
            AllowedScopes = {"openid", "profile"},
            RedirectUris = {"http://localhost:5001/signin-wsfed"},
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(new List<Client> {RelyingParty})
                .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"))
                .AddWsFederationPlugin()
                .AddInMemoryRelyingParties(new List<RelyingParty>());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer()
               .UseIdentityServerWsFederationPlugin();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}