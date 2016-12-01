using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPAChallenge.Models
{
    
    public class SendGridResponse
    {
        public List<ResponseMessage> errors { get; set; }
    }
}