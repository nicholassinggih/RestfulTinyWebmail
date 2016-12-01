using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using SPAChallenge.Models;
using SPAChallenge.Services;
using System.Web.Configuration;

namespace SPAChallenge.Controllers
{
    public class EmailController : ApiController
    {
        public HttpResponseMessage Post(Email email)
        {
            EmailServices.PrepareEmail(email);
            string message = "Sorry, your mail was not sent. Mail service providers are unreachable at the moment. Please notify your network administrators.";
            int pingTimeout = Int32.Parse(WebConfigurationManager.AppSettings["DefaultPingTimeout"]);
            List<EmailServer> servers = new List<EmailServer>();
            servers.Add(new MailgunServer());
            servers.Add(new SendGridServer());
            foreach(EmailServer server in servers)
            {
                if (server.PingServer())
                {
                    message = server.SendMail(email);
                    break;
                }
            }
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(message, Encoding.UTF8);
            return response;

        }
    }
}
