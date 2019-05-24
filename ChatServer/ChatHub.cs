using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ChatServer.Models;
using Microsoft.AspNet.SignalR;
namespace SignalRChat
{

    public class ChatHub : Hub
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
        public void Send(string chat_id,string speaker,string from,string to, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            //Clients.User(name).Send(message);
            chatEntities db = new chatEntities();
            chat_table c = new chat_table()
            {
                id = GetHashString(chat_id),
                char_text = message,
                time = DateTime.Now,
                speaker = from
            };
            db.chat_table.Add(c);
            try
            {
                db.SaveChanges();
            }
            catch(Exception e)
            {

            }
            var user_list = chat_id.Split(',');
            foreach(var u in user_list)
            {
                if (u.Length == 0) continue;
                Clients.User(u).addNewMessageToPage(speaker, message);
            }
        }
    }
}