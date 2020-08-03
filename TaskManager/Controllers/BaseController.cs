using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TaskManager.Data;
using TaskManager.Manager;

namespace TaskManager.Controllers
{
    public class BaseController : ApiController
    {
        // GET: Base
        private DAL context;
        private UserManager _userManager = null;
        private RoleManager _roleManager = null;

        protected UserManager userManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<UserManager>();
            }
        }

        protected RoleManager rolemanager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<RoleManager>();
            }
        }

        public BaseController()
        {
        }

        protected DAL _context
        {
            get
            {
                if (context == null)
                {
                    context = new DAL(this.Request, this.userManager);
                }
                return context;
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}