using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelativityAppCore.DAL.Entities
{
    public class Folder:Artifact
    {
        public int ParentId { get; set; }
    }
}
