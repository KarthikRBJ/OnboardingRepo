using RelativityAppCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using RelativityAppCore.DAL.Entities;
using kCura.Relativity.Client;

namespace RelativityAppCore.BLL.Service.RSAPIService
{
    public class CommentRSAPIService : ICommentService
    {
        enum typeGuids
        {
            error,
            review,
            correction,
            Improvement
        }
        IRSAPIClient client;
        DAL.Repositories.RSAPIRepository.CommentRepositoryRSAPI repository;
        public CommentRSAPIService(IRSAPIClient client)
        {
            this.client = client;
            repository = new DAL.Repositories.RSAPIRepository.CommentRepositoryRSAPI(client);
        }
        public int Create(Comment artifact)
        {
            int result = 0;
            int r = string.Compare("Improvement", artifact.Type, false) & string.Compare("correction", artifact.Type, false) & 
                                string.Compare("Error", artifact.Type, false) & string.Compare("review", artifact.Type, false);

            if(!r.Equals(0))
                throw new Exception("The comment type is incorrect");

            switch (artifact.Type)
            {
                case "Improvement":
                    artifact.TypeChoosed = artifact.IMPROVEMENT_TYPER_CHOICE_FIELD;
                    break;
                case "correction":
                    artifact.TypeChoosed = artifact.CORRECTION_TYPER_CHOICE_FIELD;
                    break;
                case "Error":
                    artifact.TypeChoosed = artifact.ERROR_TYPER_CHOICE_FIELD;
                    break;
                case "review":
                    artifact.TypeChoosed = artifact.REVIEW_TYPER_CHOICE_FIELD;
                    break;
                default:
                    break;
            }
            result = repository.Create(artifact);

            return result;
        }

        public bool Delete(int artifactId)
        {
            bool result;
            result = repository.Delete(artifactId);
            return result;
        }

        public Comment Get(int artifactId)
        {
            Comment comment = repository.Get(artifactId);
            return comment;
        }

        public IEnumerable<Comment> GetAll()
        {
            return repository.GetAll();
        }

        public bool Update(int artifactId, Comment artifact)
        {
            bool result;
            result= repository.Update(artifactId, artifact);
            return result;
        }

        public List<Comment> GetCommentsChild(int commentId)
        {
            throw new NotImplementedException();
        }

        public string GetFileFieldDownloadURL(int commentId)
        {
            string url = string.Empty;

            url = repository.GetFileFieldDownloadURL(commentId);
            return url;
        }

        public string GetFileFieldPath(int commentId)
        {
            throw new NotImplementedException();
        }
    }
}
