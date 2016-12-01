using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Web.Configuration;
using SPAChallenge.Models;

namespace SPAChallenge.Services
{
    public class SendGridServer : EmailServer
    {
        public override bool PingServer()
        {
            int pingTimeout = Int32.Parse(WebConfigurationManager.AppSettings["DefaultPingTimeout"]);
            return EmailServices.PingServer(WebConfigurationManager.AppSettings["SendGridHostname"], pingTimeout, 1);
        }

        private void addRecipientGroup(RecipientGroup rg, Action<RecipientGroup> createRecipientGroup, Action<RecipientGroup, string> addEmailCredential, string[] addressArray)
        {
            if (addressArray == null || addressArray.Length == 0) return;
            createRecipientGroup(rg);
            foreach (string addr in addressArray)
            {
                addEmailCredential(rg, addr);
            }
        }

        private SendGridData CreateSendGridData(Email email)
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

        public override string SendMail(Email email)
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