using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using SimpleMembershipExample.Models;
using WebMatrix.WebData;
using System.Linq;

namespace SimpleMembershipExample
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Обеспечение однократной инициализации ASP.NET Simple Membership при каждом запуске приложения
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<SMDbContext>(null);

                try
                {
                    using (var context = new SMDbContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Создание базы данных SimpleMembership без схемы миграции Entity Framework
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    /**
                     *   Инициализируем стартовых пользователей и роли
                     */
                    InitDefaultUsersAndRoles();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Не удалось инициализировать базу данных ASP.NET Simple Membership.", ex);
                }
            }

            private void InitDefaultUsersAndRoles()
            {
                var roles = (SimpleRoleProvider)Roles.Provider;
                var membership = (SimpleMembershipProvider)Membership.Provider;

                // проверяю наличие ролей, если нет создаем
                if (!roles.GetAllRoles().Contains("Administrator"))
                {
                    roles.CreateRole("Administrator");
                }
                if (!roles.GetAllRoles().Contains("User"))
                {
                    roles.CreateRole("User");
                }

                // проверяю наличие зарегистрированного пользователя
                MembershipUser user = membership.GetUser("Admin", false);

                //создаю, если пользователя не найдено
                if (user == null)
                {
                    membership.CreateUserAndAccount("Admin", "1q2w3e");
                }

                //добавляю пользователю права администратора
                if (!roles.GetRolesForUser("Admin").Contains("Administrator"))
                {
                    roles.AddUsersToRoles(
                        new[] { "Admin" },
                        new[] { "Administrator" });
                }
            }
        }
    }
}
