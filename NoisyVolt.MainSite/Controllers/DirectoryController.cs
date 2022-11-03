using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoisyVolt.Models;
using NoisyVolt.Services;

namespace NoisyVolt.MainSite.Controllers
{
    public class DirectoryController : Controller
    {

        private INoSqlDataService _noSqlDataService;

        public DirectoryController(INoSqlDataService noSqlDataService)
        {
            _noSqlDataService = noSqlDataService;
        }



        // GET: DirectoryController
        public ActionResult Index()
        {
            var items = _noSqlDataService.GetEmbeds().Result;
            return View(items);
        }

        // GET: DirectoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

      
    }
}
