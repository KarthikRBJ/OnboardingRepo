using System.Collections.Generic;
using RelativityAppCore.DAL.Entities;
using CommentPageCore.Entities;
using System.Threading.Tasks;
using Relativity.Services.Permission;

namespace CommentPageCore
{
    public interface IObjectManagerRepository
    {
        IEnumerable<Document> DocumentList();
        List<Comment> GetCommentChilds(List<Comment> comments);
        IEnumerable<Comment> GetCommentReplys(int artifactId);
        Theme getTheme();
        int changeTheme(string value);
        IEnumerable<AuditComment> getCommentAudit(int commentId);
        Comment GetCommentData(int commentAI);
        Comment GetCommentDataByRsapi(int commentAI);
        Comment getDataReplyCommentByObjectManager(int commentAI, int workspaceID);
        IEnumerable<Comment> GetReplysByObjectManager(int artifactId);
        int getAmountCommentByKepler(int documentAI);
        int getAmountReplysByKepler(int commentAI);
        Task<Permission> Permission_ReadSingleAsync();
    }
}
