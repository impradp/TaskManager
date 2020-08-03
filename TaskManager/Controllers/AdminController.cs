using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class AdminController : BaseController
    {
		[AllowAnonymous]
		[Route("{tenant}/admin/signup")]
		public async Task<IHttpActionResult> CreateUser(string tenant,AnonymousAdmin aUser)
		{
			if (ModelState.IsValid)
			{
				var user = new Users()
				{
					Email = aUser.Email,
					FirstName = aUser.FirstName,
					LastName = aUser.LastName,
					PasswordHash = aUser.Password,
					Organization = tenant,
					UserName = aUser.Email
				};

				IdentityResult addUserResult = await this.userManager.CreateAsync(user, aUser.Password);

				if (addUserResult.Succeeded)
				{
					var manager = new UserManager<Users>(new UserStore<Users>(new TaskManagerContext()));
					manager.AddToRoles(user.Id, new string[] { "Admin" });
					return Created(new Uri(Url.Link("GetRegdUser", new { id = user.Id })), new { Message = "Admin has been registered successfully" });
				}
				else
				{
					return GetErrorResult(addUserResult);
				}
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[AllowAnonymous]
		[Route("{tenant}/admin/signin")]
		public async Task<IHttpActionResult> LoginAdmin(string tenant, LoginUser modelUser)
		{
			if (ModelState.IsValid)
			{
				var potUser = await this.userManager.FindAsync(modelUser.Email, modelUser.Password);
				if (potUser != null)
				{
					if(potUser.Organization== tenant)
                    {
						return Ok(
								new APIResponse()
								{
									Message = "Admin has been logged in successfully.",
									Data = this._context.GetUser(potUser)
								});
					}
					else
                    {
						return NotFound();
					}
					
				}
				else
				{
					return NotFound();
				}
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize(Roles = "Admin")]
		[Route("{tenant}/admin/userdetail")]
		public IHttpActionResult GetAllUser(string tenant)
		{
			var identity = User.Identity as ClaimsIdentity;
			var potClaim = identity.Claims.Where(e => e.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(e => new
			{
				subject = e.Subject.Name,
				type = e.Type,
				value = e.Value
			}).FirstOrDefault();

			var potAdmin = this.userManager.FindByName(potClaim.subject);
			if(potAdmin!=null)
            {
				if(potAdmin.Organization==tenant)
                {
					using (var _context=new TaskManagerContext())
                    {
						List<UserDetail> uList = new List<UserDetail>();
						List<ReturnUserDetail> rUserList = new List<ReturnUserDetail>();
						var potUsers=_context.Users.Include(u => u.Roles).Where(e=>e.Organization==tenant).ToList();
						var chkRole = _context.Roles.Where(e => e.Name == "User").Select(e => e.Id).FirstOrDefault();
						foreach (var user in potUsers)
                        {
							foreach(var role in user.Roles)
                            {
								if(role.RoleId== chkRole)
                                {
									UserDetail uDetail = new UserDetail();
									uDetail.Email = user.Email;
									uDetail.FirstName = user.FirstName;
									uDetail.LastName = user.LastName;
									uList.Add(uDetail);
								}
                            }
                        }

						if(uList.Count>0)
                        {
							foreach(var user in uList)
                            {
								ReturnUserDetail rUserDetail = new ReturnUserDetail();
								rUserDetail.FirstName = user.FirstName;
								rUserDetail.LastName = user.LastName;
								var task = _context.UserTask.Where(e => e.Email == user.Email).Select(e=>new TaskDetail() { 
								Description=e.Description,
								Name=e.Name,
								Status=e.Status
								}).ToList();
								if(task.Count>0)
                                {
									rUserDetail.Task = task;
                                }
								rUserList.Add(rUserDetail);
                            }
                        }

						if(rUserList.Count>0)
                        {
							return Ok(new APIResponse()
							{
								Message = "User Details has been retrieved successfully.",
								Data = rUserList
							});
						}
                        else
                        {
							return NotFound();
						}
						

					}
						
				}
                else
                {
					return Unauthorized();
                }
            }
            else
            {
				return NotFound();
            }
		}
	}
}
