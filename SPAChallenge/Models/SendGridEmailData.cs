using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPAChallenge.Models
{
    public class SendGridData
    {
        public List<RecipientGroup> personalizations { get; set; }
        public EmailCredential from { get; set; }
        public string subject { get; set; }
        public List<EmailContent> content { get; set; }
    }
}