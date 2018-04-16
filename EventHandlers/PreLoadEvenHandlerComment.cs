using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.EventHandler;
using Relativity.API;

namespace EventsHandlersPractice
{
    [kCura.EventHandler.CustomAttributes.Description("Cooment Pre Load Event Handler")]
    [System.Runtime.InteropServices.Guid("B1802094-85EB-4C16-9363-26828DA08BAD")]
    public class PreLoadEvenHandler : PreLoadEventHandler
    {
        private string defaultUser = "DEFAULT_USER";
        private DateTime defaultDate = DateTime.Now.Date;
        public static readonly Guid CREATED_BY_FIELD_GUID = new Guid("4126CECF-1FA1-4FA1-A306-803013DE3B24");
        public static readonly Guid CREATED_ON_FIELD_GUID = new Guid("C764240D-47DB-435B-A929-F4DB9CC1DFB0");

        private IAPILog _logger;

        public override Response Execute()
        {
            Response retVal = new Response
            {
                Success = true,
                Message = string.Empty
            };

            _logger = Helper.GetLoggerFactory().GetLogger().ForContext<PreLoadEvenHandler>();

            try
            {
                defaultUser = $"This comment will created by this user: {Helper.GetAuthenticationManager().UserInfo.FullName}";
                if (ActiveArtifact.IsNew)
                {
                    _logger.LogError("Enter in the if sentece");
                    ActiveArtifact.Fields[CREATED_BY_FIELD_GUID.ToString()].Value.Value = defaultUser;
                    ActiveArtifact.Fields[CREATED_ON_FIELD_GUID.ToString()].Value.Value = defaultDate;
                }
                else
                {
                    _logger.LogError("do not enter in the if sentence");
                    ActiveArtifact.Fields[CREATED_BY_FIELD_GUID.ToString()].Value.Value = $"This comment was created by: {ActiveArtifact.Fields[CREATED_BY_FIELD_GUID.ToString()].Value.Value}";
                }

            }
            catch (Exception e)
            {
                retVal.Success = false;
                retVal.Message = e.ToString();
            }
            return retVal;
        }

        public override FieldCollection RequiredFields
        {
            get
            {
                FieldCollection retVal = new FieldCollection();
                retVal.Add(new Field(CREATED_BY_FIELD_GUID));
                retVal.Add(new Field(CREATED_ON_FIELD_GUID));
                return retVal;
            }
        }
    }
}
