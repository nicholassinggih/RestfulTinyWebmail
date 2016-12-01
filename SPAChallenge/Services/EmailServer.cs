using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPAChallenge.Models
{
    public abstract class EmailServer
    {
        abstract public bool PingServer();
        abstract public string SendMail(Email email);
    }
}