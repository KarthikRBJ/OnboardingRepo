using RelativityAppCore.DAL.Entities;
using RelativityAppCore.Repositories;
using System.Collections.Generic;

namespace RelativityAppCore.Interfaces.Repositories
{
    interface ICommentRepository: IGenericRepository<Comment>
    {
        List<Comment> GetCommentsChild(int commentId);

        string GetFileFieldDownloadURL(int commentId);

        string GetFileFieldPath(int commentId);
    }
}
