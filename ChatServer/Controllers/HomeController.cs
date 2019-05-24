using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
/* 基于asp.net的web聊天室
    （1）新用户注册 ok
    （2）用户登录 ok
    （3）管理员管理用户信息和权限
    （4）选择聊天对象 ok
    （5）将聊天时产生的聊天记录存放在数据库 ok
    （6）查看历史聊天记录 ok
    （7）可以编辑个人信息
    （8）显示用户是否在线或者是离开状态 ok
 */
namespace ChatServer.Controllers
{
    public class HomeController : Controller
    {
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        void updateState()
        {
            if (!(Session["login"] is null))
            {
                string id = Session["id"].ToString();
                chatEntities db = new chatEntities();
                var user = from u in db.user_table
                           where (u.id == id)
                           select u;
                user.First().last_app = DateTime.Now;
                db.SaveChanges();
            }
        }
        double timeDiff(DateTime t)
        {
            TimeSpan ts = DateTime.Now - t;
            return ts.TotalMinutes;
        }
        public ActionResult Index()
        {
            updateState();
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            updateState();
            chatEntities db = new chatEntities();
            var user = from u in db.user_table
                       where (u.username == username && u.password == password)
                       select u;
            if (user.Count() == 0)
                return Json(false);
            Session["login"] = true;
            Session["id"] = user.First().id.Trim();
            Session["username"] = user.First().username.Trim();
            Session["privilege"] = user.First().privilege.Value;
            var context = HttpContext;
            //唯一的登陆ID,作为连接ID
            context.Response.Cookies.Add(new HttpCookie("srconnectionid") { Value = user.First().id.Trim() });
            return Json(Session["id"]);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return View("Index");
        }
        public ActionResult Ground()
        {
            updateState();
            if (Session["login"] is null)
            {
                return View("LoginFirst");
            }
            chatEntities db = new chatEntities();
            var user = from u in db.user_table
                       select u;
            List<User> list = new List<User>();
            foreach(var u in user)
            {
                list.Add(new User() { username=u.username,info=u.info,age=u.age.Value, id=u.id,sex=u.sex.Value,state= (timeDiff(u.last_app.Value) < 1.0)});
            }
            ViewData["UserList"] = list;
            return View();
        }

        public ActionResult Chat(string id)
        {
            var id_list = id.Split(',');
            Array.Sort(id_list);

            chatEntities db = new chatEntities();
            var user_list = new List<user_table>();
            StringBuilder sb = new StringBuilder();
            for(int i=0;i< id_list.Length;i++)
            {
                var s = id_list[i];
                var user = from u in db.user_table
                           where u.id==s
                           select u;
                user_list.Add(user.First());
                sb.Append(user.First().username + " ");
            }
            
            string from = Session["id"].ToString();
            string chat_id = string.Join(",", id_list);
            string hash_id = GetHashString(chat_id);
            var history = from h in db.chat_table
                          join u in db.user_table
                          on h.speaker equals u.id
                          where h.id == hash_id
                          orderby h.time
                          select new History() { speaker = u.username, content = h.char_text, time = h.time };
            List<History> list = history.ToList();
            ViewData["history"] = list;
            ViewData["users"] = user_list;
            ViewBag.chat_id = chat_id;
            ViewBag.usernames = sb.ToString();
            return View();
        }
        public ActionResult Info()
        {
            string id = Session["id"].ToString().Trim();
            chatEntities db = new chatEntities();
            var user = (from u in db.user_table
                       where u.id == id
                       select u).First();
            ViewBag.password = user.password.Trim();
            ViewBag.sex = user.sex;
            ViewBag.age = user.age;
            ViewBag.info = user.info;
            string from = Session["id"].ToString();
            string chat_id = from.CompareTo(id) < 0 ? $"{from},{id}" : $"{id},{from}";
            var history = from h in db.chat_table
                          join u in db.user_table
                          on h.speaker equals u.id
                          where h.id == chat_id
                          orderby h.time
                          select new History() { speaker = u.username, content = h.char_text, time = h.time };
            List<History> list = history.ToList();
            ViewData["history"] = list;
            return View();
        }
        public ActionResult Control()
        {
            updateState();
            if (Session["login"] is null)
            {
                return View("LoginFirst");
            }
            chatEntities db = new chatEntities();
            var user = from u in db.user_table
                       select u;
            ViewData["UserList"] = user.ToList();
            return View();
        }
    }
}