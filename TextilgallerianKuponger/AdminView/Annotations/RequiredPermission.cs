﻿using System.Web;
using System.Web.Mvc;
using Domain.Entities;

namespace AdminView.Annotations
{
    public class RequiredPermission : AuthorizeAttribute
    {
        private Permission _permission;

        public RequiredPermission(Permission permission)
        {
            _permission = permission;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Controller.TempData.Add("error", "Du har inte behörighet att visa sidan!");
            filterContext.Result = new RedirectResult("~/");
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session == null) return false;

            var user = httpContext.Session["user"] as User;
            var role = httpContext.Session["role"] as Role;

            return user != null && user.IsActive && role.Permissions.Contains(_permission);
        }
    }
}