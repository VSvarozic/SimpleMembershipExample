using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Properties;

namespace SimpleMembershipModule
{
    /**
     * Для контроллеров WebApi необходимо немного переопределить аттрибут RoledAuthorize
     * из MVC неподходит, потому как в Api используется другой родительский аттрибут из System.Web.Http
     * 
     * Если пользователь не аутентифицирован (не залогинен) возвращаем 401 (Unauthorized)
     * Если у его ролей не хватает прав - 403 (Forbidden)
     * 
     */
    public class ApiRoledAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);

            IPrincipal user = actionContext.ControllerContext.RequestContext.Principal;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("Authenticate required"));
            }
            else
            {
                actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, new HttpError("Not allowed for role"));
            }
            
        }
    }
}
