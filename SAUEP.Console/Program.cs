using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.Core.Repositories;
using SAUEP.Core.Services;
using SAUEP.Core.Connections;
using SAUEP.Core.Models;
using SAUEP.Core.Exceptions;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Mail;

namespace SAUEP.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            SendMail();
        }
        static async void Test()
        {
            Guardian guardian = new Guardian(new Logger());
            AuthorizationService authorizationService = new AuthorizationService(new ServerConnection(new FileReader(), new JsonParser()), new JsonParser());
            PollRepository pollRepository = new PollRepository(new ServerConnection(new FileReader(), new JsonParser()), new JsonParser());
            var i = await authorizationService.Authorize("Admin", "123456789q");
            System.Console.WriteLine("Read end");
            try
            {
                var q = await pollRepository.Get<PollModel>(1, DateTime.Now, "ANUBIS", i.access_token);
                System.Console.WriteLine("Register end");
            }
            catch (SAUEP.Core.Exceptions.TokenLifetimeException ex)
            {

            }
        }

        public static void SendMail(string smtpServer = "smtp.gmail.com", string from = "sauep228@gmail.com", string password = "Vinniko333*", string mailto="vinnik_21@bk.ru", string caption="message", string message = "test", string attachFile = null)
        {
            try
            {
                MailAddress mailfrom = new MailAddress(from, "Tom");
                MailAddress to = new MailAddress(mailto);
                MailMessage m = new MailMessage(mailfrom, to);
                m.Subject = caption;
                m.Body = message;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(from, password);
                smtp.EnableSsl = true;
                smtp.Send(m);
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }
    }
}
