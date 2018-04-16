using CommentPageCore;
using CommentPageCore.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CP = Relativity.CustomPages.ConnectionHelper;


namespace Custom_Pages.Controllers
{
    public class BaseController : Controller
    {
        private IObjectManagerRepository _repository;

        public IObjectManagerRepository ObjectManagerRepository
        {
            get
            {
                if(_repository == null)
                {
                    _repository = new ObjectManagerRepository(CP.Helper(), CP.Helper().GetActiveCaseID());
                }
                return _repository;
            }
        }
    }
}