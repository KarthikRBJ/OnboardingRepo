using RelativityAppCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using RelativityAppCore.DAL.Entities;
using kCura.Relativity.Client;
using DTOs = kCura.Relativity.Client.DTOs;

namespace RelativityAppCore.DAL.Repositories.RSAPIRepository
{
    class CommentRepositoryRSAPI : ICommentRepository
    {
        IRSAPIClient client;
        public CommentRepositoryRSAPI(IRSAPIClient client)
        {
            this.client = client;
        }
        public int Create(Comment artifact)
        {
            int result = 0;
            DTOs.RDO comment = new DTOs.RDO();
            comment.ArtifactTypeGuids.Add(new Guid(artifact.ARTIFACT_TYPE));
            comment.Fields.Add(new DTOs.FieldValue(new Guid(artifact.COMMENT), artifact.Name));
            comment.Fields.Add(new DTOs.FieldValue(new Guid(artifact.SINGLE_CHOICE_FIELD), new Guid(artifact.TypeChoosed)));
            try
            {
                result = client.Repositories.RDO.CreateSingle(comment);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public bool Delete(int artifactId)
        {
            bool result = false;
            try
            {
                client.Repositories.RDO.DeleteSingle(artifactId);
                result = true;
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        public Comment Get(int artifactId)
        {
            Comment comment = new Comment();
            try
            {
                DTOs.RDO commentDTO = new DTOs.RDO(artifactId);
                commentDTO.Fields = DTOs.FieldValue.AllFields;
                commentDTO.ArtifactTypeGuids.Add(new Guid(comment.ARTIFACT_TYPE));
                commentDTO = client.Repositories.RDO.ReadSingle(artifactId);
                comment.ArtifactId = commentDTO.ArtifactID;
                comment.Name = commentDTO.TextIdentifier;
                Entities.Artifact user = new Entities.Artifact(commentDTO.SystemCreatedBy.ArtifactID);
                user.Name = commentDTO.SystemCreatedBy.FullName;
                comment.CreatedBy = user;
                Entities.Artifact lastUser = new Entities.Artifact(commentDTO.SystemLastModifiedBy.ArtifactID);
                lastUser.Name = commentDTO.SystemCreatedBy.FullName;
                comment.LastModifiedBy = lastUser;
                comment.CreatedOn = commentDTO.SystemCreatedOn.ToString();
                comment.LastModifiedOn = commentDTO.SystemLastModifiedOn.ToString();
                comment.imageBase64 = (string)commentDTO[new Guid(comment.COMMENT_THUMBNAIL_FIELD)].Value;

            }
            catch (Exception)
            {

                throw;
            }
          
            return comment;
        }

        public IEnumerable<Comment> GetAll()
        {
            Comment comment = new Comment();
            DTOs.Query<DTOs.RDO> query = new DTOs.Query<DTOs.RDO>();
            DTOs.QueryResultSet<DTOs.RDO> results = new DTOs.QueryResultSet<DTOs.RDO>();
            query.ArtifactTypeGuid = new Guid(comment.ARTIFACT_TYPE);
            query.Fields.Add(new DTOs.FieldValue(new Guid(comment.COMMENT), "Comment"));
            List<Comment> commentList = new List<Comment>();
            try
            {
                results = client.Repositories.RDO.Query(query);
                foreach (var c in results.Results)
                {
                    Comment commentToAdd = new Comment();
                    commentList.Add(Get(c.Artifact.ArtifactID));
                }
            }
            catch (Exception)
            {

                throw;
            }
            return commentList;
        }

        public List<Comment> GetCommentsChild(int commentId)
        {
            throw new NotImplementedException();
        }

        public string GetFileFieldDownloadURL(int commentId)
        {
            Comment comment = new Comment(commentId);
            string url = "";
            var downloadUrlRequest = new DownloadURLRequest(client.APIOptions);

            downloadUrlRequest.BaseURI = new Uri("http://192.168.0.148");
            downloadUrlRequest.Target.ObjectArtifactId = commentId;
            downloadUrlRequest.Target.FieldGuid = new Guid(comment.COMMENT_IMAGE_FIELD);

            DownloadURLResponse response;

            try
            {
                response = client.Repositories.RDO.GetFileFieldDownloadURL(downloadUrlRequest);
            }
            catch (Exception)
            {

                throw;
            }


            if (response.Success)
            {
                url = response.URL;
            }

            return url;
        }

        public string GetFileFieldPath(int commentId)
        {
            throw new NotImplementedException();
        }


        public bool Update(int artifactId, Comment artifact)
        {
            DTOs.RDO commentToUpdate = client.Repositories.RDO.ReadSingle(artifactId);
            commentToUpdate.Fields.Add(new DTOs.FieldValue() { Name = "Comment", Value = artifact.Name });
            try
            {
                client.Repositories.RDO.UpdateSingle(commentToUpdate);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
