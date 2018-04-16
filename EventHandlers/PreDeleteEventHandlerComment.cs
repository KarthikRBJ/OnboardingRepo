using System;
using kCura.EventHandler;

namespace EventsHandlersPractice
{

    [kCura.EventHandler.CustomAttributes.Description("Pre Delete Event Handler Comment")]
    [System.Runtime.InteropServices.Guid("359027C2-2C7D-4360-9AE3-9DE79373B55A")]

    public class PreDeleteEventHandlerComment : PreDeleteEventHandler
    {

        public static readonly Guid SYSTEM_CREATED_BY_FIELD = new Guid("4126CECF-1FA1-4FA1-A306-803013DE3B24");
        private const String DELETE_JOBS_COMMENTS_LINKED = @"DELETE from [EDDSDBO].commentJob where comment_artifactId = @artifacId";
        public override void Commit()
        {
        }

        public override Response Execute()
        {
            Response eventRes = new Response()
            {
                Success = true,
                Message = String.Empty
            };
            try
            {
                System.Data.SqlClient.SqlParameter artifactId = new System.Data.SqlClient.SqlParameter("@artifacId", System.Data.SqlDbType.Int);
                artifactId.Value = this.ActiveArtifact.ArtifactID;
                int activeWorkspaceId = this.Helper.GetActiveCaseID();
                this.Helper.GetDBContext(activeWorkspaceId).ExecuteNonQuerySQLStatement(DELETE_JOBS_COMMENTS_LINKED, new System.Data.SqlClient.SqlParameter[] { artifactId });
                int currentUserId = this.Helper.GetAuthenticationManager().UserInfo.ArtifactID;

                using (kCura.Relativity.Client.IRSAPIClient client =
                          this.Helper.GetServicesManager().CreateProxy<kCura.Relativity.Client.IRSAPIClient>(Relativity.API.ExecutionIdentity.System))
                {
                    client.APIOptions.WorkspaceID = this.Helper.GetActiveCaseID();
                    kCura.Relativity.Client.DTOs.RDO comment = client.Repositories.RDO.ReadSingle(ActiveArtifact.ArtifactID);
                    int commentUserId = comment.SystemCreatedBy.ArtifactID;

                    if (!currentUserId.Equals(commentUserId))
                    {
                        throw new DeleteAnotherComment();
                    }

                }
            }
            catch (Exception ex)
            {
                eventRes.Success = false;
                eventRes.Exception = new SystemException("ProcessPreDeleteFailure failure: "
                    + ex.Message);
            }


            return eventRes;
        }

        public override void Rollback()
        {
        }

        public override FieldCollection RequiredFields
        {
            get
            {
                return null;
            }
        }
    }
    public class DeleteAnotherComment : Exception
    {
        public DeleteAnotherComment() :
            base($"You can't delete a comment created by another user")
        {

        }
    }
}
