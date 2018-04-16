using RelativityAppCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelativityAppCore.DAL.Entities;
using RelativityAppCore.DAL.Connection.SqlConnection;
using Relativity.API;
using System.Data;

namespace RelativityAppCore.DAL.Repositories.SqlRepository
{
    class CommentRepositorySql : ICommentRepository
    {

        IDBContext DBContextSql;

        public CommentRepositorySql(IDBContext dbContext)
        {
            DBContextSql = dbContext;
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
            string sql = $@"SELECT [ArtifactID]
                            FROM [{DBContextSql.Database}].[EDDSDBO].[Comment]
                            Where
                            RelatedComments = {commentId}";
            List<Comment> commentsChild = new List<Comment>();
            DataRowCollection CommentsChildsList = DBContextSql.ExecuteSqlStatementAsDataTable(sql).Rows;

            foreach (DataRow childs in CommentsChildsList)
            {
                foreach(var childArtifacId in childs.ItemArray)
                {
                    Comment comment = new Comment((int)childArtifacId);
                    commentsChild.Add(comment);
                }
            }

            return commentsChild;
        }

        public string GetFileFieldDownloadURL(int commentId)
        {
            throw new NotImplementedException();
        }

        public string GetFileFieldPath(int commentId)
        {
            string path = string.Empty;
            string sql = $@"SELECT
                                [Location]
                            FROM 
                                [EDDS1027611].[EDDSDBO].[File1043239]
                            where 
                                ObjectArtifactID = {commentId}";
            DataRowCollection data = DBContextSql.ExecuteSqlStatementAsDataTable(sql).Rows;
            if (!data.Count.Equals(0))
                path = data[0].ItemArray[0].ToString();

            return path;
        }

        public bool Update(int artifactId, Comment artifact)
        {
            throw new NotImplementedException();
        }
    }
}
