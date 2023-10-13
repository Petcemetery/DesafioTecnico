using Microsoft.AspNetCore.Authorization;
using DesafioTecnico.Crosscutting.Constants;

namespace DesafioTecnico.Security
{
    public static class PoliciesConstants
    {
        public static readonly AuthorizationPolicy PolicyAdmin = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser().RequireRole(RolesConstants.ADMIN).Build();

        public static readonly AuthorizationPolicy PolicyUser = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser().RequireRole(RolesConstants.USER).Build();
    }
}
