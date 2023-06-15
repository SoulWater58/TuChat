using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TuChatWebService2.Models;
using TuChatWebService2.Models.Database;

namespace TuChatWebService2.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // метод авторизации пользователя
        [HttpPost]
        public string Auth(string s)
        {
            DigitPair dP = JsonConvert.DeserializeObject<DigitPair>(s);

            User authUser;
            User authPassUser;
            using (TuChatDBEntities context = new TuChatDBEntities())
            {
                authUser = context.Users.Where(b => b.Number == dP.Number).FirstOrDefault();
                authPassUser = context.Users.Where(b => b.Number == dP.Number && b.Password == dP.Password).FirstOrDefault();
            }

            if (authUser != null)
            {
                if (authPassUser != null)
                {
                    UserData uD = new UserData()
                    {
                        Id = authPassUser.Id.ToString(),
                        Username = authPassUser.Login
                    };

                    return JsonConvert.SerializeObject(uD);
                }
                else
                {
                    return JsonConvert.SerializeObject(-1);
                }
            }
            else
            {
                return JsonConvert.SerializeObject(0);
            }
        }

        // метод регистрации пользователя
        [HttpPost]
        public string Reg(string s)
        {
            RegistrationUserData rUD = JsonConvert.DeserializeObject<RegistrationUserData>(s);

            User numberUser;
            User emailUser;
            using (TuChatDBEntities context = new TuChatDBEntities())
            {
                numberUser = context.Users.Where(b => b.Number == rUD.Number).FirstOrDefault();
                emailUser = context.Users.Where(b => b.Email == rUD.Email).FirstOrDefault();
            }

            if (numberUser == null)
            {
                if (emailUser == null)
                {
                    TuChatDBEntities tuChatDB = new TuChatDBEntities();
                    User user = new User()
                    {
                        Name = rUD.Name,
                        Surname = rUD.Surname,
                        Login = rUD.Login,
                        Password = rUD.Password,
                        Email = rUD.Email,
                        Number = rUD.Number
                    };
                    tuChatDB.Users.Add(user);
                    tuChatDB.SaveChanges();
                    return JsonConvert.SerializeObject(1);
                }
                else
                {
                    return JsonConvert.SerializeObject(0);
                }
            }
            else if (numberUser != null && emailUser != null)
            {
                return JsonConvert.SerializeObject(-1);
            }
            else
            {
                return JsonConvert.SerializeObject(-2);
            }
        }

        // метод поиска зарегистрированных пользователей по номерам
        [HttpPost]
        public string FriendNumber(string s)
        {
            FriendNumber fN = JsonConvert.DeserializeObject<FriendNumber>(s);

            User userNum;
            using (TuChatDBEntities context = new TuChatDBEntities())
            {
                userNum = context.Users.Where(b => b.Number == fN.Number).FirstOrDefault();
            }

            if (userNum != null)
            {
                return JsonConvert.SerializeObject(userNum.Id);
            }
            else
            {
                return JsonConvert.SerializeObject(false);
            }
        }

        [HttpPost]
        public string RememberPassword(string s)
        {
            User authUser = null;
            using(var context = new TuChatDBEntities())
            {
                authUser = context.Users.Where(b => b.Number == s).FirstOrDefault();

                if (authUser != null)
                {
                    string email = authUser.Email;
                    string pass = authUser.Password;

                    try
                    {
                        WebMail.Send(to: email, subject: "TuChat", body: $"Пароль к вашему аккаунту: {pass}. Больше не забывайте)");

                        return JsonConvert.SerializeObject(email);
                    }
                    catch
                    {
                        return JsonConvert.SerializeObject(-1);
                    }

                    //WebMail.Send(to: email, subject: "TuChat", body: $"Пароль к вашему аккаунту: {pass}. Больше не забывайте)");

                    //return JsonConvert.SerializeObject(email);
                }
                else
                {
                    return JsonConvert.SerializeObject(0);
                }
            }
        }
    }
}