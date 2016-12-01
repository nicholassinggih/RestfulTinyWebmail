using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using SPAChallenge.Models;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Web.Configuration;

namespace SPAChallenge.Services
{
    public class EmailServices
    {
        public static bool PingServer(string host, int timeout, int retries)
        {
            var ping = new Ping();
            try
            {
                PingReply reply = ping.Send(host, timeout);
                if (reply.Status.Equals(IPStatus.TimedOut))
                {
                    if (retries <= 0) return false;
                    else return PingServer(host, timeout, retries - 1);
                }
                else if (reply.Status.Equals(IPStatus.Success))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                
            }

            return false;   // other than if the ping was successful, it is assumed that the remote server is unreachable
        }
        public static void PrepareEmail(Email email)
        {
            email.Tos = email.Tos != null && email.Tos.Length > 0 ? email.Tos : !string.IsNullOrEmpty(email.From)? new string[] { email.From } : null;
            email.Ccs = email.Ccs!= null && email.Ccs.Length > 0 ? email.Ccs : null;
            email.Bccs = email.Bccs != null && email.Bccs.Length > 0 ? email.Bccs : null;
            email.Content = string.IsNullOrEmpty(email.Content) ? ((char) 160).ToString() : email.Content;
        }
        public static string SendViaMailgun(Email email)
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
            request.AddParameter("to", email.Tos == null? email.From : string.Join(", ", email.Tos));
            if (email.Ccs != null) request.AddParameter("cc", string.Join(", ", email.Ccs));
            if (email.Bccs != null) request.AddParameter("bcc", string.Join(", ", email.Bccs));
            request.AddParameter("subject", email.Subject);
            request.AddParameter("text", email.Content);
            request.Method = Method.POST;
            RestResponse rr = (RestResponse) client.Execute(request);
            ResponseMessage rm = JsonConvert.DeserializeObject<ResponseMessage>(rr.Content);
            return rm.message;         
        }

        private static void addRecipientGroup(RecipientGroup rg, Action<RecipientGroup> createRecipientGroup, Action<RecipientGroup, string> addEmailCredential, string[] addressArray)
        {
            if (addressArray == null || addressArray.Length == 0) return;
            createRecipientGroup(rg);
            foreach(string addr in addressArray)
            {
                addEmailCredential(rg, addr);
            }
        }

        public static SendGridData CreateSendGridData(Email email)
        {
            SendGridData sgd = new SendGridData
            {
                personalizations = new List<RecipientGroup>(),
                from = new EmailCredential { email = email.From },
                subject = email.Subject,
                content = new List<EmailContent>()
            };
            RecipientGroup group = new RecipientGroup();
            sgd.personalizations.Add(group);
            addRecipientGroup(group, (rg) => { rg.to = new List<EmailCredential>(); }, (rg, addr) => { rg.to.Add(new EmailCredential { email = addr }); }, email.Tos);
            addRecipientGroup(group, (rg) => { rg.cc = new List<EmailCredential>(); }, (rg, addr) => { rg.cc.Add(new EmailCredential { email = addr }); }, email.Ccs);
            addRecipientGroup(group, (rg) => { rg.bcc = new List<EmailCredential>(); }, (rg, addr) => { rg.bcc.Add(new EmailCredential { email = addr }); }, email.Bccs);
            sgd.content.Add(new EmailContent { type = "text/plain", value = email.Content });
            return sgd;
        }
        public static string SendViaSendGrid(Email email)
        {
            var request = (HttpWebRequest)WebRequest.Create(WebConfigurationManager.AppSettings["SendGridSendUri"]);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + WebConfigurationManager.AppSettings["SendGridKey"]);

            StringBuilder resultingJson = new StringBuilder();
            var serializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            using (JsonWriter writer = new JsonTextWriter(new StringWriter(resultingJson)))
            {
                serializer.Serialize(writer, CreateSendGridData(email));
                streamWriter.Write(resultingJson);
                streamWriter.Flush();
                streamWriter.Close();
            }
            string response = string.Empty;
            
            try
            {
                var httpResponse = request.GetResponse();
                response = "Email sent successfully."; 
            }
            catch (WebException e)
            {
                HttpWebResponse hwr = (HttpWebResponse)e.Response;
                var sgr = JsonConvert.DeserializeObject<SendGridResponse>(new StreamReader(hwr.GetResponseStream()).ReadToEnd());
                List<string> messages = new List<string>();
                if (sgr.errors != null)
                {
                    foreach (ResponseMessage rm in sgr.errors)
                    {
                        messages.Add(rm.message);
                    }
                    response = string.Join("<br>", messages.ToArray());
                }
                
            }

            return response;

        }
    }
}