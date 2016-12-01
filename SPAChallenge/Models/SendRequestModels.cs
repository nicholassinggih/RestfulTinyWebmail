using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPAChallenge.Models
{
    public class SendRequestModels
    {
        public string Tos { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}