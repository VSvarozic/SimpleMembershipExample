/**
 *  Необходимо для кастомной реакции на доступ к данным, требующим авторизации зависящей от ролей.
 *  Базовый атрибут просто проверяет авторизован или нет пользователь, и редиректит на страницу логина.
 *  В случае же с ролями может быть так, что пользователь авторизован, но не имеет права на просмотр данной страницы, 
 *  так как его роль не соответствует требуемой. Поэтому редиректим его на кастомную страницу ошибок.
 */

using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleMembershipModule
{
    public class RoledAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Error", action = "AccessDenied", Area = "" }));
            }
        }
    }
}