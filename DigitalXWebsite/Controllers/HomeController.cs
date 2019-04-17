using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace DigitalXWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ServiceReference.Service1Client product = new ServiceReference.Service1Client();
            var p = product.GetPopularProducts();

            return View(p);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View(new { returnURL = "/Home/About" });
        }

        [HandleError]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [OutputCache(Duration = 600, Location = OutputCacheLocation.Server, VaryByParam = "id")]
        public FileContentResult GetImage(int id)
        {
            ServiceReference.Service1Client product = new ServiceReference.Service1Client();
            var pro = product.GetProduct(id);

            return File(pro.Picture, "image/jpeg");
        }
    }
}