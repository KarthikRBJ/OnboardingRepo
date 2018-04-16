using RelativityAppCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelativityAppCore.DAL.Entities;
using RelativityAppCore.DAL.Connection.SqlConnection;
using Relativity.API;

namespace RelativityAppCore.BLL.Service.SqlService
{
    public class CommentSqlService : ICommentService
    {
        IDBContext DBContext;
        DAL.Repositories.SqlRepository.CommentRepositorySql repository;
        public CommentSqlService(IDBContext dbContext)
        {
            DBContext = dbContext;
            repository = new DAL.Repositories.SqlRepository.CommentRepositorySql(dbContext);
        }

        public int Create(Comment artifact)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int artifactId)
        {
            throw new NotImplementedException();
        }

        public Comment Get(int artifactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comment> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Comment> GetCommentsChild(int commentId)
        {
            return repository.GetCommentsChild(commentId);
        }

        public string GetFileFieldDownloadURL(int commentId)
        {
            throw new NotImplementedException();
        }

        public string GetFileFieldPath(int commentId)
        {
            string path = repository.GetFileFieldPath(commentId);
            return path;
        }

        public bool Update(int artifactId, Comment artifact)
        {
            throw new NotImplementedException();
        }
    }
}
