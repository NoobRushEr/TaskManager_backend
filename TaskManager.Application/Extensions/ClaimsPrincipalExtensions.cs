using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TaskManager.Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public static string GetFirstName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public static List<string> GetUserRoles(this ClaimsPrincipal user)
        {
            return user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        }

        public static bool IsInRole(this ClaimsPrincipal user, string role)
        {
            return user.IsInRole(role);
        }
    }
}