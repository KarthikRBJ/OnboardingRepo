using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CORE = RelativityAppCore;

namespace Custom_Pages.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult DocumentList()
        {
            List<CORE.DAL.Entities.Document> documents = new List<CORE.DAL.Entities.Document>();
            documents = ObjectManagerRepository.DocumentList().ToList();
            return Json(documents, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Permission_ReadSingleAsync()
        {
            var p = ObjectManagerRepository.Permission_ReadSingleAsync();
            return Json(p,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCommentReplys(int artifactId)
        {
            List<CORE.DAL.Entities.Comment> replys = new List<CORE.DAL.Entities.Comment>();
            replys = ObjectManagerRepository.GetCommentReplys(artifactId).ToList();

            return Json(replys, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getTheme()
        {
            var theme = ObjectManagerRepository.getTheme();
            return Json(theme, JsonRequestBehavior.AllowGet);
        }

        public ActionResult changeTheme(string value)
        {
            var data = ObjectManagerRepository.changeTheme(value);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getCommentAudit(int commentId)
        {
            var audit = ObjectManagerRepository.getCommentAudit(commentId);
            return Json(audit, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCommentData(int commentAI)
        {
            var comment = ObjectManagerRepository.GetCommentData(commentAI);
            return Json(comment, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCommentDataByRsapi(int commentAI)
        {
            var comment = ObjectManagerRepository.GetCommentDataByRsapi(commentAI);
            return Json(comment, JsonRequestBehavior.AllowGet);
        }

       

        public ActionResult GetReplysByObjectManager(int artifactId)
        {
            var newChilds = ObjectManagerRepository.GetReplysByObjectManager(artifactId);
            return Json(newChilds, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getAmountCommentByKepler(int documentAI)
        {
            var amount = ObjectManagerRepository.getAmountCommentByKepler(documentAI);
            return Json(amount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getAmountReplysByKepler(int commentAI)
        {
            var amountReplys = ObjectManagerRepository.getAmountReplysByKepler(commentAI);
            return Json(amountReplys, JsonRequestBehavior.AllowGet);
        }


    }
}
