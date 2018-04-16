using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.EventHandler;
using Relativity.API;
using DTOs = kCura.Relativity.Client.DTOs;
namespace EventsHandlersPractice
{
    [kCura.EventHandler.CustomAttributes.Description("Mass Operation Event Handler Comment")]
    [System.Runtime.InteropServices.Guid("1C81FF0A-B90C-414F-AB2B-B98F2297F9E2")]
    class MassOperationEventHandler : kCura.MassOperationHandlers.MassOperationHandler
    {
        public static readonly Guid COMMENT_FIELD_GUID = new Guid("338C23DE-21B1-49C1-A8B9-78F5CD742318");
        public static readonly Guid TYPE_FIELD_GUID = new Guid("F0D53EE2-3AD2-43ED-B68F-397849A17F89");
        public static readonly Guid ERROR_TYPE_FIELD_GUID = new Guid("73DF39C3-3E97-4584-9AA4-03821A9D8AD2");
        const string ARTIFACT_TYPE = "3136AA28-7D29-4164-A928-CF2272197090";
        const string COMMENT = "338C23DE-21B1-49C1-A8B9-78F5CD742318";
        public override Response DoBatch()
        {
            Response retVal = new Response()
            {
                Success = true,
                Message = "The mass edit was start"
            };

           
            return retVal;
        }

        public override Response PostMassOperation()
        {
            Response retVal = new Response()
            {
                Success = true,
                Message = "The comment jobs were inserted with sucess"
            };

            string sqlText = $" SELECT * from [RESOURCE].[{this.MassActionTableName}]";

            System.Data.DataTable results= this.Helper.GetDBContext(this.Helper.GetActiveCaseID()).ExecuteSqlStatementAsDataTable(sqlText);

            return retVal;

        }

        public override Response PreMassOperation()
        {
            ConsoleEventHandlerComment console = new ConsoleEventHandlerComment();
            kCura.EventHandler.Response retVal = new kCura.EventHandler.Response();
            retVal.Success = true;
            retVal.Message = "Successful Pre Execute Operation method";
            IDBContext dbContext = this.Helper.GetDBContext(this.Helper.GetActiveCaseID());
            string sqlText = $" SELECT * FROM [Comment] WHERE ArtifactID IN (SELECT ARTIFACTID from[RESOURCE].[{ this.MassActionTableName}]) ";
            
            System.Data.DataRowCollection results = dbContext.ExecuteSqlStatementAsDataTable(sqlText).Rows;
            foreach (System.Data.DataRow row in results)
            {
                
                DTOs.RDO comme = new DTOs.RDO((int)row.ItemArray[0]);
                comme.ArtifactTypeGuids.Add(new Guid(ARTIFACT_TYPE));
               
                comme.Fields.Add(new DTOs.FieldValue(new Guid(COMMENT_FIELD_GUID.ToString()), row.ItemArray[1]));
                console.insertJob(dbContext, this.Helper.GetAuthenticationManager().UserInfo.FullName, comme);

               
                DTOs.Choice choice = new DTOs.Choice(ERROR_TYPE_FIELD_GUID);
                comme.Fields.Add(new DTOs.FieldValue(TYPE_FIELD_GUID, choice));

                using (kCura.Relativity.Client.IRSAPIClient client =
                      this.Helper.GetServicesManager().CreateProxy<kCura.Relativity.Client.IRSAPIClient>(Relativity.API.ExecutionIdentity.System))
                {
                    client.APIOptions.WorkspaceID = this.Helper.GetActiveCaseID();
                    client.Repositories.RDO.UpdateSingle(comme);
                }
            }
            return retVal;
        }

        public override Response ValidateLayout()
        {
            Response retVal = new Response()
            {
                Success = true,
                Message = "Success all comment jobs were inserted"
            };

            string comment = (string)this.LayoutMask.Fields[COMMENT_FIELD_GUID.ToString()].Value.Value;
            if (!string.IsNullOrEmpty(comment))
            {
                retVal.Success = false;
                retVal.Message = "This mass edit operation requires that you leave the comment field blank";
                return retVal;
            }

            ChoiceCollection choices = (ChoiceCollection)this.LayoutMask.Fields[TYPE_FIELD_GUID.ToString()].Value.Value;
            foreach (Choice choice in choices)
            {
                if (!choice.Name.Equals("Error"))
                {
                    retVal.Success = false;
                    retVal.Message = "This mass edit operation requires that type selected is Error";
                    return retVal;
                }

            }

           
            return retVal;

        }

        public override Response ValidateSelection()
        {
            Response retVal = new Response()
            {
                Success = true,
                Message = "Warnign this operation permanantly edits the selected objects. Please ve certain if you wish continue"
            };

            int activeWorkspaceId = this.Helper.GetActiveCaseID();
            string sqlText = $" SELECT COUNT(*) from [RESOURCE].[{this.MassActionTableName}]";
            int count = (int)this.Helper.GetDBContext(activeWorkspaceId).ExecuteSqlStatementAsScalar(sqlText);
            if(!(count % 2 == 0))
            {
                retVal.Success = false;
                retVal.Message = "The mass operation requires an even number of items to fuction";
            }
            
            return retVal;
        }
    }
    
}
