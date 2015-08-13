using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using AccidentalFish.AspNet.Identity.Azure.GraphAPIHelper;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ActiveDirectory;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;

namespace AccidentalFish.AspNet.Identity.Azure
{
    /// <summary>
    /// This specialization of the ClaimsAuthenticationManager class uses the Azure AD Graph API to
    /// find the AD groups that a user is a member of and transforms them into role claims.
    /// 
    /// These claims can then be used with the Authorize(Roles="A Role") authorization attribute in
    /// an MVC application.
    /// </summary>
    public class GraphRoleClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        private const string TenantIdClaim = "http://schemas.microsoft.com/identity/claims/tenantid";
        private const string ObjectIdentifierClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        private readonly string _clientId;
        private readonly string _password;
        private readonly string _issuer;

        public GraphRoleClaimsAuthenticationManager()
        {
            _clientId = CloudConfigurationManager.GetSetting("ida:ClientID");
            _password = CloudConfigurationManager.GetSetting("ida:Password");
            _issuer = CloudConfigurationManager.GetSetting("ida:RoleClaimIssuer");
            if (String.IsNullOrWhiteSpace(_issuer))
            {
                _issuer = "DefaultRoleIssuer";
            }
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal != null && incomingPrincipal.Identity.IsAuthenticated)
            {
                // Get the claims required to make further Graph API enquiries about the user
                Claim tenantClaim = incomingPrincipal.FindFirst(TenantIdClaim);
                if (tenantClaim == null)
                {
                    throw new NotSupportedException("Tenant claim not available, role authentication is not supported");
                }
                Claim objectIdentifierClaim = incomingPrincipal.FindFirst(ObjectIdentifierClaim);
                if (objectIdentifierClaim == null)
                {
                    throw new NotSupportedException("Object identifier claim not available, role authentication is not supported");
                }

                string tenantId = tenantClaim.Value;
                string currentUserObjectId = objectIdentifierClaim.Value;

                // Connect to the graph service
                AADJWTToken token = DirectoryDataServiceAuthorizationHelper.GetAuthorizationToken(tenantId, _clientId, _password);
                DirectoryDataService graphService = new DirectoryDataService(tenantId, token);

                // Find the user in the graph
// ReSharper disable once ReplaceWithSingleCallToSingleOrDefault - SingleOrDefault not supported on directory service directly
                User currentUser = graphService.directoryObjects.OfType<User>().Where(it => (it.objectId == currentUserObjectId)).SingleOrDefault();
                if (currentUser == null)
                {
                    throw new SecurityException("User cannot be found in graph");
                }

                // Find the groups the user is a member of and add them as role claims
                graphService.LoadProperty(currentUser, "memberOf");
                List<Group> currentRoles = currentUser.memberOf.OfType<Group>().ToList();
                foreach (Group role in currentRoles)
                {
                    ((ClaimsIdentity) incomingPrincipal.Identity).AddClaim(new Claim(ClaimTypes.Role, role.displayName, ClaimValueTypes.String, _issuer));
                }
            }
            return base.Authenticate(resourceName, incomingPrincipal);
        }
    }
}