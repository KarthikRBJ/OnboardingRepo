using CommentPageCore.Entities;
using kCura.Relativity.Client;
using Newtonsoft.Json;
using Relativity.API;
using Relativity.Services.Group;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.Permission;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CORE = RelativityAppCore;

namespace CommentPageCore.Managers
{
    public class ObjectManagerRepository : IObjectManagerRepository
    {
        IHelper _helper;
        int _workspaceID;


        public ObjectManagerRepository(IHelper helper, int workspaceID)
        {
            this._helper = helper;
            this._workspaceID = workspaceID;
        }

        public  Task<Permission> Permission_ReadSingleAsync()
        {
            
            GroupRef groupRef;
            Task<Permission> pv;
            using (IPermissionManager proxy = _helper.GetServicesManager().CreateProxy<IPermissionManager>(ExecutionIdentity.System))
            {
                groupRef = new GroupRef();
                pv =  proxy.ReadSingleAsync(_workspaceID, 44);
            }
            return pv;

        }

        public IEnumerable<CORE.DAL.Entities.Document> DocumentList()
        {
            List<CORE.DAL.Entities.Document> documents = new List<CORE.DAL.Entities.Document>();

            try
            {
                using (IRSAPIClient proxy =
                                    _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {
                    int workspaceId = _workspaceID;
                    proxy.APIOptions.WorkspaceID = workspaceId;
                    CORE.BLL.Service.RSAPIService.DocumentRSAPIService CRSAPIservice = new CORE.BLL.Service.RSAPIService.DocumentRSAPIService(proxy);
                    documents = CRSAPIservice.GetAll().ToList();
                    foreach (CORE.DAL.Entities.Document d in documents)
                    {
                        d.Comments = GetCommentChilds(d.Comments.ToList());
                    }
                }


            }

            catch (Exception)
            {
                throw;
            }

            return documents;
        }


        public List<CORE.DAL.Entities.Comment> GetCommentChilds(List<CORE.DAL.Entities.Comment> comments)
        {
            List<CORE.DAL.Entities.Comment> childs = new List<CORE.DAL.Entities.Comment>();
            List<CORE.DAL.Entities.Comment> newChilds = new List<CORE.DAL.Entities.Comment>();
            using (IRSAPIClient proxy =
                               _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                int workspaceId = _workspaceID;
                proxy.APIOptions.WorkspaceID = workspaceId;
                CORE.BLL.Service.RSAPIService.CommentRSAPIService cRSAPIService = new CORE.BLL.Service.RSAPIService.CommentRSAPIService(proxy);
                CORE.BLL.Service.SqlService.CommentSqlService commentService = new CORE.BLL.Service.SqlService.CommentSqlService(_helper.GetDBContext(workspaceId));
                foreach (CORE.DAL.Entities.Comment item in comments)
                {
                    childs = commentService.GetCommentsChild(item.ArtifactId);


                    if (!childs.Count().Equals(0))
                    {
                        foreach (CORE.DAL.Entities.Comment child in childs)
                        {
                            CORE.DAL.Entities.Comment comment = cRSAPIService.Get(child.ArtifactId);
                            newChilds.Add(comment);
                        }
                        item.CommentChilds = GetCommentChilds(newChilds);


                    }
                    else
                    {
                        item.CommentChilds = childs;
                    }


                }


            }
            return comments;

        }

        public IEnumerable<CORE.DAL.Entities.Comment> GetCommentReplys(int artifactId)
        {
            List<CORE.DAL.Entities.Comment> childs = new List<CORE.DAL.Entities.Comment>();
            List<CORE.DAL.Entities.Comment> newChilds = new List<CORE.DAL.Entities.Comment>();
            using (IRSAPIClient proxy =
                               _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                int workspaceId = _workspaceID;
                proxy.APIOptions.WorkspaceID = workspaceId;
                CORE.BLL.Service.RSAPIService.CommentRSAPIService cRSAPIService = new CORE.BLL.Service.RSAPIService.CommentRSAPIService(proxy);
                CORE.BLL.Service.SqlService.CommentSqlService commentService = new CORE.BLL.Service.SqlService.CommentSqlService(_helper.GetDBContext(workspaceId));
                childs = commentService.GetCommentsChild(artifactId);

                foreach (var child in childs)
                {
                    CORE.DAL.Entities.Comment comment = cRSAPIService.Get(child.ArtifactId);
                    newChilds.Add(comment);
                }
            }

            return newChilds;
        }


        public Theme getTheme()
        {
            DataRowCollection data;
            Theme theme = new Theme();
            using (IRSAPIClient proxy = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                int workspace = _workspaceID;
                IDBContext dbContext = _helper.GetDBContext(-1);
                string sql = $@"SELECT 
                                 
                                  [Value]
                                  
                              FROM 
                                    [eddsdbo].[InstanceSetting]
                              WHERE 
                                    Name LIKE '%Theme UI (light/dark)%'";
                data = dbContext.ExecuteSqlStatementAsDataTable(sql).Rows;
                foreach (DataRow item in data)
                {
                    foreach (var d in item.ItemArray)
                    {

                        theme.textValue = (string)d;

                    }
                }
                theme.value = theme.textValue.Equals("true") ? true : false;
                theme.textValue = theme.value ? "LIGHT" : "DARK";
            }

            return theme;
        }


        public int changeTheme(string value)
        {
            int data;
            using (IRSAPIClient proxy = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {

                int workspace = _workspaceID;
                IDBContext dbContext = _helper.GetDBContext(-1);
                string sql = $@"UPDATE [eddsdbo].[InstanceSetting]
                          SET [Value] = '{value}'
                          WHERE 
                          [Name] LIKE '%Theme UI (light/dark)%'";
                data = dbContext.ExecuteNonQuerySQLStatement(sql);
            }


            return data;
        }

        public IEnumerable<AuditComment> getCommentAudit(int commentId)
        {
            List<AuditComment> audit = new List<AuditComment>();
            DataRowCollection data;
            using (IRSAPIClient proxy = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                int workspace = _workspaceID;
                IDBContext dbContext = _helper.GetDBContext(workspace);
                string sql = $@"SELECT TOP (1000) [ArtifactId]
                                  ,[CommentId]
                                  ,[CreatedOn]
                                  ,[CreateByUserId]
                                  ,[CreatedByUserName]
                                  ,[ModifiedOn]
                                  ,[ModifiedByUserId]
                                  ,[ModifiedByUserName]
                                  ,[ReplysAmount]
                                  ,[comment]
                                  ,[type]
                              FROM [EDDSDBO].[AuditComment]
                              WHERE [CommentId] ={commentId};";
                data = dbContext.ExecuteSqlStatementAsDataTable(sql).Rows;
                foreach (DataRow item in data)
                {
                    AuditComment commentAudit = new AuditComment();
                    commentAudit.commentId = (int)item.ItemArray[1];
                    commentAudit.createdOn = (item.ItemArray[2]).ToString();
                    commentAudit.createByUserId = commentAudit.createdOn == string.Empty ? 0 : (int)item.ItemArray[3];
                    commentAudit.createdByUserName = commentAudit.createdOn == string.Empty ? "" : (string)item.ItemArray[4];
                    commentAudit.modifiedOn = (item.ItemArray[5]).ToString();
                    commentAudit.modifiedByUserId = commentAudit.createdOn == string.Empty ? (int)item.ItemArray[6] : 0;
                    commentAudit.modifiedByUserName = commentAudit.createdOn == string.Empty ? (string)item.ItemArray[7] : "";
                    commentAudit.replysAmount = (int)item.ItemArray[8];
                    commentAudit.comment = (string)item.ItemArray[9];
                    commentAudit.type = (string)item.ItemArray[10];
                    audit.Add(commentAudit);
                }
            }
            return audit;

        }

        public CORE.DAL.Entities.Comment GetCommentData(int commentAI)
        {
            CORE.DAL.Entities.Comment comment = new CORE.DAL.Entities.Comment();
            using (IObjectManager OM = _helper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
            {
                QueryRequest QR = new QueryRequest();
                QR.ObjectType = new ObjectTypeRef() { Name = "Comment" };
                QR.Condition = $"'Artifact ID' == {commentAI}";
                QR.Fields = new List<FieldRef>() {
                    new FieldRef() { Name = "Comment"},
                    new FieldRef() { Name = "System Created On"},
                    new FieldRef() { Name = "Thumbnail_Image_base64"}
                };
                var task = OM.QueryAsync(_workspaceID, QR, 1, int.MaxValue);
                task.Wait();
                comment.Name = task.Result.Objects.FirstOrDefault().FieldValues[0].Value.ToString();
                comment.CreatedOn = task.Result.Objects.FirstOrDefault().FieldValues[1].Value.ToString();
                comment.imageBase64 = task.Result.Objects.FirstOrDefault().FieldValues[2].Value.ToString();
            }

            return comment;
        }
        public CORE.DAL.Entities.Comment GetCommentDataByRsapi(int commentAI)
        {
            CORE.DAL.Entities.Comment comment = new CORE.DAL.Entities.Comment();
            using (IRSAPIClient proxy = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                proxy.APIOptions.WorkspaceID = _workspaceID;
                CORE.BLL.Service.RSAPIService.CommentRSAPIService csrsapi = new CORE.BLL.Service.RSAPIService.CommentRSAPIService(proxy);
                CORE.BLL.Service.SqlService.CommentSqlService cssql = new RelativityAppCore.BLL.Service.SqlService.CommentSqlService(_helper.GetDBContext(_workspaceID));
                comment = csrsapi.Get(commentAI);
                comment.CommentChilds = cssql.GetCommentsChild(comment.ArtifactId);
                List<CORE.DAL.Entities.Comment> childs = new List<RelativityAppCore.DAL.Entities.Comment>();
                foreach (var c in comment.CommentChilds)
                {
                    childs.Add(csrsapi.Get(c.ArtifactId));
                }
                comment.CommentChilds = childs;
                comment.CommentChilds = GetCommentChilds(comment.CommentChilds.ToList());
            }

            return comment;
        }

        public CORE.DAL.Entities.Comment getDataReplyCommentByObjectManager(int commentAI, int workspaceID)
        {
            CORE.DAL.Entities.Comment comment = new RelativityAppCore.DAL.Entities.Comment(commentAI);
            Relativity.Services.Objects.DataContracts.ReadResult result = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);
                client.DefaultRequestHeaders.Add("Authorization",
                    "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("relativity.admin@relativity.com:Nserio.1")));
                client.DefaultRequestHeaders.Add("X-Kepler-Version", "2.0");
                client.BaseAddress = new Uri("http://192.168.0.148/Relativity.REST/api/Relativity.Objects/");

                string inputJSON = $"{{\"Request\": {{\"Object\": {{\"ArtifactID\": {commentAI}}},\"Fields\": [{{\"Name\": \"Comment\"}},{{\"Name\": \"System Created On\"}}]}}}}";
                var url = $"workspace/{workspaceID}/object/read";
                var response = client.PostAsync(url, new StringContent(inputJSON, Encoding.UTF8, "application/json")).Result;
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<Relativity.Services.Objects.DataContracts.ReadResult>(content);
                comment.CreatedOn = result.Object.FieldValues[0].Value.ToString();
                comment.Name = result.Object.FieldValues[1].Value.ToString();
            }


            return comment;
        }

        public IEnumerable<CORE.DAL.Entities.Comment> GetReplysByObjectManager(int artifactId)
        {
            List<CORE.DAL.Entities.Comment> childs = new List<CORE.DAL.Entities.Comment>();
            List<CORE.DAL.Entities.Comment> newChilds = new List<CORE.DAL.Entities.Comment>();
            using (IRSAPIClient proxy =
                                _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                int workspaceId = _workspaceID;
                proxy.APIOptions.WorkspaceID = workspaceId;
                CORE.BLL.Service.RSAPIService.CommentRSAPIService cRSAPIService = new CORE.BLL.Service.RSAPIService.CommentRSAPIService(proxy);
                CORE.BLL.Service.SqlService.CommentSqlService commentService = new CORE.BLL.Service.SqlService.CommentSqlService(_helper.GetDBContext(workspaceId));
                childs = commentService.GetCommentsChild(artifactId);
                foreach (var child in childs)
                {
                    CORE.DAL.Entities.Comment comment = getDataReplyCommentByObjectManager(child.ArtifactId, workspaceId);
                    newChilds.Add(comment);
                }
            }

            return newChilds;
        }

        public int getAmountCommentByKepler(int documentAI)
        {
            int amount = 0;
            int workspaceID = -1;
            using (IRSAPIClient proxy =
                                _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                workspaceID = _workspaceID;

            }
            try
            {
                HttpWebResponse response = null;
                var req = (HttpWebRequest)WebRequest.Create("http://192.168.0.148/Relativity.REST/api/KeplerService.Comment.Services.Interfaces.ICommentModule/CommentManager/getAmountComments");
                req.Method = "POST";
                req.Timeout = int.MaxValue;
                req.ContentType = "application/json; charset=utf-8";

                req.Headers.Add("Authorization", $"Basic cmVsYXRpdml0eS5hZG1pbkByZWxhdGl2aXR5LmNvbTpOc2VyaW8uMQ==");
                req.Headers.Add("X-CSRF-Header", ".");
                req.Headers.Add("Content", "application/json");

                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    string json = $"{{\"documentAI\":\"{documentAI}\",\"workspaceAI\":\"{workspaceID}\"}}";

                    streamWriter.Write(json);
                    //streamWriter.Flush();
                    streamWriter.Close();
                }
                response = (HttpWebResponse)req.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream());
                amount = JsonConvert.DeserializeObject<int>(sr.ReadToEnd().Trim());
                if (response != null)
                {
                    response.Close();
                    response = null;

                }
                return amount;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public int getAmountReplysByKepler(int commentAI)
        {
            int amountReplys = 0;
            int workspaceID = -1;
            using (IRSAPIClient proxy =
                                _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                workspaceID = _workspaceID;

            }
            try
            {
                HttpWebResponse response = null;
                var req = (HttpWebRequest)WebRequest.Create("http://192.168.0.148/Relativity.REST/api/KeplerService.Comment.Services.Interfaces.ICommentModule/CommentManager/getAmountReplys");
                req.Method = "POST";
                req.Timeout = int.MaxValue;
                req.ContentType = "application/json; charset=utf-8";
                req.Headers.Add("Authorization", $"Basic cmVsYXRpdml0eS5hZG1pbkByZWxhdGl2aXR5LmNvbTpOc2VyaW8uMQ==");
                req.Headers.Add("X-CSRF-Header", ".");
                req.Headers.Add("Content", "application/json");

                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    string json = $"{{\"parentCommentAI\":\"{commentAI}\",\"workspaceAI\":\"{workspaceID}\"}}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                response = (HttpWebResponse)req.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream());
                amountReplys = JsonConvert.DeserializeObject<int>(sr.ReadToEnd().Trim());
                if (response != null)
                {
                    response.Close();
                    response = null;

                }
                return amountReplys;

            }
            catch (Exception)
            {

                throw;
            }

        }




    }
}
