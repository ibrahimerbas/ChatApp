using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace ChatApp.Web.Helpers
{
    public static class IdentityExtensions
    {
        public static string NickName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("NickName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string Avatar(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Avatar");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static Guid? LastReadedMessage(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LastReadedMessage");
            // Test for null to avoid issues during local testing
            return (claim != null && !string.IsNullOrWhiteSpace( claim.Value)) ? new Guid( claim.Value) : (Guid?)null;
        }
    }
}