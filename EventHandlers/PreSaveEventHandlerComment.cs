using System;
using kCura.EventHandler;
using Relativity.API;
using System.Data;
using Service = RelativityAppCore.BLL.Service;
using Data = RelativityAppCore.DAL;
using System.Collections.Generic;

namespace EventsHandlersPractice.PreSaveEventsHandler
{
    [kCura.EventHandler.CustomAttributes.Description("Comment Pre Save")]
    [System.Runtime.InteropServices.Guid("82D03346-6526-4D3A-B870-76AE838A72B4")]
    public class PreSaveEventHandlerComment : PreSaveEventHandler
    {
        public static readonly Guid COMMENT_FIEL_GUID = new Guid("338C23DE-21B1-49C1-A8B9-78F5CD742318");
        public static readonly Guid TYPE_FIELD_GUID = new Guid("F0D53EE2-3AD2-43ED-B68F-397849A17F89");
        public static readonly Guid SYSTEM_CREATED_BY_FIELD = new Guid("4126CECF-1FA1-4FA1-A306-803013DE3B24");
        public static readonly Guid RELATED_COMMENT_FIELD = new Guid("8A9383C2-DE31-4B99-B31C-32FC4EA560EA");
        private IAPILog _logger;

        public override Response Execute()
        {
            _logger = this.Helper.GetLoggerFactory().GetLogger().ForContext<PreSaveEventHandlerComment>();
            Response retVal = new Response();
            retVal.Success = true;
            retVal.Message = string.Empty;

            

           // Console.WriteLine(output);

            //string output = null;
            try
            {
                String comment = (String)this.ActiveArtifact.Fields[COMMENT_FIEL_GUID.ToString()].Value.Value;
                string user = (String)this.ActiveArtifact.Fields[SYSTEM_CREATED_BY_FIELD.ToString()].Value.Value;


                

                if (!(ActiveArtifact.Fields[RELATED_COMMENT_FIELD.ToString()]==null))
                {

                   

                    using (kCura.Relativity.Client.IRSAPIClient client =
                          this.Helper.GetServicesManager().CreateProxy<kCura.Relativity.Client.IRSAPIClient>(Relativity.API.ExecutionIdentity.System))
                    {
                        client.APIOptions.WorkspaceID = this.Helper.GetActiveCaseID();
                        Service.RSAPIService.CommentRSAPIService commentRSAPIService = new Service.RSAPIService.CommentRSAPIService(client);
                        Service.SqlService.CommentSqlService commentSqlService = new Service.SqlService.CommentSqlService(this.Helper.GetDBContext(Helper.GetActiveCaseID()));
                        int parentCommentId = (int)this.ActiveArtifact.Fields[RELATED_COMMENT_FIELD.ToString()].Value.Value;
                        Data.Entities.Comment parentComment = new Data.Entities.Comment(parentCommentId);
                        parentComment = commentRSAPIService.Get(parentCommentId);
                        List<Data.Entities.Comment> commentsChild = commentSqlService.GetCommentsChild(parentComment.ArtifactId);
                        parentComment.CommentChilds = commentsChild;
                        int user1 = parentComment.CreatedBy.ArtifactId;
                        int user2 = this.Helper.GetAuthenticationManager().UserInfo.ArtifactID;

                        if (commentsChild.Count.Equals(0))
                        {
                            if (user1.Equals(user2))
                            {
                                throw new StartConversation();
                            }
                        }

                       
                    }
                }

                
                
                


                if (String.IsNullOrWhiteSpace(comment))
                {
                    _logger.LogError($"the comment field was not fill up ");
                    throw new FieldMissingException("Comment");
                }
                if (!ActiveArtifact.IsNew)
                {


                    using (kCura.Relativity.Client.IRSAPIClient client =
                              this.Helper.GetServicesManager().CreateProxy<kCura.Relativity.Client.IRSAPIClient>(ExecutionIdentity.System))
                    {
                        client.APIOptions.WorkspaceID = this.Helper.GetActiveCaseID();
                        Service.RSAPIService.CommentRSAPIService commentRSAPIService = new Service.RSAPIService.CommentRSAPIService(client);
                        Data.Entities.Comment currentComment = new Data.Entities.Comment(ActiveArtifact.ArtifactID);
                        currentComment = commentRSAPIService.Get(ActiveArtifact.ArtifactID);
                        int commentUserId = currentComment.CreatedBy.ArtifactId;
                        int currentUserId = this.Helper.GetAuthenticationManager().UserInfo.ArtifactID;

                        if (!commentUserId.Equals(currentUserId))
                            throw new DontEditAnotherComment();
                    }

                }


                Boolean typeSelected = false;
                ChoiceCollection typeField = (ChoiceCollection)this.ActiveArtifact.Fields[TYPE_FIELD_GUID.ToString()].Value.Value;

                foreach (Choice typeChoice in typeField)
                {
                    if (typeChoice.IsSelected)
                    {
                        typeSelected = true;
                        break;
                    }
                }

                if (!typeSelected)
                {
                    throw new FieldMissingException("Comment Type");
                }

            }
            catch (FieldMissingException fielMissingEx)
            {
                retVal.Success = false;
                retVal.Message = fielMissingEx.Message;
            }


            catch (Exception e)
            {
                retVal.Success = false;
                retVal.Message = e.Message;
            }
            return retVal;
        }
        public DataRowCollection showCommentChilds(IDBContext dbcontext, int parentCommentId)
        {

            string comment_child_query = $@"SELECT  [ArtifactID]
                                                    FROM [{dbcontext.Database}].[EDDSDBO].[Comment]
                                                    where RelatedComments = {parentCommentId}";
            DataRowCollection data = dbcontext.ExecuteSqlStatementAsDataTable(comment_child_query).Rows;

            return data;
        }


        public override FieldCollection RequiredFields
        {
            get
            {
                FieldCollection retVal = new FieldCollection();
                retVal.Add(new Field(COMMENT_FIEL_GUID));
                retVal.Add(new Field(TYPE_FIELD_GUID));
                return retVal;
            }
        }

    }

    public class FieldMissingException : Exception
    {
        public FieldMissingException(String missinField) :
            base($"You must fill in the following field: {missinField}")
        {

        }
    }

    public class StartConversation : Exception
    {
        public StartConversation() :
            base($"You can't start a conversation in your own comment")
        {

        }
    }

    public class DontEditAnotherComment : Exception
    {
        public DontEditAnotherComment() :
            base($"You can't edit a comment created by another user")
        {

        }
    }
    
}
