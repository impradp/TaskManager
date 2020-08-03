namespace TaskManager.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TaskManager.Data;
    using TaskManager.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TaskManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TaskManagerContext context)
        {
            var manager = new UserManager<Users>(new UserStore<Users>(new TaskManagerContext()));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TaskManagerContext()));

            var user = new Users()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                FirstName = "admin",
                LastName = "admin",
                Organization = "LeapFrog"           //No organization needed for SuperAdmin
            };

            manager.Create(user, "admin@123###");

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "User" });
                roleManager.Create(new IdentityRole { Name = "SuperAdmin" });
            }

            var adminUser = manager.FindByName("admin@gmail.com");

            manager.AddToRoles(adminUser.Id, new string[] { "SuperAdmin" });
        }
    }
}
