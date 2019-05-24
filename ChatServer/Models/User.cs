using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatServer.Models
{
    public class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public int sex { get; set; }
        public int age { get; set; }
        public string info { get; set; }
        public bool state { get; set; }
    }
}