using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class UserTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        public string Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
    }

    public class TaskGenerate
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UTask
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class TaskDetail
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}