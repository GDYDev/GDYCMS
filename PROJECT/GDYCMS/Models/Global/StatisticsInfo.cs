using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDYCMS.Models.Global
{
    /// <summary>
    /// This is a statistics class
    /// </summary>
    public sealed class StatisticsInfo
    {
        /// <summary>
        /// This is used for unique user detection.
        /// Controller checks it's value, and when variable is false, controoler set unique visitor information in database and set's this flag to true;
        /// </summary>
        public bool isCapturedUser { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public StatisticsInfo() {
            TimeStamp = DateTime.Now; // Register TimeStamp
            isCapturedUser = true;
        }
    }
}