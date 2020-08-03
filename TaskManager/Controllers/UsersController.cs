using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.Models;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TaskManager.Data;
using System.Data.Entity;

namespace TaskManager.Controllers
{
    public class UsersController : BaseController
    {
		[Authorize(Roles = "Admin,User")]
		[Route("users/{id:guid}", Name = "GetRegdUser")]
		public async Task<IHttpActionResult> GetRegdUser(string Id)
		{
			var user = await this.userManager.FindByIdAsync(Id);

			if (user != null)
			{
				return Ok(
					new APIResponse()
					{
						Message = "User Detail has been retrieved successfully",
						Data = this._context.GetUser(user)
					});
			}

			return NotFound();

		}

		[Authorize(Roles = "Admin")]
		[Route("users/all")]
		public IHttpActionResult GetAllUser()
		{
			return Ok(new APIResponse()
				{
					Message = "User Details has been retrieved successfully.",
					Data = this.userManager.Users.ToList().Select(u => this._context.GetUser(u))
				});
		}

		[AllowAnonymous]
		[Route("users/signup")]
		public async Task<IHttpActionResult> CreateUser(AnonymousUser aUser)
		{
			if (ModelState.IsValid)
			{
				var user = new Users()
				{
					Email = aUser.Email,
					FirstName = aUser.FirstName,
					LastName = aUser.LastName,
					PasswordHash = aUser.Password,
					Organization = aUser.Organization,
					UserName = aUser.Email
				};

				IdentityResult addUserResult = await this.userManager.CreateAsync(user, aUser.Password);

				if (addUserResult.Succeeded)
				{
					this.userManager.AddToRole(user.Id, "User");
					return Created(new Uri(Url.Link("GetRegdUser", new { id = user.Id })),new{Message = "User has been registered successfully"});	
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
		[Route("users/signin")]
		public async Task<IHttpActionResult> LoginUser(LoginUser modelUser)
		{
			if (ModelState.IsValid)
			{
				var potUser = await this.userManager.FindAsync(modelUser.Email, modelUser.Password);
				if(potUser !=null)
                {
					return Ok(
					new APIResponse()
					{
						Message = "User has been logged in successfully.",
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
				return BadRequest(ModelState);
			}
		}
	}
}