// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using System.Transactions;
using IdentityModel;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerAspNetIdentity
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var alice = userMgr.FindByNameAsync("alice").Result;
                    if (alice == null)
                    {
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                alice = new ApplicationUser
                                {
                                    UserName = "alice"
                                };
                                var createresult = userMgr.CreateAsync(alice, "Pass123$").Result;
                                if (!createresult.Succeeded)
                                {
                                    throw new Exception(createresult.Errors.First().Description);
                                }

                                var addresult = userMgr.AddClaimsAsync(alice, new Claim[]{
                                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                    new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                                }).Result;
                                if (!addresult.Succeeded)
                                {
                                    throw new Exception(addresult.Errors.First().Description);
                                }
                                transaction.Complete();
                                Console.WriteLine("alice created");
                            }
                            catch (Exception ex)
                            {
                                transaction.Dispose();
                                Console.WriteLine(ex.InnerException);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("alice already exists");
                    }

                    var bob = userMgr.FindByNameAsync("bob").Result;
                    if (bob == null)
                    {
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                bob = new ApplicationUser
                                {
                                    UserName = "bob"
                                };
                                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                                if (!result.Succeeded)
                                {
                                    throw new Exception(result.Errors.First().Description);
                                }

                                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                    new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                                }).Result;
                                if (!result.Succeeded)
                                {
                                    throw new Exception(result.Errors.First().Description);
                                }
                                transaction.Complete();
                                Console.WriteLine("bob created");
                            }
                            catch (Exception ex)
                            {
                                transaction.Dispose();
                                Console.WriteLine(ex.InnerException);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("bob already exists");
                    }
                }
            }
        }
    }
}
