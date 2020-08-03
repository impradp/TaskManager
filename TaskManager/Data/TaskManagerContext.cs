using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class TaskManagerContext : IdentityDbContext<Users>
    {
        public TaskManagerContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static TaskManagerContext Create()
        {
            return new TaskManagerContext();
        }
        public DbSet<UserTask> UserTask { get; set; }
    }
}