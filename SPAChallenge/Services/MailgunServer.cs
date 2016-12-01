using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Web.Configuration;
using SPAChallenge.Models;

namespace SPAChallenge.Services
{
    public class MailgunServer : EmailServer
    {
        public override bool PingServer()
        {
            int pingTimeout = Int32.Parse(WebConfigurationManager.AppSettings["DefaultPingTimeout"]);
            return EmailServices.PingServer(WebConfigurationManager.AppSettings["MailgunHostname"], pingTimeout, 1);
        }

        public override string SendMail(Email email)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri(WebConfigurationManager.AppSettings["MailgunSendUri"]);
            client.Authenticator =
                   new HttpBasicAuthenticator("api",
                                              WebConfigurationManager.AppSettings["MailgunKey"]);
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                WebConfigurationManager.AppSettings["MailgunDomain"],
                ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", email.From);
            request.AddParameter("to", email.Tos == null ? email.From : string.Join(", ", email.Tos));
            if (email.Ccs != null) request.AddParameter("cc", string.Join(", ", email.Ccs));
            if (email.Bccs != null) request.AddParameter("bcc", string.Join(", ", email.Bccs));
            request.AddParameter("subject", email.Subject);
            request.AddParameter("text", email.Content);
            request.Method = Method.POST;
            RestResponse rr = (RestResponse)client.Execute(request);
            ResponseMessage rm = JsonConvert.DeserializeObject<ResponseMessage>(rr.Content);
            return rm.message;
        }
    }
}