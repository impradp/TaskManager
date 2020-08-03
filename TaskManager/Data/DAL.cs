using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using TaskManager.Manager;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class DAL
    {
        private UrlHelper _urlHelper;
        private UserManager _userManager;
        private readonly Random _random = new Random();

        public DAL(HttpRequestMessage request, UserManager userManager)
        {
            _urlHelper = new UrlHelper(request);
            _userManager = userManager;
        }

        public string GetPotentialUsername(string fName, string lName)
        {
            return String.Concat(fName[0], lName.ToLower(), _random.Next(100));
        }

        public ReturnUser GetUser(Users user)
        {
            return new ReturnUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Organization = user.Organization
            };
        }

        public ReturnRole Create(IdentityRole appRole)
        {

            return new ReturnRole
            {
                Url = _urlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id,
                Name = appRole.Name
            };
        }
    }
}