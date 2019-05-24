using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChatServer.Controllers
{
    public class UpdateController : ApiController
    {
        public string Post([FromBody]dynamic value)
        {
            if((int)value["type"]==1)
            {
                chatEntities db = new chatEntities();
                string id = value["id"];
                var list = from u in db.user_table where u.id == id select u;

                if ((list.Count() == 0))
                {
                    return "用户名不存在";
                }
                var user = list.First();
                user.password = value["password"];
                user.privilege = value["privilege"];
                user.sex = value["sex"];
                user.age = value["age"];
                user.info = value["info"];
                int resCount = db.SaveChanges();
                return user.id;
            }
            else
            {
                chatEntities db = new chatEntities();
                string username = value["username"];
                var list = from u in db.user_table where u.username == username select u;

                if ((list.Count() == 0))
                {
                    return "用户名不存在";
                }
                var user = list.First();
                user.password = value["password"];
                user.sex = value["sex"];
                user.age = value["age"];
                user.info = value["info"];
                int resCount = db.SaveChanges();
                return user.id;
            }
        }
    }
}
