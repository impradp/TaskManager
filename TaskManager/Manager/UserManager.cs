using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Manager
{
    public class UserManager : UserManager<Users>
    {
        public UserManager(IUserStore<Users> store) : base(store)
        {
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {

            var managerContext = context.Get<TaskManagerContext>();
            var manager = new UserManager(new UserStore<Users>(managerContext));
            manager.UserValidator = new UserValidator<Users>(manager)
            {
                RequireUniqueEmail = true
            };
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 10
            };
            return manager;
        }
    }
}