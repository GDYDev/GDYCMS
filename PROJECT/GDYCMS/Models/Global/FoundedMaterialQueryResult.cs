using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDYCMS.Models.Global
{
    /// <summary>
    /// Data result of query to database
    /// </summary>
    /// This is a container. It used for read and write operaions
    public class FoundedMaterialQueryResult
    {
        /// <summary>
        /// Result type: object not found, object is not published, object is published.
        /// </summary>
        public enum QueryResults {NotFound,NotPublished,Published }
        /// Object id (for example- material id in database)
        public int? id { get; set; }
        /// Is authenticated or user create this query or not ?
        public bool IsAuthenticated { get; set; }
        /// HTML container
        public string Container { get; set; }
        /// Result
        public QueryResults Result { get; set; } 
        public FoundedMaterialQueryResult() { 
            this.Result = QueryResults.NotFound;
        }
    }
}