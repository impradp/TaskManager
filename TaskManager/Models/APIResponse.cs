using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class APIResponse
    {
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
}