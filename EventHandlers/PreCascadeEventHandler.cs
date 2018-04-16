using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.EventHandler;

namespace EventsHandlersPractice
{
    [kCura.EventHandler.CustomAttributes.Description("Pre Cascade Event Handler")]
    [System.Runtime.InteropServices.Guid("1B411B12-636B-4843-BA6D-87E11787CD25")]
    class PreCascadeEventHandler : PreCascadeDeleteEventHandler
    {
        public override FieldCollection RequiredFields
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Commit()
        {
            throw new NotImplementedException();
        }

        public override Response Execute()
        {
            throw new NotImplementedException();
        }

        public override void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
