using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Linq;

namespace SimpleMembershipModule
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SimpleMembershipInitializeDbAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipModuleInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Инициализаци нужных таблиц происходит только ОДИН раз при запуске приложения
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
            
        }
    }
    
    public class SimpleMembershipModuleInitializer
    {
        private static SimpleMembershipModuleSection _config = null;

        static SimpleMembershipModuleInitializer()
        {
            // Получаем доступ к файлу конфигурации и к нашей секции с настройками
            if (_config == null)
            {
                _config = (SimpleMembershipModuleSection)System.Configuration.ConfigurationManager.GetSection("simpleMembershipModule");
            }
        }

        public SimpleMembershipModuleInitializer()
        {
            // Получаем тип DbContext используемый в проекте
            var localDbContext = _config.DbContext.GetDbContextType();

            // Необходимо для того, чтобы исключить сообщения об ошибках, 
            // которые сгенерирует EF при несоответствии структуры БД и кода модели
            // simpleMembershipModule генерирует несколько внутренних таблиц, необходимых ему для работы
            // но ненужных в нашей модели
            // ********************************************************
            // Используем рефлексию и тип нашей локальной DbContext
            //Database.SetInitializer<MembershipDbContext>(null);
            var dbCtx = typeof(Database).GetMethod("SetInitializer").MakeGenericMethod(localDbContext).Invoke(null, new object[] { null });

            try
            {
                var context = Activator.CreateInstance(localDbContext);

                var dbContext = context as DbContext;

                if (dbContext != null)
                {
                    if (!dbContext.Database.Exists())
                    {
                        // Создаем БД в обход EF Migration
                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                    }

                    dbContext.Dispose();
                }

                InitDbConnection();
                InitDefaultUsersAndRoles();


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The SimpleMembershipModule database could not be initialized. ", ex);
            }
        }

        // Выносим в отдельный статический метод, потому как нужен в нескольких местах
        // и не всегда с созданием объекта.
        public static void InitDbConnection()
        {
            var config =
                (SimpleMembershipModuleSection)System.Configuration.ConfigurationManager.GetSection("simpleMembershipModule");

            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection(
                    config.DbContext.ConnectionString,
                    config.UserProfileTable.Name,
                    config.UserProfileTable.UserIdField,
                    config.UserProfileTable.UserNameField,
                    autoCreateTables: true);
            }
        }

        private void InitDefaultUsersAndRoles()
        {
            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;
            
            if (!roles.GetAllRoles().Contains("Administrator"))
            {
                roles.CreateRole("Administrator");
            }
            if (!roles.GetAllRoles().Contains("User"))
            {
                roles.CreateRole("User");
            }

            MembershipUser user = membership.GetUser("Admin", false);

            if (user == null)
            {
                membership.CreateUserAndAccount("Admin", "1q2w3e");
            }

            if (!roles.GetRolesForUser("Admin").Contains("Administrator"))
            {
                roles.AddUsersToRoles(
                    new[] { "Admin" },
                    new[] { "Administrator" });
            }
        }
    }
}