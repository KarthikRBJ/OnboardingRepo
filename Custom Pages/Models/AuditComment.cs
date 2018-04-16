using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Custom_Pages.Models
{
    public class AuditComment
    {
        public int commentId { get; set; }
        public string createdOn { get; set; }
        public int createByUserId { get; set; }
        public string createdByUserName { get; set; }
        public string modifiedOn { get; set; }
        public int modifiedByUserId { get; set; }
        public string modifiedByUserName { get; set; }
        public int replysAmount { get; set; }
        public string comment { get; set; }
        public string  type { get; set; }
    }
}