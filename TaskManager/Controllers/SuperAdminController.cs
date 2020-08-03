using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class SuperAdminController : ApiController
    {
		[Authorize(Roles = "SuperAdmin")]
		[Route("all")]
		public IHttpActionResult GetAllUser()
		{
			using (var _context=new TaskManagerContext())
            {
				var potUsers = _context.Users.Include(u => u.Roles).ToList();
				var chkRoleuser = _context.Roles.Where(e => e.Name == "User").Select(e => e.Id).FirstOrDefault();
				var chkRoleAdmin = _context.Roles.Where(e => e.Name == "Admin").Select(e => e.Id).FirstOrDefault();
				AllDetail alldetail = new AllDetail();
				List<AllAdmin> alladmin = new List<AllAdmin>();
				List<AllUsers> alluser = new List<AllUsers>();
				foreach (var user in potUsers)
                {
					foreach (var role in user.Roles)
                    {
						if(role.RoleId== chkRoleAdmin)
                        {
							AllAdmin admin = new AllAdmin();
							admin.Email = user.Email;
							admin.FirstName = user.FirstName;
							admin.LastName = user.LastName;
							admin.Organization = user.Organization;
							alladmin.Add(admin);
						}
						if (role.RoleId == chkRoleuser)
						{
							AllUsers users = new AllUsers();
							users.Email = user.Email;
							users.FirstName = user.FirstName;
							users.LastName = user.LastName;
							users.Organization = user.Organization;
							users.Task= _context.UserTask.Where(e => e.Email == user.Email).Select(e => new TaskDetail()
							{
								Description = e.Description,
								Name = e.Name,
								Status = e.Status
							}).ToList();
							alluser.Add(users);
						}
					}
                }
				alldetail.Admins = alladmin;
				alldetail.Users = alluser;
				if(alldetail !=null)
                {
					return Ok(new APIResponse()
					{
						Message = "User Details has been retrieved successfully.",
						Data = alldetail
					});
				}
                else
                {
					return NotFound();//No Data Found
				}
			}
			
		}
	}
}
