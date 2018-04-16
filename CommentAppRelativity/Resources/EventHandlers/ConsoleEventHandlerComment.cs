using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.EventHandler;
using Relativity.API;
using DTOs = kCura.Relativity.Client.DTOs;
using System.Data.SqlClient;
using Service = RelativityAppCore.BLL.Service;
using Data = RelativityAppCore.DAL;
using System.Data;

namespace EventsHandlersPractice
{
    [kCura.EventHandler.CustomAttributes.Description("Comment Console Event Handler")]
    [System.Runtime.InteropServices.Guid("072727A5-C0BB-4F89-A579-8A89D06E47F4")]
    class ConsoleEventHandlerComment : ConsoleEventHandler
    {
        private static readonly Guid COMMENT_HISTORY_APPLICATION_GUID = new Guid("B9C4168D-A204-4478-B15B-89110F7ABEBB");
        public static readonly Guid COMMENT_FIELD_GUID = new Guid("338C23DE-21B1-49C1-A8B9-78F5CD742318");

        private const String CONSOLE_TITLE = "Process Comment Console";

        private const String INSERT_JOB_BUTTON_NAME = "_insertJobButton";
        private const String INSERT_JOB_DISPLAY_TEXT = "Comment Job";
        private const String INSERT_JOB_TOOL_TIP = "Insert a Comment Job";

        private const String DELETE_JOB_BUTTON_NAME = "_deleteJobButton";
        private const String DELETE_JOB_DISPLAY_TEXT = "Delete Job";
        private const String DELETE_JOB_TOOL_TIP = "Delete a Comment Job";

        private const String JOB_EXISTS_QUERY = "SELECT COUNT(*) FROM [EDDSDBO].commentJob where comment_artifactid = @commentArtifacId";

       

        public override kCura.EventHandler.Console GetConsole(PageEvent pageEvent)
        {
            int activeWorkspaceId = this.Helper.GetActiveCaseID();

            //Construct a console object to build the console appearing in the UI.
            kCura.EventHandler.Console returnConsole = new kCura.EventHandler.Console();
            returnConsole.Items = new List<IConsoleItem>();
            returnConsole.Title = CONSOLE_TITLE;
            string select = "<h3 style='color:#11599E'>Comments Tree</h3>";
            
            List<string> elements = new List<string>();
            elements.Add(select);
            using (kCura.Relativity.Client.IRSAPIClient client =
                          this.Helper.GetServicesManager().CreateProxy<kCura.Relativity.Client.IRSAPIClient>(Relativity.API.ExecutionIdentity.System))
            {
                client.APIOptions.WorkspaceID = this.Helper.GetActiveCaseID();
                Service.SqlService.CommentSqlService commentService = new Service.SqlService.CommentSqlService(this.Helper.GetDBContext(this.Helper.GetActiveCaseID()));
                Service.RSAPIService.CommentRSAPIService commentRSAPIService = new Service.RSAPIService.CommentRSAPIService(client);
                Data.Entities.Comment comment = commentRSAPIService.Get(this.ActiveArtifact.ArtifactID);
                comment.CommentChilds = commentService.GetCommentsChild(comment.ArtifactId);
                drawCommentTree2(ref elements, (comment.CommentChilds).ToList());
                returnConsole.HTMLBlocks = elements;
            }
            
            ConsoleHeader header = new ConsoleHeader("Console Application");

            //Construct the submit job button.
            ConsoleButton submitJobButton = new ConsoleButton();
            submitJobButton.Name = INSERT_JOB_BUTTON_NAME;
            submitJobButton.DisplayText = INSERT_JOB_DISPLAY_TEXT;
            submitJobButton.ToolTip = INSERT_JOB_TOOL_TIP;
            submitJobButton.RaisesPostBack = true;
            submitJobButton.Enabled = true;

            //Construct the delete job button
            ConsoleButton deleteJobButton = new ConsoleButton()
            {
                Name = DELETE_JOB_BUTTON_NAME,
                DisplayText = DELETE_JOB_DISPLAY_TEXT,
                ToolTip = DELETE_JOB_TOOL_TIP,
                RaisesPostBack = true,
                Enabled = true

            };

            //Button to see the comment data
            ConsoleButton seeCommentButton = new ConsoleButton()
            {
                Name = "See Comment Data",
                DisplayText = "Commen Data",
                ToolTip = "Comment Data",
                RaisesPostBack = true,
                Enabled = true
            };


            ConsoleSeparator separador = new ConsoleSeparator();


            //If a job is already in the queue, change the text and disable the button.
            if (pageEvent == PageEvent.PreRender)
            {

                SqlParameter commentArtifactId = new SqlParameter("@commentArtifacId", System.Data.SqlDbType.Int);
                commentArtifactId.Value = ActiveArtifact.ArtifactID;

                int jobCount = this.Helper.GetDBContext(activeWorkspaceId).ExecuteSqlStatementAsScalar<Int32>(JOB_EXISTS_QUERY, new SqlParameter[] { commentArtifactId });

                //Use the helper function to check if a job currently exists. Set Enabled to the opposite value.
                if (jobCount > 0)
                {
                    submitJobButton.Enabled = false;
                    deleteJobButton.Enabled = true;

                }
                else
                {
                    submitJobButton.Enabled = true;
                    deleteJobButton.Enabled = false;
                }

                //Get the base path to the application.
                String basePath = this.Application.ApplicationUrl.Substring(0, this.Application.ApplicationUrl.IndexOf("/Case/Mask/"));

                //Construct the path to the custom page with the current patient artifact id and current workspace.
                String patientProfilePageUrl = String.Format("{0}/CustomPages/{1}/Home/Index/?artifacId={2}", basePath, COMMENT_HISTORY_APPLICATION_GUID, ActiveArtifact.ArtifactID);

                //Create the JavaScript for the button and set the button property.
                String windowOpenJavaScript = String.Format("window.open('{0}', '', 'location=no,scrollbars=yes,menubar=no,toolbar=no,status=no,resizable=yes,width=300,height=400');", patientProfilePageUrl);
                seeCommentButton.OnClickEvent = windowOpenJavaScript;
            }


            //Add the buttons to the console.
            returnConsole.Items.Add(header);
            returnConsole.Items.Add(submitJobButton);
            returnConsole.Items.Add(deleteJobButton);
            returnConsole.Items.Add(seeCommentButton);
            returnConsole.Items.Add(separador);
            return returnConsole;
        }

        public override void OnButtonClick(ConsoleButton consoleButton)
        {
            bool result = true;
            int activeWorkspaceId = this.Helper.GetActiveCaseID();
            //Use the name to determine which button was clicked. 
            switch (consoleButton.Name)
            {
                case INSERT_JOB_BUTTON_NAME:
                    //The user clicked the button for the insert job so add the job to the queue table on the EDDS database.
                    result = insertJob(this.Helper.GetDBContext(activeWorkspaceId), this.Helper.GetAuthenticationManager().UserInfo.FullName, this.ActiveArtifact);

                    break;

                case DELETE_JOB_BUTTON_NAME:

                    result = deleteJob(this.Helper.GetDBContext(activeWorkspaceId), this.ActiveArtifact.ArtifactID);

                    break;
            }
        }

        public bool insertJob(IDBContext dbContext, string currentUser, Artifact activeArtifact)
        {
            bool result = true;
            DateTime date = DateTime.Now;
            string INSERT_JOB_QUERY = $@"IF NOT EXISTS(SELECT TOP 1 * FROM [EDDSDBO].commentJob where comment_artifactid = @commentArtifactId)
                                      BEGIN
                                      insert into [EDDSDBO].commentJob (cojob_comment, cojob_createdBy, comment_artifactId, cojob_createdOn)
                                      Values (@comment, @user, @commentArtifactId, @createdOn )
                                      END";

            System.Data.SqlClient.SqlParameter comment = new System.Data.SqlClient.SqlParameter("@comment", System.Data.SqlDbType.VarChar);
            comment.Value = activeArtifact.Fields[COMMENT_FIELD_GUID.ToString()].Value.Value;
            System.Data.SqlClient.SqlParameter user = new System.Data.SqlClient.SqlParameter("@user", System.Data.SqlDbType.VarChar);
            user.Value = currentUser;
            System.Data.SqlClient.SqlParameter commentArtifactId = new System.Data.SqlClient.SqlParameter("@commentArtifactId", System.Data.SqlDbType.Int);
            commentArtifactId.Value = activeArtifact.ArtifactID;
            System.Data.SqlClient.SqlParameter createdOn = new System.Data.SqlClient.SqlParameter("@createdOn", System.Data.SqlDbType.DateTime);
            createdOn.Value = DateTime.Now;



            try
            {
                dbContext.ExecuteNonQuerySQLStatement(INSERT_JOB_QUERY, new System.Data.SqlClient.SqlParameter[] { commentArtifactId, comment, user, createdOn });
                result = true;
            }
            catch (Exception e)
            {

                System.Console.WriteLine($"There was a problem in a Query: {e.Message}");
                result = false;
            }

            return result;
        }
        public bool insertJob(IDBContext dbContext, string currentUser, DTOs.RDO rdo)
        {
            bool result = true;
            DateTime date = DateTime.Now;
            string INSERT_JOB_QUERY = $@"IF NOT EXISTS(SELECT TOP 1 * FROM [EDDSDBO].commentJob where comment_artifactid = @commentArtifactId)
                                      BEGIN
                                      insert into [EDDSDBO].commentJob (cojob_comment, cojob_createdBy, comment_artifactId, cojob_createdOn)
                                      Values (@comment, @user, @commentArtifactId, @createdOn )
                                      END";

            System.Data.SqlClient.SqlParameter comment = new System.Data.SqlClient.SqlParameter("@comment", System.Data.SqlDbType.VarChar);
            comment.Value = rdo.Fields[0].Value;
            System.Data.SqlClient.SqlParameter user = new System.Data.SqlClient.SqlParameter("@user", System.Data.SqlDbType.VarChar);
            user.Value = currentUser;
            System.Data.SqlClient.SqlParameter commentArtifactId = new System.Data.SqlClient.SqlParameter("@commentArtifactId", System.Data.SqlDbType.Int);
            commentArtifactId.Value = rdo.ArtifactID;
            System.Data.SqlClient.SqlParameter createdOn = new System.Data.SqlClient.SqlParameter("@createdOn", System.Data.SqlDbType.DateTime);
            createdOn.Value = DateTime.Now;



            try
            {
                dbContext.ExecuteNonQuerySQLStatement(INSERT_JOB_QUERY, new System.Data.SqlClient.SqlParameter[] { commentArtifactId, comment, user, createdOn });
                result = true;
            }
            catch (Exception e)
            {

                System.Console.WriteLine($"There was a problem in a Query: {e.Message}");
                result = false;
            }

            return result;
        }

        public DataRowCollection showCommentChilds (IDBContext dbcontext, int parentCommentId)
        {
           
            string comment_child_query = $@"SELECT  [ArtifactID]
                                                    FROM [{dbcontext.Database}].[EDDSDBO].[Comment]
                                                    where RelatedComments = {parentCommentId}";
            DataRowCollection  data = dbcontext.ExecuteSqlStatementAsDataTable(comment_child_query).Rows;
            
            return data;
        }
        public bool deleteJob(IDBContext dbcontext, int artifactId)
        {
            bool result = true;
            string DELETE_JOB_QUERY = "IF EXISTS(SELECT TOP 1 * FROM [EDDSDBO].commentJob where comment_artifactid = @artifactId)"
                + " BEGIN"
                + " DELETE from [EDDSDBO].commentJob"
                + " WHERE comment_artifactId = @artifactId"
                + " END";

            System.Data.SqlClient.SqlParameter artifact = new System.Data.SqlClient.SqlParameter("@artifactId", System.Data.SqlDbType.Int);
            artifact.Value = artifactId;

            try
            {
                dbcontext.ExecuteNonQuerySQLStatement(DELETE_JOB_QUERY, new SqlParameter[] { artifact });
            }
            catch (Exception)
            {

                throw;
            }

            return result;

        }

        public void  drawCommentTree(ref List<string> tree, DataRowCollection commentChilds, IDBContext context)
        {
            
            if (commentChilds.Count.Equals(0))
            {
                tree.Add("<ul style='padding:0 0 0 20px'>|<li style='font-size:smaller;list-style:none;padding:0 0 0 0'>|-- Without Childs</li></ul>");
            }
            else
            {
                tree.Add("<ul  style='padding:0 0 0 20px'>|");

                foreach (DataRow item in commentChilds)
                {
                    foreach (var id in item.ItemArray)
                    {
                        tree.Add($"<li style='list-style:none;padding:0 0 0 0px'><a  target='_blank' href='http://192.168.0.148/Relativity/Case/Mask/View.aspx?AppID=1034680&ArtifactID={id}&ArtifactTypeID=1001041&SelectedTab=null'>|-- {id}</a></li>");
                        
                        drawCommentTree(ref tree, showCommentChilds(context, (int)id),context);
                    }
                }
               
                tree.Add("</ul>");
            }
        }

        public void drawCommentTree2(ref List<string> tree, List<Data.Entities.Comment> commentChilds)
        {

            if (commentChilds.Count.Equals(0))
            {
                tree.Add("<ul style='padding:0 0 0 20px'>|<li style='font-size:smaller;list-style:none;padding:0 0 0 0'>|-- Without Childs</li></ul>");
            }
            else
            {
                tree.Add("<ul  style='padding:0 0 0 20px'>|");

                foreach (Data.Entities.Comment coment in commentChilds)
                {
                        Service.SqlService.CommentSqlService commentService = new Service.SqlService.CommentSqlService(this.Helper.GetDBContext(this.Helper.GetActiveCaseID()));
                        coment.CommentChilds = commentService.GetCommentsChild(coment.ArtifactId);
                        tree.Add($"<li style='list-style:none;padding:0 0 0 0px'><a  target='_blank' href='http://192.168.0.148/Relativity/Case/Mask/View.aspx?AppID=1034680&ArtifactID={coment.ArtifactId}&ArtifactTypeID=1001041&SelectedTab=null'>|-- {coment.ArtifactId}</a></li>");

                        drawCommentTree2(ref tree, (coment.CommentChilds).ToList());
                    
                }

                tree.Add("</ul>");
            }
        }


        public override FieldCollection RequiredFields
        {
            get
            {
                FieldCollection retVal = new kCura.EventHandler.FieldCollection();
                return retVal;
            }
        }
    }
}
