using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChatServer.Controllers
{
    public class RegistController : ApiController
    {
        public string Post([FromBody]dynamic value)
        {
            chatEntities db = new chatEntities();
            string username = value["username"];
            if ((from u in db.user_table where u.username == username select u).Count()!=0)
            {
                return "用户名已存在";
            }
            user_table usrModel = new user_table()
            {
                id = Guid.NewGuid().ToString(),
                username = value["username"],
                password = value["password"],
                privilege = 1,
                sex = value["sex"],
                age = value["age"],
                info = value["info"]
            };
            db.user_table.Add(usrModel);
            int resCount = db.SaveChanges();
            return usrModel.id;
        }
    }
}
