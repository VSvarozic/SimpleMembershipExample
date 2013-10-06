using System;
using System.Configuration;

namespace SimpleMembershipModule
{
    public class SimpleMembershipModuleSection : ConfigurationSection
    {
        [ConfigurationProperty("dbContext", IsRequired = true)]
        public ProjectsDbContext DbContext
        {
            get { return (ProjectsDbContext)this["dbContext"]; }
            set { this["dbContext"] = value; }
        }

        [ConfigurationProperty("userProfilesTable")]
        public UserProfilesTable UserProfileTable
        {
            get { return (UserProfilesTable)this["userProfilesTable"]; }
            set { this["userProfilesTable"] = value; }
        }
    }

    public class ProjectsDbContext : ConfigurationElement
    {
        public Type GetDbContextType()
        {
            return Type.GetType(FactoryTypeName);
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string FactoryTypeName 
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("connectionString", DefaultValue = "DefaultConnection", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }
    }

    public class UserProfilesTable : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "UserProfile", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("userIdField", DefaultValue = "UserId", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string UserIdField
        {
            get { return (string)this["userIdField"]; }
            set { this["userIdField"] = value; }
        }

        [ConfigurationProperty("userNameField", DefaultValue = "UserName", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string UserNameField
        {
            get { return (string)this["userNameField"]; }
            set { this["userNameField"] = value; }
        }
    }
}