using System;
using kCura.EventHandler;
using Relativity.API;
using System.Net.Mail;
using System.Net;
using CORE = RelativityAppCore;

namespace EventsHandlersPractice
{
    [kCura.EventHandler.CustomAttributes.Description("Post Save Event Handler Comment")]
    [System.Runtime.InteropServices.Guid("7F9F8FCE-BF6F-4030-A307-4D794B17E1C4")]
    public class PostSaveEventHandlerComment : PostSaveEventHandler
    {

        public static readonly Guid COMMENT_TYPE_FIELD_GUID = new Guid("F0D53EE2-3AD2-43ED-B68F-397849A17F89");
        public static readonly Guid COMMENT_FIELD_GUID = new Guid("338C23DE-21B1-49C1-A8B9-78F5CD742318");
        public static readonly Guid COMMENT_LAST_MODIFIED_GUID = new Guid("A4E0C8F3-7872-4DC8-8D4C-8F402B5FEA5E");
        public static readonly Guid RELATED_COMMENT_FIELD = new Guid("8A9383C2-DE31-4B99-B31C-32FC4EA560EA");
        public override Response Execute()
        {
            Artifact activeArtifact = this.ActiveArtifact;
            int activeWorkspaceId = this.Helper.GetActiveCaseID();
            string currentUser = this.Helper.GetAuthenticationManager().UserInfo.FullName;
            IDBContext dbcontext = this.Helper.GetDBContext(activeWorkspaceId);
            bool result = true;
            ConsoleEventHandlerComment consoleEventHandler = new ConsoleEventHandlerComment();
            Response retVal = new Response()
            {
                Success = true,
                Message = String.Empty
            };
            //verify if the comment has a parent 
            if (!(ActiveArtifact.Fields[RELATED_COMMENT_FIELD.ToString()] == null))
            {
                int parentCommentId = (int)ActiveArtifact.Fields[RELATED_COMMENT_FIELD.ToString()].Value.Value;
                kCura.Relativity.Client.DTOs.RDO parentComment = new kCura.Relativity.Client.DTOs.RDO(parentCommentId);
                using (kCura.Relativity.Client.IRSAPIClient client =
                          this.Helper.GetServicesManager().CreateProxy<kCura.Relativity.Client.IRSAPIClient>(ExecutionIdentity.System))
                {
                    int workspaceId= this.Helper.GetActiveCaseID();
                    client.APIOptions.WorkspaceID = workspaceId;
                    parentComment = client.Repositories.RDO.ReadSingle(parentCommentId);

                    client.APIOptions.WorkspaceID = -1;
                    kCura.Relativity.Client.DTOs.User userComment =  new kCura.Relativity.Client.DTOs.User(parentComment.SystemCreatedBy.ArtifactID);
                    userComment = client.Repositories.User.ReadSingle(parentComment.SystemCreatedBy.ArtifactID);
                    

                    if (ActiveArtifact.IsNew)
                    {
                        MailMessage email = new MailMessage();
                        string userEmail = userComment.EmailAddress;
                        string author = this.Helper.GetAuthenticationManager().UserInfo.FullName;
                        email.To.Add(new MailAddress(userEmail));
                       // sentEmailNew(email, author);
                    }
                }

            }
            else
            {
                using (kCura.Relativity.Client.IRSAPIClient client =
                          this.Helper.GetServicesManager().CreateProxy<kCura.Relativity.Client.IRSAPIClient>(ExecutionIdentity.System))
                {
                    string type =string.Empty;
                    int workspaceId = this.Helper.GetActiveCaseID();
                    client.APIOptions.WorkspaceID = workspaceId;
                    CORE.BLL.Service.RSAPIService.CommentRSAPIService commentRSAPIService = new CORE.BLL.Service.RSAPIService.CommentRSAPIService(client);
                    CORE.DAL.Entities.Comment comment = commentRSAPIService.Get(ActiveArtifact.ArtifactID);
                    ChoiceCollection typeChoices = (ChoiceCollection)this.ActiveArtifact.Fields[COMMENT_TYPE_FIELD_GUID.ToString()].Value.Value;
                    foreach (Choice typeC in typeChoices)
                    {
                        type = typeC.Name;
                    }
                    comment.TypeChoosed = type;
                    auditComment(comment, ActiveArtifact.IsNew, this.Helper.GetDBContext(workspaceId));
                   
                }
            }
            

            try
            {
                ChoiceCollection typeChoices = (ChoiceCollection)this.ActiveArtifact.Fields[COMMENT_TYPE_FIELD_GUID.ToString()].Value.Value;

                foreach (Choice typeChoice in typeChoices)
                {
                    if (typeChoice.Name.Equals("Error"))
                    {
                        result =consoleEventHandler.insertJob(dbcontext, currentUser, activeArtifact);
                    }
                    else
                    {
                        result = consoleEventHandler.deleteJob(dbcontext, activeArtifact.ArtifactID);
                    }
                }

            }
            catch (Exception e)
            {

                retVal.Success = false;
                retVal.Message = e.Message;
            }
            return retVal;

        }


        public override FieldCollection RequiredFields
        {
            get
            {
                FieldCollection retVal = new FieldCollection();
                retVal.Add(new Field(COMMENT_TYPE_FIELD_GUID));
                retVal.Add(new Field(COMMENT_FIELD_GUID));
                retVal.Add(new Field(COMMENT_LAST_MODIFIED_GUID));
                return retVal;
            }
        }

      public void sentEmailNew(MailMessage email, string author)
        {
            email.From = new MailAddress("guillermoguerrero1226@gmail.com");
            email.Subject = "New Comment";
            email.Body = $"You have a new comment by {author}";
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("guillermoguerrero1226@gmail.com", "--------");

            try
            {
                smtp.Send(email);
                email.Dispose();
                //output = "Corre electrónico fue enviado satisfactoriamente.";
            }
            catch (Exception ex)
            {
                //output = "Error enviando correo electrónico: " + ex.Message;
            }
        }

        public void auditComment(CORE.DAL.Entities.Comment comment, bool isNew, IDBContext dbContext)
        {
            string sql = string.Empty;
            int value = 0;
            if (isNew)
            {
                sql = $@"INSERT INTO [EDDSDBO].[AuditComment]
                           ([CommentId]
                           ,[CreatedOn]
                           ,[CreateByUserId]
                           ,[CreatedByUserName]
                           ,[ReplysAmount]
                           ,[comment]
                           ,[type])
                        VALUES
                           ({comment.ArtifactId},
		                   GETDATE(),
		                   {comment.CreatedBy.ArtifactId},
		                   '{comment.CreatedBy.Name}',
		                   0,
		                   '{comment.Name}',
		                   '{comment.TypeChoosed}'
		                   )";
            }
            else
            {
                sql = $@"INSERT INTO [EDDSDBO].[AuditComment]
                           ([CommentId]
                           ,[ModifiedOn]
                           ,[ModifiedByUserId]
                           ,[ModifiedByUserName]
                           ,[ReplysAmount]
                           ,[comment]
                           ,[type])
                        VALUES
                           ({comment.ArtifactId},
		                   GETDATE(),
		                   {comment.CreatedBy.ArtifactId},
		                   '{comment.CreatedBy.Name}',
		                   0,
		                   '{comment.Name}',
		                   '{comment.TypeChoosed}'
		                   )";
            }

            value = dbContext.ExecuteNonQuerySQLStatement(sql);

        }
        

    }

}



