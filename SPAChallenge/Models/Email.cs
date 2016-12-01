using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPAChallenge.Models
{
    public class Email
    {
        public string[] Tos { get; set; }
        public string[] Ccs { get; set; }
        public string[] Bccs { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}