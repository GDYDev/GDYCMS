using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
namespace GDYCMS.Models
{
    public class StatisticsModel
    {
        public struct Visitor{
            public string Date;
            public string BrowserInfo;
            public string IP;
        }
        public List<Visitor> Visitors { get; set; }

        public string Action { get; set; } 
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public StatisticsModel() {
            Visitors = new List<Visitor>();
        }
        
    }
}