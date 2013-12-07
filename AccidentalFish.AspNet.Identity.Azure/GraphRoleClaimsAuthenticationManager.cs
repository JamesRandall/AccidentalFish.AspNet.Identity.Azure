using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using AccidentalFish.AspNet.Identity.Azure.GraphAPIHelper;
using Microsoft.WindowsAzure;
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
                string tenantId = incomingPrincipal.FindFirst(TenantIdClaim).Value;
                AADJWTToken token = DirectoryDataServiceAuthorizationHelper.GetAuthorizationToken(tenantId, _clientId, _password);
                DirectoryDataService graphService = new DirectoryDataService(tenantId, token);

                // Find the user in the graph
                string currentUserObjectId = incomingPrincipal.FindFirst(ObjectIdentifierClaim).Value;
                User currentUser = graphService.directoryObjects.OfType<User>().SingleOrDefault(it => (it.objectId == currentUserObjectId));
                if (currentUser == null)
                {
                    throw new SecurityException("User cannot be found in graph");
                }

                // Find the groups the user is a member of and add them as role claims
                graphService.LoadProperty(currentUser, "memberOf");
                List<Group> currentRoles = currentUser.memberOf.OfType<Group>().ToList();
                foreach (Group role in currentRoles)
                {
                    ((ClaimsIdentity)incomingPrincipal.Identity).AddClaim(new Claim(ClaimTypes.Role, role.displayName, ClaimValueTypes.String, _issuer));
                }
            }
            return base.Authenticate(resourceName, incomingPrincipal);
        }
    }
}
