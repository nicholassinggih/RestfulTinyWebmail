using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPAChallenge.Models
{
    public class RecipientGroup
    {
        public List<EmailCredential> to { get; set; }
        public List<EmailCredential> cc { get; set; }
        public List<EmailCredential> bcc { get; set; }
    }
}