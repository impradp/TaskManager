using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
	public class TaskController : BaseController
	{
		[Authorize(Roles = "User")]
		[Route("tasks/create")]
		public async Task<IHttpActionResult> CreateTask(TaskGenerate tasks)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var identity = User.Identity as ClaimsIdentity;
					var potClaim = identity.Claims.Where(e => e.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(e => new
					{
						subject = e.Subject.Name,
						type = e.Type,
						value = e.Value
					}).FirstOrDefault();

					var user = await this.userManager.FindByNameAsync(potClaim.subject);
					if (user != null)
					{
						using (var _context = new TaskManagerContext())
						{
							var potTask = new UserTask()
							{
								Email = user.Email,
								Name = tasks.Name,
								Description = tasks.Description,
								Status = "Created"
							};
							_context.UserTask.Add(potTask);
							_context.SaveChanges();
							return Created(new Uri(Url.Link("GetRegdUser", new { id = user.Id })), new APIResponse()
							{
								Message = "Task created successfully.",
								Data = new { Id = potTask.Id, Name = potTask.Name, Description = potTask.Description, Status = potTask.Status }
							});
						}
					}
					else
					{
						return Unauthorized();
					}

				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch
			{
				//store the error log for debugging
				return InternalServerError();
			}

		}

		[Authorize(Roles = "User")]
		[Route("tasks/all")]
		public IHttpActionResult DisplayTask()
		{
			var identity = User.Identity as ClaimsIdentity;
			var potClaim = identity.Claims.Where(e => e.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(e => new
			{
				subject = e.Subject.Name,
				type = e.Type,
				value = e.Value
			}).FirstOrDefault();
			using (var _context = new TaskManagerContext())
			{
				var uuTask = _context.UserTask.Where(s => s.Email == potClaim.subject).Select(e=>new
				{
					Id = e.Id,
					Name = e.Name,
					Description = e.Description,
					Status = e.Status
				}).ToList();
				return Ok(new APIResponse()
				{
					Message = "User tasks are retrieved successfully.",
					Data = uuTask
				});
			}

		}

		[Authorize(Roles = "User")]
		[Route("tasks/update")]
		public IHttpActionResult UpdateTask(UTask uTask)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var identity = User.Identity as ClaimsIdentity;
					var potClaim = identity.Claims.Where(e => e.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(e => new
					{
						subject = e.Subject.Name,
						type = e.Type,
						value = e.Value
					}).FirstOrDefault();
					using (var _context = new TaskManagerContext())
					{
						var uuTask = _context.UserTask.Where(s => s.Email == potClaim.subject && s.Id == uTask.Id).FirstOrDefault();
						if(uuTask!=null)
                        {
							if (!String.IsNullOrEmpty(uTask.Name))
							{
								uuTask.Name = uTask.Name;
							}
							if (!String.IsNullOrEmpty(uTask.Description))
							{
								uuTask.Description = uTask.Description;
							}
							_context.UserTask.Attach(uuTask);
							if (!String.IsNullOrEmpty(uTask.Name))
							{
								_context.Entry(uuTask).Property(x => x.Name).IsModified = true;
							}
							if (!String.IsNullOrEmpty(uTask.Name))
							{
								_context.Entry(uuTask).Property(x => x.Description).IsModified = true;
							}
							_context.SaveChanges();
							return Ok(new APIResponse()
							{
								Message = "User task has been updated successfully.",
								Data = new {Id=uuTask.Id,Name=uuTask.Name,Description=uuTask.Description,Status=uuTask.Status }
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
					return BadRequest(ModelState);
				}
			}
			catch (Exception ex)
			{
				string exp = ex.ToString();
				//store the error log for debugging
				return InternalServerError();
			}

		}
		[Authorize(Roles = "User")]
		[Route("task/statusupdate/{id:guid}", Name = "UpdateTStatus")]
		public IHttpActionResult UpdateTStatus(Guid Id)
		{
			var identity = User.Identity as ClaimsIdentity;
			var potClaim = identity.Claims.Where(e => e.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(e => new
			{
				subject = e.Subject.Name,
				type = e.Type,
				value = e.Value
			}).FirstOrDefault();
			using (var _context=new TaskManagerContext())
            {
				var task = _context.UserTask.Where(e => e.Email == potClaim.subject && e.Id == Id).FirstOrDefault();
				if (task != null)
				{
					task.Status = "Completed";
					_context.UserTask.Attach(task);
					_context.Entry(task).Property(x => x.Status).IsModified = true;
					_context.SaveChanges();
					return Ok(
						new APIResponse()
						{
							Message = "Task has been marked completed successfully.",
							Data = new { Id = task.Id, Name = task.Name, Description = task.Description, Status = task.Status }
						});
				}
                else
                {
					return NotFound();
				}
			}
		}

	}
}
