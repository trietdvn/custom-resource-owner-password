using IdentityModel;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            TestUser user = null;

            // verify username password
            user = Config.GetUsers().FirstOrDefault(p => p.Username == context.UserName && p.Password == context.Password);
            if (user != null)
            {
                context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
            } 
            else
            {
                // verify phone number and auth code
                var authCode = context.Request.Raw["AuthCode"];
                var phoneNumber = context.Request.Raw["PhoneNumber"];

                user = Config.GetUsers().FirstOrDefault(p => p.Claims.Any(k => k.Type == "AuthCode" && k.Value == authCode) 
                    && p.Claims.Any(k => k.Type == ClaimTypes.MobilePhone && k.Value == phoneNumber));
                if (user != null)
                    context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
            }

            return Task.FromResult(0);
        }
    }
}
