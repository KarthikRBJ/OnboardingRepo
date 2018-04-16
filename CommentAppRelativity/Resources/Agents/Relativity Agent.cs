using System;
using System.Collections.Generic;
using kCura.Agent;
using kCura.Relativity.Client;
using Relativity.API;
using System.Data;
using DTOs = kCura.Relativity.Client.DTOs;
using RelativityAppCore.DAL.Entities;
using System.IO;
using CORE = RelativityAppCore;
using System.Drawing;
using System.Drawing.Imaging;
using Resources;

namespace Agents_EXample
{
    [kCura.Agent.CustomAttributes.Name("Basic Agent")]
    [System.Runtime.InteropServices.Guid("F05EA339-7C01-4014-84F0-6FBE13CA4295")]



    public class RelativityAgent : AgentBase
    {

        string guidSet = "A727D354-5DC6-433C-9A95-6DB25053873A";
        string thumbnailsImage = "241FD1B9-85B4-4251-BE46-93D39EFC3616";
        string Image_guid_field = "D6F64A63-9619-405D-95E0-9B2E5556AE57";
        
        
        public override void Execute()
        {
            int contador = 0;
            
            //Get the current Agent artifactID
            Int32 agentArtifactID = this.AgentID;
            //Get a dbContext for the EDDS database
            IDBContext eddsDBContext = this.Helper.GetDBContext(-1);
            List<int> worksapcesID = new List<int>();
            Comment comment = new Comment();


            try
            {



                using (IRSAPIClient proxy =
                    Helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {

                    RelativityAppCore.BLL.Service.RSAPIService.CommentRSAPIService commentRSAPIService = new CORE.BLL.Service.RSAPIService.CommentRSAPIService(proxy);
                    IDBContext DBContext = this.Helper.GetDBContext(-1);
                    DataRowCollection data = DBContext.ExecuteSqlStatementAsDataTable(Queries.GetWorkspacesWithApp).Rows;
                    RaiseMessage("Find for the workspaces with the application", 1);
                    foreach (var item in data[0].ItemArray)
                    {
                        worksapcesID.Add((int)item);
                    }


                    foreach (var item in worksapcesID)
                    {
                        proxy.APIOptions.WorkspaceID = item;
                        DTOs.Query<DTOs.RDO> query = new DTOs.Query<DTOs.RDO>();
                        DTOs.QueryResultSet<DTOs.RDO> results = new DTOs.QueryResultSet<DTOs.RDO>();
                        query.ArtifactTypeGuid = new Guid(comment.ARTIFACT_TYPE);
                        query.Fields = DTOs.FieldValue.AllFields;
                       // query.Condition = new BooleanCondition(new Guid(guidSet), BooleanConditionEnum.EqualTo, false);
                        
                        try
                        {

                            results = proxy.Repositories.RDO.Query(query);
                            
                            foreach (var c in results.Results)

                            {
                                RaiseMessage($"verifying if the comment:  {c.Artifact.ArtifactID} already has the thumnails", 1);
                                DTOs.RDO commentDto = new DTOs.RDO(c.Artifact.ArtifactID);
                                commentDto.ArtifactTypeGuids.Add(new Guid(comment.ARTIFACT_TYPE));
                                commentDto.Fields = DTOs.FieldValue.AllFields;
                                commentDto.Fields.Add(new DTOs.FieldValue(new Guid(guidSet)));
                                commentDto = proxy.Repositories.RDO.ReadSingle(c.Artifact.ArtifactID);
                                //bool fieldValue = (bool)commentDto[new Guid(guidSet)].Value;
                                string image = (string)commentDto[new Guid(Image_guid_field)].Value;

                                if (!string.IsNullOrEmpty(image))
                                {
                                    RaiseMessage($"Creating Thumbnails for the comment {c.Artifact.ArtifactID}", 1);
                                    string thumbnail = getImage(c.Artifact.ArtifactID, this.Helper.GetDBContext(proxy.APIOptions.WorkspaceID));
                                    commentDto.Fields.Add(new DTOs.FieldValue(new Guid(thumbnailsImage), thumbnail));
                                    commentDto.Fields.Add(new DTOs.FieldValue(new Guid(guidSet), true));
                                    proxy.Repositories.RDO.UpdateSingle(commentDto);
                                    
                                    
                                }
                                else
                                {
                                    contador = contador + 1;
                                    commentDto.Fields.Add(new DTOs.FieldValue(new Guid(guidSet), false));
                                    proxy.Repositories.RDO.UpdateSingle(commentDto);



                                }

                            }
                        }


                        catch (Exception)
                        {

                            throw;
                        }
                    }


                }


                RaiseMessage($"There are {contador} comments without thumbnail", 1);



            }
            catch (System.Exception ex)
            {
                //Your Agent caught an exception
                this.RaiseError(ex.Message, ex.Message);
            }
            
        }

        public string getImage(int commentId, IDBContext DBContext)
        {
            string imageBase64 = string.Empty;
            string path = string.Empty;
            CORE.BLL.Service.SqlService.CommentSqlService CSService = new CORE.BLL.Service.SqlService.CommentSqlService(DBContext);
            path = CSService.GetFileFieldPath(commentId);

            if (!path.Equals(string.Empty))
            {
                Image image = Image.FromFile(path);
                using (Image thumbnail = image.GetThumbnailImage(50, 50, () => false, IntPtr.Zero))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        thumbnail.Save(memoryStream, ImageFormat.Png);
                        Byte[] bytes = new Byte[memoryStream.Length];
                        memoryStream.Position = 0;
                        memoryStream.Read(bytes, 0, (int)bytes.Length);
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                        imageBase64 = "data:image/png;base64," + base64String;
                    }
                }
            }


            return imageBase64;
        }
        /**
		 * Returns the name of agent
		 */
        public override string Name
        {
            get
            {
                return "Basic Agent Sample";
            }
        }



    }
}
